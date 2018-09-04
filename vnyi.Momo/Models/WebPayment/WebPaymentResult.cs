namespace vnyi.Momo.Models.WebPayment
{
    public class WebPaymentResult
    {
        public string RawResponse { get; set; }

        public string QrCodeUrl { get; set; }

        public string PayUrl { get; set; }

        public string QrCodeImg { get; set; }
    }
}
