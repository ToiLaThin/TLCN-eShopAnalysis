using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.EventBus.Abstraction
{
    public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionManager
    {
        //IE handlers for an event with key is IE type(name)
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;
        public event EventHandler<string> OnEventRemoved;
        //delegate/event will be assigned a callback so when this is raised, the callback executed
        //called when an event do not have any handler left, not when the dict have no keys(event)
        public InMemoryEventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public bool IsEmpty => _handlers is { Count: 0};

        public void Clear() => _handlers.Clear();        

        public string GetEventKey<T>() => typeof(T).Name;

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

        public bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return _handlers.ContainsKey(key);
        }

        public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public IEnumerable<SubscriptionInfo> GetHandlerForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return _handlers[key];
        }


        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            if(!HasSubscriptionForEvent<T>()) {
                _handlers.Add(eventName, new List<SubscriptionInfo>() { });
            }
            if (_handlers[eventName].Any(s => s.HandlerType == typeof(TH))) {
                throw new ArgumentException($"Handler Type {typeof(TH).Name} already registered for '{eventName}'");
            }

            _handlers[eventName].Add(SubscriptionInfo.Typed(typeof(TH)));

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            var subsToRemove = !HasSubscriptionForEvent<T>() ? 
                null : _handlers[eventName].SingleOrDefault(s => s.HandlerType == typeof(TH));

            if (subsToRemove != null) {
                _handlers[eventName].Remove(subsToRemove);
                if (!_handlers[eventName].Any()) {
                    _handlers.Remove(eventName);
                    var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (eventType != null) {
                        _eventTypes.Remove(eventType);
                    }
                    RaiseOnEventRemoved(eventName);
                }
            }

        }

        #region Private method used internally
        //this executed the callback
        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }
        #endregion

        #region For Dynamic Event
        public void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }
        public void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }               

        #endregion
    }
}
