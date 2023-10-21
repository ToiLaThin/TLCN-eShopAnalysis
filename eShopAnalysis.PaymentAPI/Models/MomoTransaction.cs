namespace eShopAnalysis.PaymentAPI.Models
{
    public enum PaymentStatus
    {
        Pending = 0, //payment inited and waiting to become locked
        Locked = 1, //cannot be canceled
        Completed = 2, //from locked to completed on delivery done
        Cancelled //before the status changed to locked, user cancelled the order then the payment for that order set to this
    }

    public class MomoTransaction
    {
        //in case paying with momo, this will be orderId
        public Guid TransactionId { get; set; }

        //foreign key to customer user mapping key
        public string CustomerId { get; set; }

        public PaymentStatus TransactionStatus { get; set; }

        public double SubTotal { get; set; }

        public double TotalDiscount { get; set; }

        public double Tax { get; set; }

        public double Total { get; set; }
    }
}
