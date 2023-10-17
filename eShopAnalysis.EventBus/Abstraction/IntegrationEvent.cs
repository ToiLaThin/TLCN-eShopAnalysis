using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eShopAnalysis.EventBus.Abstraction
{
    public record IntegrationEvent
    {
        [JsonInclude]
        public Guid Id { get; private set; }

        [JsonInclude]
        public DateTime CreationDate {
            get;
            private set;
        }

        [JsonConstructor] //deserializing a JSON to a class using this
        public IntegrationEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }
    }
}
