using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.EventBus.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.EventBus.Extension
{
    public static class EventBusExtension
    {
        //just some helper
        public static string GetRequiredValue(this IConfiguration configuration, string name) =>
        configuration[name] ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");

        public static string GetRequiredConnectionString(this IConfiguration configuration, string name) =>
            configuration.GetConnectionString(name) ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":ConnectionStrings:" + name : "ConnectionStrings:" + name)}");

        //pass in the builder.Configuration
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var eventBusSection = configuration.GetSection("EventBus");
            if (!eventBusSection.Exists()) { return services; }

            //since we only have rabbit mq, do not need if else
            //this will create an instance of DefaultRabbitMQPersistentConnection and EventBusRabbitMQ with required conf in settings file at run time
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = configuration.GetRequiredConnectionString("EventBus"),
                    DispatchConsumersAsync = true
                };
                //only need host name, username and password are optional
                if (!string.IsNullOrEmpty(eventBusSection["UserName"]))
                {
                    factory.UserName = eventBusSection["UserName"];
                }
                if (!string.IsNullOrEmpty(eventBusSection["Password"]))
                {
                    factory.Password = eventBusSection["Password"];
                }
                var retryCount = 5;
                if (!string.IsNullOrEmpty(eventBusSection["RetryCount"]))
                {
                    try { retryCount = Int32.Parse(eventBusSection.Value); }
                    catch
                    {
                        retryCount = 5;
                    }
                }
                //get the required value and call the constructor not the empty constructor
                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });

            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var subscriptionClientName = eventBusSection.GetRequiredValue("SubscriptionClientName");
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                var retryCount = 5;
                if (!string.IsNullOrEmpty(eventBusSection["RetryCount"]))
                {
                    try { retryCount = Int32.Parse(eventBusSection.Value); }
                    catch
                    {
                        retryCount = 5;
                    }
                }
                //get the required value and call the constructor not the empty constructor
                //subscription client name la queue name
                return new EventBusRabbitMQ(rabbitMQPersistentConnection, eventBusSubscriptionsManager, sp, logger, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
    }
}
