namespace eShopAnalysis.PaymentAPI.Models
{
    public class UserCustomerMapping
    {
        public Guid UserId { get; set; }

        public string CustomerId { get; set; } //in the format of cus_..... by
    }
}
