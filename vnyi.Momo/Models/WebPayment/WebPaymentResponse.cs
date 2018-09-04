using Newtonsoft.Json;

namespace vnyi.Momo.Models.WebPayment
{
    public class WebPaymentResponse
    {
        [JsonProperty(PropertyName = "errorCode")]
        public int ErrorCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "localMessage")]
        public string LocalMessage { get; set; }

        [JsonProperty(PropertyName = "requestId")]
        public string RequestId { get; set; }

        [JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        [JsonProperty(PropertyName = "requestType")]
        public string RequestType { get; set; }

        [JsonProperty(PropertyName = "payUrl")]
        public string PayUrl { get; set; }

        [JsonProperty(PropertyName = "signature")]
        public string Signature { get; set; }

        [JsonProperty(PropertyName = "paymentCredit")]
        public string PaymentCredit { get; set; }

        [JsonProperty(PropertyName = "qrCodeUrl")]
        public string QrCodeUrl { get; set; }

        [JsonProperty(PropertyName = "deeplink")]
        public string Deeplink { get; set; }

        [JsonProperty(PropertyName = "deeplinkWebInApp")]
        public string DeeplinkWebInApp { get; set; }
    }
}
