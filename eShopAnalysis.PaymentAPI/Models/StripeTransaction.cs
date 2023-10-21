namespace eShopAnalysis.PaymentAPI.Models
{
    public class StripeTransaction
    {

        //similar to TransactionId
        public string PaymentIntentId { get; set; }

        //foreign key to customer user mapping key
        public string CustomerId { get; set; }

        public Guid OrderId { get; set; }

        //since this is always payment by card
        //need card types?
        public string CardId { get; set; }

        public PaymentStatus TransactionStatus { get; set; }


        public double SubTotal { get; set; }

        public double TotalDiscount { get; set; }
        public double Tax { get; set; }

        public double Total { get; set; }
    }
}
