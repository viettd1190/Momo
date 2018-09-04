using System.Collections.Generic;
using Newtonsoft.Json;

namespace vnyi.Momo.Models.WebPayment
{
    public class ErrorWebPaymentResponse
    {
        [JsonProperty(PropertyName = "errorCode")]
        public int ErrorCode { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "localMessage")]
        public string LocalMessage { get; set; }

        [JsonProperty(PropertyName = "details")]
        public List<ErrorWebResponseDetail> Details { get; set; }
    }

    public class ErrorWebResponseDetail
    {
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
