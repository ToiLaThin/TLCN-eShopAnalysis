using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.EventBus.Abstraction
{
    public class SubscriptionInfo
    {
        public bool IsDynamic { get; }

        public Type HandlerType { get; }

        private SubscriptionInfo(bool isDynamic, Type handlerType)
        {
            IsDynamic = isDynamic;
            HandlerType = handlerType;
        }

        //factory method to create an instance with specify type
        public static SubscriptionInfo Dynamic(Type handlerType) => new SubscriptionInfo(isDynamic: true, handlerType: handlerType);
        public static SubscriptionInfo Typed(Type handlerType) => new SubscriptionInfo(isDynamic: false, handlerType: handlerType);
    }
}
