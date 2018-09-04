using System.Web.Mvc;
using Newtonsoft.Json;
using NLog;

namespace vnyi.MomoTestWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ActionResult Index(string partnerCode,
                                  string accessKey,
                                  string requestId,
                                  string amount,
                                  string orderId,
                                  string orderInfo,
                                  string orderType,
                                  string transid,
                                  string message,
                                  string localMessage,
                                  string responseTime,
                                  string errorCode,
                                  string payType,
                                  string extraData,
                                  string signature)
        {
            if(!string.IsNullOrEmpty(transid))
            {
                var obj = new
                          {
                                  partnerCode = Request["partnerCode"],
                                  accessKey = Request["accessKey"],
                                  requestId = Request["requestId"],
                                  amount = Request["amount"],
                                  orderId = Request["orderId"],
                                  orderInfo = Request["orderInfo"],
                                  orderType = Request["orderType"],
                                  transid = Request["transid"],
                                  message = Request["message"],
                                  localMessage = Request["localMessage"],
                                  responseTime = Request["responseTime"],
                                  errorCode = Request["errorCode"],
                                  payType = Request["payType"],
                                  extraData = Request["extraData"],
                                  signature = Request["signature"]
                          };

                Logger.Debug(JsonConvert.SerializeObject(obj,
                                                         Formatting.None));

                return RedirectToAction("About");
            }
                
            return View();
        }

        [HttpPost]
        public ActionResult NotifyUrl()
        {
            var obj = new
                      {
                              partnerCode = Request["partnerCode"],
                              accessKey = Request["accessKey"],
                              requestId = Request["requestId"],
                              amount = Request["amount"],
                              orderId = Request["orderId"],
                              orderInfo = Request["orderInfo"],
                              orderType = Request["orderType"],
                              transid = Request["transid"],
                              message = Request["message"],
                              localMessage = Request["localMessage"],
                              responseTime = Request["responseTime"],
                              errorCode = Request["errorCode"],
                              payType = Request["payType"],
                              extraData = Request["extraData"],
                              signature = Request["signature"]
                      };

            Logger.Debug(JsonConvert.SerializeObject(obj,
                                                     Formatting.None));

            return Json(obj,
                        "json");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Thanh toán thành công qua ví điện tử MOMO";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
