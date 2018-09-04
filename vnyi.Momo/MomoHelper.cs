using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using vnyi.Momo.Models.WebPayment;
using vnyi.Momo.MomoUtils;

namespace vnyi.Momo
{
    public static class MomoHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Thanh toan momo offline bang cach quet ma vach tren app momo
        /// </summary>
        /// <param name="partnerCode">Ma doi tac do momo cung cap</param>
        /// <param name="merchantRefId">Ma don hang luu tren he thong momo</param>
        /// <param name="amount">So tien can thanh toan</param>
        /// <param name="paymentCode">Ma so quet tren app momo cua khach hang</param>
        /// <param name="storeId">Store id cua hang</param>
        /// <param name="storeName">Store name cua hang</param>
        /// <param name="description">Thong tin don hang</param>
        /// <param name="publicKey">Public key do momo cung cap</param>
        /// <param name="test">Test=true la moi truong test, test=false la moi truong production. Mac dinh la false</param>
        /// <returns></returns>
        public static string SendPayOffline(string partnerCode,
                                            string merchantRefId,
                                            string amount,
                                            string paymentCode,
                                            string storeId,
                                            string storeName,
                                            string description,
                                            string publicKey,
                                            bool test = false)
        {
            try
            {
                var endpoint = test
                                       ? "https://test-payment.momo.vn/pay/pos"
                                       : "https://payment.momo.vn/pay/pos";
                var version = CommonConstant.MOMO_VERSION;

                publicKey = RsaKeyConverter.PemToXml(RsaKeyConverter.FormatPem(publicKey,
                                                                               "PUBLIC KEY"));

                //get hash
                var momoCrypto = new MoMoSecurity();
                var hash = momoCrypto.getHash(partnerCode,
                                              merchantRefId,
                                              amount,
                                              paymentCode,
                                              storeId,
                                              storeName,
                                              publicKey);

                //request to MoMo
                var jsonRequest = "{\"partnerCode\":\"" + partnerCode + "\",\"partnerRefId\":\"" + merchantRefId + "\",\"description\":\"" + description + "\",\"amount\":" + amount + ",\"version\":" + version + ",\"hash\":\"" + hash + "\"}";
                Logger.Debug(jsonRequest);

                //response from MoMo
                var responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint,
                                                                         jsonRequest);
                Logger.Debug(responseFromMomo);

                return responseFromMomo;
            }
            catch (Exception exception)
            {
                Logger.Debug(exception);
            }

            return string.Empty;
        }

        /// <summary>
        ///     Kiem tra trang thai giao dich offline
        /// </summary>
        /// <param name="partnerCode">Ma doi tac do momo cung cap</param>
        /// <param name="merchantRefId">Ma don hang luu tren he thong momo</param>
        /// <param name="publicKey">Public key do momo cung cap</param>
        /// <param name="requestId">Request id</param>
        /// <param name="description"></param>
        /// <param name="test">Test=true la moi truong test, test=false la moi truong production. Mac dinh la false</param>
        /// <returns></returns>
        public static string QueryTransactionStatusPayOffline(string partnerCode,
                                                              string merchantRefId,
                                                              string publicKey,
                                                              string requestId,
                                                              string description,
                                                              bool test = false)
        {
            string endpoint = test
                                      ? "https://test-payment.momo.vn/pay/query-status"
                                      : "https://payment.momo.vn/pay/query-status";
            string version = CommonConstant.MOMO_VERSION;

            publicKey = RsaKeyConverter.PemToXml(RsaKeyConverter.FormatPem(publicKey,
                                                                           "PUBLIC KEY"));

            //get hash
            MoMoSecurity momoCrypto = new MoMoSecurity();
            string hash = momoCrypto.buildQueryHash(partnerCode,
                                                    merchantRefId,
                                                    requestId,
                                                    publicKey);

            //request to MoMo
            string jsonRequest = "{\"partnerCode\":\"" + partnerCode + "\",\"partnerRefId\":\"" + merchantRefId + "\",\"description\":\"" + description + "\",\"version\":" + version + ",\"hash\":\"" + hash + "\"}";

            //response from MoMo
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint,
                                                                        jsonRequest);
            Logger.Debug(responseFromMomo);

            return responseFromMomo;
        }

        /// <summary>
        ///     Refund tien cho giao dich thanh toan offline
        /// </summary>
        /// <param name="partnerCode">Ma doi tac do momo cung cap</param>
        /// <param name="requestId">Request id</param>
        /// <param name="merchantRefId">Ma don hang luu tren he thong momo</param>
        /// <param name="amount">So tien can refund</param>
        /// <param name="momoTransId">Ma giao dich momo</param>
        /// <param name="description"></param>
        /// <param name="publicKey">Public key do momo cung cap</param>
        /// <param name="test">Test=true la moi truong test, test=false la moi truong production. Mac dinh la false</param>
        /// <returns></returns>
        public static string RefundTransactionPayOffline(string partnerCode,
                                                         string requestId,
                                                         string merchantRefId,
                                                         string amount,
                                                         string momoTransId,
                                                         string description,
                                                         string publicKey,
                                                         bool test = false)
        {
            string endpoint = test
                                      ? "https://test-payment.momo.vn/pay/refund"
                                      : "https://payment.momo.vn/pay/refund";
            string version = CommonConstant.MOMO_VERSION;

            publicKey = RsaKeyConverter.PemToXml(RsaKeyConverter.FormatPem(publicKey,
                                                                           "PUBLIC KEY"));

            //get hash
            MoMoSecurity momoCrypto = new MoMoSecurity();
            string hash = momoCrypto.buildRefundHash(partnerCode,
                                                     merchantRefId,
                                                     momoTransId,
                                                     Convert.ToInt64(amount),
                                                     description,
                                                     publicKey);

            //request to MoMo
            string jsonRequest = "{\"partnerCode\":\"" + partnerCode + "\",\"requestId\":\"" + requestId + "\",\"version\":" + version + ",\"hash\":\"" + hash + "\"}";

            //response from MoMo
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint,
                                                                        jsonRequest);
            Logger.Debug(responseFromMomo);

            return responseFromMomo;
        }

        /// <summary>
        ///     Thanh toan momo online tich hop tren website
        /// </summary>
        /// <param name="partnerCode">Ma doi tac do momo cung cap</param>
        /// <param name="accessKey">Access key do momo cung cap</param>
        /// <param name="secretKey">Secret key do momo cung cap</param>
        /// <param name="orderInfo">Thong tin don hang</param>
        /// <param name="returnUrl">Url tra ve khi thanh toan thanh cong</param>
        /// <param name="notifyUrl">Url post de nhan thong tin thanh toan thanh cong</param>
        /// <param name="amount">So tien can thanh toan</param>
        /// <param name="orderId">Order id</param>
        /// <param name="test">Test=true la moi truong test, test=false la moi truong production. Mac dinh la false</param>
        /// <returns></returns>
        public static string SendPayOnline(string partnerCode,
                                           string accessKey,
                                           string secretKey,
                                           string orderInfo,
                                           string returnUrl,
                                           string notifyUrl,
                                           string amount,
                                           string orderId,
                                           bool test = false)
        {
            var result = new WebPaymentResult
                         {
                                 RawResponse = string.Empty,
                                 QrCodeUrl = string.Empty,
                                 PayUrl = string.Empty
                         };

            try
            {
                string requestType = "captureMoMoWallet";
                string requestId = Guid.NewGuid()
                                       .ToString();

                Dictionary<string, string> obj = new Dictionary<string, string>
                                                 {
                                                         ["partnerCode"] = partnerCode,
                                                         ["accessKey"] = accessKey,
                                                         ["requestId"] = requestId,
                                                         ["amount"] = amount,
                                                         ["orderId"] = orderId,
                                                         ["orderInfo"] = orderInfo,
                                                         ["returnUrl"] = returnUrl,
                                                         ["notifyUrl"] = notifyUrl,
                                                         ["extraData"] = ""
                                                 };

                var responseFromMomo = TransactionOnline(obj,
                                                         secretKey,
                                                         test,
                                                         requestType);

                result.RawResponse = responseFromMomo;

                if(string.Equals(responseFromMomo,
                                 "The remote server returned an error: (404) Not Found."))
                {
                    result.QrCodeUrl = "Sai thông tin cấu hình endpoint";
                }
                else
                {
                    var response = JsonConvert.DeserializeObject<WebPaymentResponse>(responseFromMomo);

                    if(response != null)
                    {
                        Logger.Debug("Return from MoMo: " + responseFromMomo);

                        if(!string.IsNullOrEmpty(response.QrCodeUrl))
                        {
                            result.QrCodeUrl = response.QrCodeUrl;
                            result.PayUrl = response.PayUrl;

                            //get qr code image
                            try
                            {
                                WebClient client = new WebClient();
                                client.Headers.Add("user-agent",
                                                   "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
                                string htmlCode = client.DownloadString(result.PayUrl);
                                string matchString = Regex.Match(htmlCode,
                                                                 "<img class=\"qr-code\".+?src=[\"'](.+?)[\"'].+?>",
                                                                 RegexOptions.IgnoreCase)
                                                          .Groups[1]
                                                          .Value;
                                result.QrCodeImg = matchString;
                            }
                            catch (Exception exception)
                            {
                                Logger.Debug(exception);
                            }
                        }
                        else
                        {
                            result.QrCodeUrl = response.LocalMessage;
                        }
                    }
                    else
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorWebPaymentResponse>(responseFromMomo);
                        if(errorResponse != null) result.QrCodeUrl = errorResponse.LocalMessage;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Debug(exception);
            }

            return JsonConvert.SerializeObject(result,
                                               Formatting.None);
        }

        /// <summary>
        ///     Kiem tra trang thai giao dich online
        /// </summary>
        /// <param name="partnerCode">Ma doi tac do momo cung cap</param>
        /// <param name="accessKey">Access key do momo cung cap</param>
        /// <param name="secretKey">Secret key do momo cung cap</param>
        /// <param name="orderId">Order id</param>
        /// <param name="test">Test=true la moi truong test, test=false la moi truong production. Mac dinh la false</param>
        /// <returns></returns>
        public static string QueryTransactionStatusPayOnline(string partnerCode,
                                                             string accessKey,
                                                             string secretKey,
                                                             string orderId,
                                                             bool test = false)
        {
            string requestType = "transactionStatus";
            string requestId = Guid.NewGuid()
                                   .ToString();

            Dictionary<string, string> obj = new Dictionary<string, string>
                                             {
                                                     ["partnerCode"] = partnerCode,
                                                     ["accessKey"] = accessKey,
                                                     ["requestId"] = requestId,
                                                     ["orderId"] = orderId,
                                                     ["requestType"] = requestType
                                             };

            return TransactionOnline(obj,
                                     secretKey,
                                     test);
        }

        /// <summary>
        ///     Refund tien cho giao dich thanh toan online
        /// </summary>
        /// <param name="partnerCode">Ma doi tac do momo cung cap</param>
        /// <param name="accessKey">Access key do momo cung cap</param>
        /// <param name="secretKey">Secret key do momo cung cap</param>
        /// <param name="amount">So tien can refund</param>
        /// <param name="transId">Ma giao dich momo</param>
        /// <param name="orderId">Order id</param>
        /// <param name="test">Test=true la moi truong test, test=false la moi truong production. Mac dinh la false</param>
        /// <returns></returns>
        public static string RefundTransactionPayOnline(string partnerCode,
                                                        string accessKey,
                                                        string secretKey,
                                                        string amount,
                                                        string transId,
                                                        string orderId,
                                                        bool test = false)
        {
            string requestType = "refundMoMoWallet";
            string requestId = Guid.NewGuid()
                                   .ToString();

            Dictionary<string, string> obj = new Dictionary<string, string>
                                             {
                                                     ["partnerCode"] = partnerCode,
                                                     ["accessKey"] = accessKey,
                                                     ["requestId"] = requestId,
                                                     ["amount"] = amount,
                                                     ["orderId"] = orderId,
                                                     ["transId"] = transId,
                                                     ["requestType"] = requestType
                                             };

            return TransactionOnline(obj,
                                     secretKey,
                                     test);
        }

        /// <summary>
        ///     Kiem tra trang thai refund tien online
        /// </summary>
        /// <param name="partnerCode">Ma doi tac do momo cung cap</param>
        /// <param name="accessKey">Access key do momo cung cap</param>
        /// <param name="secretKey">Secret key do momo cung cap</param>
        /// <param name="orderId">Order id</param>
        /// <param name="test">Test=true la moi truong test, test=false la moi truong production. Mac dinh la false</param>
        /// <returns></returns>
        public static string QueryRefundTransactionStatusOnline(string partnerCode,
                                                                string accessKey,
                                                                string secretKey,
                                                                string orderId,
                                                                bool test = false)
        {
            string requestType = "refundStatus";
            string requestId = Guid.NewGuid()
                                   .ToString();

            Dictionary<string, string> obj = new Dictionary<string, string>
                                             {
                                                     ["partnerCode"] = partnerCode,
                                                     ["accessKey"] = accessKey,
                                                     ["requestId"] = requestId,
                                                     ["orderId"] = orderId,
                                                     ["requestType"] = requestType
                                             };

            return TransactionOnline(obj,
                                     secretKey,
                                     test);
        }

        private static string TransactionOnline(Dictionary<string, string> obj,
                                                string secretKey,
                                                bool test,
                                                string requestType = "")
        {
            string endpoint = test
                                      ? "https://test-payment.momo.vn/gw_payment/transactionProcessor"
                                      : "https://payment.momo.vn/gw_payment/transactionProcessor";

            StringBuilder builder = new StringBuilder();
            string condition = string.Empty;

            foreach (var item in obj)
            {
                builder.AppendFormat("{0}{1}={2}",
                                     condition,
                                     item.Key,
                                     item.Value);
                condition = "&";
            }

            var rawHash = builder.ToString();

            var crypto = new MoMoSecurity();
            //sign signature SHA256
            var signature = crypto.signSHA256(rawHash,
                                              secretKey);

            //build body json request
            var message = new JObject();
            foreach (var item in obj)
            {
                message.Add(item.Key,
                            item.Value);
            }

            if(!string.IsNullOrEmpty(requestType))
            {
                message.Add("requestType",
                            requestType);
                
            }

            message.Add("signature",
                        signature);
            Logger.Debug("Json request to MoMo: " + message);
            var responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint,
                                                                     message.ToString());

            return responseFromMomo;
        }
    }
}
