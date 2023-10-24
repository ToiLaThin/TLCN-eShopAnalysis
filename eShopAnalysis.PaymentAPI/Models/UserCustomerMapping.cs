namespace eShopAnalysis.PaymentAPI.Models
{
    public class UserCustomerMapping
    {
        public Guid UserId { get; set; }

        public string CustomerId { get; set; } //in the format of cus_..... by
    }

    public enum PaymentMethod
    {
        COD = 0,
        Momo = 1,
        CreditCard = 2
    }
}
