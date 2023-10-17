using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopAnalysis.EventBus.Abstraction
{
    public interface IIntegrationEventHandler { }
    public interface IIntegrationEventHandler<in T> : IIntegrationEventHandler
        where T : IntegrationEvent
    {
        Task Handle(T @event);
    }

    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
