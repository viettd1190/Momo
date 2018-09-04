using System;
using System.Security.Cryptography;
using System.Text;
using NLog;

namespace vnyi.Momo.MomoUtils
{
    public class MoMoSecurity
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string getHash(string partnerCode,
                              string merchantRefId,
                              string amount,
                              string paymentCode,
                              string storeId,
                              string storeName,
                              string publicKey)
        {
            string json = "{\"partnerCode\":\"" + partnerCode + "\",\"partnerRefId\":\"" + merchantRefId + "\",\"amount\":" + amount + ",\"paymentCode\":\"" + paymentCode + "\",\"storeId\":\"" + storeId + "\",\"storeName\":\"" + storeName + "\"}";
            Logger.Debug("Raw hash: " + json);
            byte[] data = Encoding.UTF8.GetBytes(json);
            string result = null;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048)) // or 4096, base on key length
            {
                try
                {
                    // Client encrypting data with public key issued by server
                    // "publicKey" must be XML format, use https://superdry.apphb.com/tools/online-rsa-key-converter
                    // to convert from PEM to XML before hash
                    rsa.FromXmlString(publicKey);
                    byte[] encryptedData = rsa.Encrypt(data,
                                                       false);
                    string base64Encrypted = Convert.ToBase64String(encryptedData);
                    result = base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return result;
        }

        public string buildQueryHash(string partnerCode,
                                     string merchantRefId,
                                     string requestid,
                                     string publicKey)
        {
            string json = "{\"partnerCode\":\"" + partnerCode + "\",\"partnerRefId\":\"" + merchantRefId + "\",\"requestId\":\"" + requestid + "\"}";
            Logger.Debug("Raw hash: " + json);
            byte[] data = Encoding.UTF8.GetBytes(json);
            string result = null;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    // client encrypting data with public key issued by server
                    rsa.FromXmlString(publicKey);
                    byte[] encryptedData = rsa.Encrypt(data,
                                                       false);
                    string base64Encrypted = Convert.ToBase64String(encryptedData);
                    result = base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return result;
        }

        public string buildRefundHash(string partnerCode,
                                      string merchantRefId,
                                      string momoTranId,
                                      long amount,
                                      string description,
                                      string publicKey)
        {
            string json = "{\"partnerCode\":\"" + partnerCode + "\",\"partnerRefId\":\"" + merchantRefId + "\",\"momoTransId\":\"" + momoTranId + "\",\"amount\":" + amount + ",\"description\":\"" + description + "\"}";
            Logger.Debug("Raw hash: " + json);
            byte[] data = Encoding.UTF8.GetBytes(json);
            string result = null;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    // client encrypting data with public key issued by server
                    rsa.FromXmlString(publicKey);
                    byte[] encryptedData = rsa.Encrypt(data,
                                                       false);
                    string base64Encrypted = Convert.ToBase64String(encryptedData);
                    result = base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }

            return result;
        }

        public string signSHA256(string message,
                                 string key)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (HMACSHA256 hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage);
                hex = hex.Replace("-",
                                  "")
                         .ToLower();
                return hex;
            }
        }
    }
}
