using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.EventBus.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved; //
        void Clear();
        string GetEventKey<T>();

        void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        bool HasSubscriptionForEvent<T>() where T : IntegrationEvent;

        IEnumerable<SubscriptionInfo> GetHandlerForEvent<T>() where T : IntegrationEvent;

        #region For dynamic type
        void AddDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        bool HasSubscriptionForEvent(string eventName);

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        #endregion
    }
}
