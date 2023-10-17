using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.EventBus.RabbitMQ
{
    //resilient rabbitMQ with retry polly enhancement
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
    {
        private IConnection _amqpConnection;
        private readonly int _retryCount;
        public bool Disposed = false;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
        readonly object _synRoot = new(); //?

        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? 
                throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? 
                throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
        }
        public bool IsConnected => _amqpConnection is { IsOpen: true } && !Disposed;

        public IModel CreateModel()
        {
            if (!IsConnected) {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }
            return _amqpConnection.CreateModel();
        }

        public void Dispose()
        {
            if (Disposed) { return; }
            Disposed = true;
            try
            {
                _amqpConnection.ConnectionShutdown -= OnConnectionShutdown;
                _amqpConnection.CallbackException -= OnCallbackException;
                _amqpConnection.ConnectionBlocked -= OnConnectionBlocked;
                _amqpConnection.Dispose();
            }
            catch (IOException ex) {
                _logger.LogCritical(ex.ToString());
            }

        }

        public bool TryConnect()
        {
            _logger.LogInformation("Rabbit MQ client is trying to connect");

            lock(_synRoot) {
                var policy = RetryPolicy
                    .Handle<SocketException>()
                    .Or<BrokerUnreachableException>() //exception by rabbit mq client will caused to retry
                    .WaitAndRetry(
                        _retryCount, //pass in the callback in the form of lambda
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) => { 
                            _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s", $"{time.TotalSeconds:n1}"); 
                        }
                    );

                policy.Execute(() =>
                {
                    _amqpConnection = _connectionFactory.CreateConnection();
                });

                if (IsConnected) { //retry on a few event
                    _amqpConnection.ConnectionShutdown += OnConnectionShutdown;
                    _amqpConnection.ConnectionBlocked += OnConnectionBlocked;
                    _amqpConnection.CallbackException += OnCallbackException;
                    _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _amqpConnection.Endpoint.HostName);
                    return true;
                }
                else
                {
                    //connection failed even after retry
                    _logger.LogCritical("Fatal Error: RabbitMQ connections could not be created and opened");
                    return false;
                }
            }

        }

        #region Callbacks
        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (Disposed) 
                return;
            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (Disposed) 
                return;
            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (Disposed) 
                return;
            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            TryConnect();
        }
        #endregion
    }
}
