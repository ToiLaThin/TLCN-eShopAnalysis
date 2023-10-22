namespace eShopAnalysis.PaymentAPI.Utilities
{
    

    public class MomoSetting
    {
        public string PartnerCode { get; set; }

        public string PartnerName { get; set; }

        public string StoreId { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set;  }

        public string PaymentSuccessRedirectUrl { get; set; } 

        public string PaymentCancelUrl { get; set; }

        public string InstantPaymentNotificationUrl { get; set; }

        public string RefundUrl { get; set; }

    }

    public class StripeSetting
    {
        public string PublishKey { get; set; }
        public string SecretKey { get; set; }

        public string SuccessUrl { get; set; }

        public string CancelUrl { get; set; }

        public string MetaOrderKey { get; set; }
    }
}
