namespace eShopAnalysis.PaymentAPI.Dto
{
    public record PaymentRequestDto
    {
        public Guid UserId { get; set; }

        public Guid OrderId { get; set; }

        public string? CardId { get; set; }

        public double SubTotal { get; set; }

        public double TotalDiscount { get; set; }
    }
}
