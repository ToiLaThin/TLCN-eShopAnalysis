namespace eShopAnalysis.CartOrderAPI.Application.Queries
{
    public record OrderDraftViewModel
    {
        public Guid OrderId { get;  }
    }

    public record OrderViewModel
    {
        public Guid OrderId { get; set; }
    }
}
