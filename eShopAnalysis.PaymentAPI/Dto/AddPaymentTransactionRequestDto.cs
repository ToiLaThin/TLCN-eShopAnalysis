using Stripe;

namespace eShopAnalysis.PaymentAPI.Dto
{
    public record AddPaymentTransactionRequestDto
    {
        //record can only be instantiate within this CreateInstance
        public string? StripePaymentIntentId { get; private init; }  //for stripe only

        public Dictionary<string, string>? StripePaymentIntentMeta { get; private init; } //for stripe only

        public string? StripeCardId { get; private init; }  //for stripe only

        public Guid? MomoOrderId { get; private init; }  //for momo only, in stripe we would not get the orderId to pass in IPaymentTransactionRepository the from this but from the paymentIntentMeta, and let the paymentStrategu do that

        public string CustomerId { get; private init; }

        public double SubTotal { get; private init; }
        public double Discount { get; private init; }

        private AddPaymentTransactionRequestDto(
                string? stripePaymentIntentId,
                Dictionary<string, string>? stripePaymentIntentMeta,
                string? stripeCardId,
                Guid? momoOrderId,
                string customerId,
                double subTotal,
                double discount)
        {
            StripePaymentIntentId = stripePaymentIntentId;
            StripePaymentIntentMeta = stripePaymentIntentMeta;
            StripeCardId = stripeCardId;
            MomoOrderId = momoOrderId;
            CustomerId = customerId;
            SubTotal = subTotal;
            Discount = discount;
        }

        public static AddPaymentTransactionRequestDto CreateStripeInstance(
                string stripePaymentIntentId,
                Dictionary<string, string> stripePaymentIntentMeta,
                string stripeCardId,
                string customerId,
                double subTotal,
                double discount
            )
        {
            //validate the params with predefined condition
            bool paymentIntentIdIsCorrectFormat = stripePaymentIntentId.StartsWith("pi_");
            bool customerIdIsCorrectFormat = customerId.StartsWith("cus_");
            bool cardIdIsCorrectFormat = stripeCardId.StartsWith("pm_");
            bool stripeMetaDictHaveData = stripePaymentIntentMeta.Any();

            if (stripePaymentIntentId == null || !paymentIntentIdIsCorrectFormat)
            {
                throw new ArgumentException("Payment Intent Id exception, check if it is null or in the correct format");
            }
            if (stripePaymentIntentMeta == null || !stripeMetaDictHaveData)
            {
                throw new ArgumentException("Payment Intent Meta exception, check if it is null or have contain orderIdStr for the strategy to handle");
            }
            if (stripeCardId == null || !cardIdIsCorrectFormat)
            {
                throw new ArgumentException("Card Id exception, check if it is null or in the correct format");
            }
            if (customerId == null || !customerIdIsCorrectFormat)
            {
                throw new ArgumentException("Customer Id exception, check if it is null or in the correct format");
            }
            if (subTotal == null || subTotal <= 0)
            {
                throw new ArgumentException("Subtoal exception, check if it is null or in the valid value range");
            }
            if (discount == null || discount < 0)
            {
                throw new ArgumentException("Discount exception, check if it is null or in the valid value range");
            }

            return new AddPaymentTransactionRequestDto(
                stripePaymentIntentId: stripePaymentIntentId,
                stripePaymentIntentMeta: stripePaymentIntentMeta,
                stripeCardId: stripeCardId,
                momoOrderId: null, //**/
                customerId: customerId,
                subTotal: subTotal,
                discount: discount
            );
        }

        //public static AddPaymentTransactionRequestDto CreateMomoInstance(accept different parameter since the controller receive a different response) 

    }
}
