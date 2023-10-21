namespace eShopAnalysis.PaymentAPI.Dto
{
    public record PaymentRequestDto
    {
        public Guid UserId { get; }

        public Guid OrderId { get; }

        public string? CardId { get; }

        public double SubTotal { get; }

        public double TotalDiscount { get; }
    }
}
