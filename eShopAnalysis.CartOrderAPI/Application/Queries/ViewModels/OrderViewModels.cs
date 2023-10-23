using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;

namespace eShopAnalysis.CartOrderAPI.Application.Queries
{
    public record OrderDraftViewModel
    {
        public Guid OrderId { get;  }

        public double SubTotal { get; }

        public double TotalDiscount { get; }
    }

    public record OrderViewModel
    {
        public Guid OrderId { get; set; }
    }
}
