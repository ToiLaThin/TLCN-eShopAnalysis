using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace eShopAnalysis.EventBus.RabbitMQ
{
    public  class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private const string BROKER_NAME = "eshop_event_bus";
        private static readonly JsonSerializerOptions s_indentedOptions = new() { WriteIndented = true };
        private static readonly JsonSerializerOptions s_caseInsensitiveOptions = new() { PropertyNameCaseInsensitive = true };

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IEventBusSubscriptionManager _subsManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _retryCount;
        private readonly ILogger<EventBusRabbitMQ> _logger;

        private IModel _consumerChannel; //just used for receiving and handling message from a queue with _queueName
        private string _queueName;
        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection,
                                IEventBusSubscriptionManager subsManager,
                                IServiceProvider serviceProvider,
                                ILogger<EventBusRabbitMQ> logger,
                                string queueName = null, //la ten cua microservice client
                                int retryCount = 5)
        {
            _serviceProvider = serviceProvider;
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _subsManager.OnEventRemoved += SubsManager_OnEventRemoved; //called when an event do not have any handler left, not when the dict have no keys(event)
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
        }

        //mostly unbind the queue and the exchange with the routing key(match the event name) in direct mode
        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_persistentConnection.IsConnected) { _persistentConnection.TryConnect(); } //make sure we still have connection
            using var channel = _persistentConnection.CreateModel();
            channel.QueueUnbind(queue: _queueName, exchange: BROKER_NAME, routingKey: eventName);

            if (_subsManager.IsEmpty) {
                _queueName = String.Empty;
                _consumerChannel.Close();
            }
        }

        //get list of subscription and handler, then execute it depend on the type of integration event
        private async Task ProcessEvent(string eventName, string message)
        {
            _logger.LogTrace(eventName, "Processing RabbitMQ event: { EventName }", eventName);
            if (_subsManager.HasSubscriptionForEvent(eventName))
            {
                await using var scope = _serviceProvider.CreateAsyncScope();
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach ( var subscription in subscriptions ) {
                    if (subscription.IsDynamic)
                    {//get the handler then execute
                        if (scope.ServiceProvider.GetService(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler) continue; //check type, if false then pass
                        using dynamic eventData = JsonDocument.Parse(message); //can do because of dynamic
                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        //using meta programming and reflection to execute the handler since this is execute in runtime
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                        if (handler == null) { continue; }
                        var eventType = _subsManager.GetEventTypeByName(eventName); //using subsMan to get the type using the internal list of type 
                        var integrationEvent = JsonSerializer.Deserialize(message, eventType, s_caseInsensitiveOptions);
                        //the reason why some prop not deserialize correctly because we lack [Jsonproperty] to instruct in to deserialize, (in both the producer and consumer, we must have jsson property and jsson constructor)
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType); //construct the generic type for the event handler with a integration event type

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent }); //invoke the method with integration event as argument
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: { EventName }", eventName);
            }
        }

        //will get the message and event name from event args, since this is trigger by the event(delegate) 
        //then call ProcessEvent which officially process the event with handlers.Handle() in subsManager
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey; //routing key is event name by our convention
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try {
                if (message.ToLowerInvariant().Contains("throw-fake-exception")) { throw new InvalidOperationException($"Fake exception requested: \"{message}\"");}
                await ProcessEvent(eventName, message);
            } catch (Exception ex) {
                _logger.LogWarning(ex, "Error processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        //this will set up the consumer channel to use Consumer_Received to handle message received via queue with _queueName
        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");
            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                //this will wrap consumer channel with a few event, we can assign callback to this which will be called on the event trigger
                consumer.Received += Consumer_Received;
                //set up this consummer channel to process the message with consumer declared (Consumer_Received will be used to handle basic delivery when message arrive)
                //using the queue with name
                _consumerChannel.BasicConsume(
                    queue: _queueName,
                    autoAck: false, //client explicit set message is received
                    consumer: consumer);
            } else {
                _logger.LogError("Start Basic Consume can't call on _consumerChannel == null");
            }
        }

        //declate a channel with  a queue and an exchange with BROKER NAME and _queuename, on exception, call the callback which startBasicConsume again
        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected) { _persistentConnection.TryConnect(); } //must have connection to create channel

            _logger.LogTrace("Creating RabbitMQ consumer channel");
            var channel = _persistentConnection.CreateModel();
            channel.ExchangeDeclare(exchange: BROKER_NAME, type: ExchangeType.Direct);
            channel.QueueDeclare(queue: _queueName, //default is empty AKA to be generated
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.CallbackException += (sender, evtArgs) =>
            {
                _logger.LogWarning(evtArgs.Exception, "Recreating RabbitMQ consumer channel");
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel(); //? recursion but on condition
                StartBasicConsume();

            };
            return channel;
        }
        
        //if not have any handler for and integration event yet, bind the queue to an existing exchange with a routing key that match this integration event name
        //if it does have, simple do nothing
        private void DoInternalSubscription(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                _consumerChannel.QueueBind(queue: _queueName,
                                    exchange: BROKER_NAME,
                                    routingKey: eventName);
            }
        }
        public void Dispose()
        {
            if (_consumerChannel != null) { _consumerChannel.Dispose(); }

            _subsManager.Clear();
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected) { _persistentConnection.TryConnect(); }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, 
                              retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                              (ex, time) => {
                                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s", @event.Id, $"{time.TotalSeconds:n1}");
                              });

            var eventName = @event.GetType().Name;

            //one time use channel, the next time something publish will create another channel
            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);
            using var channel = _persistentConnection.CreateModel();
            _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);
            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), s_indentedOptions);
            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                //publish to exchange with a routing key, depend on that routing key, it will send the body to the matching queue
                channel.BasicPublish(
                    exchange: BROKER_NAME,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }

        //will be called on microservices
        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            DoInternalSubscription(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());
            _subsManager.AddSubscription<T, TH>();
            StartBasicConsume(); //repeated declare for each time some handler and event subscribe?
        }

        //remove handler for event from subsMan, if no handler left for and event , call SubsManager_OnEventRemoved to unbind the queue and exchange with routing key = eventName
        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            _subsManager.RemoveSubscription<T, TH>();
        }
    }
}
