using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace vnyi.Momo
{
    public static class RsaKeyConverter
    {
        public static string FormatPem(string pem,
                                       string keyType)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("-----BEGIN {0}-----\n",
                            keyType);

            int line = 1,
                width = 64;

            while ((line - 1) * width < pem.Length)
            {
                var startIndex = (line - 1) * width;
                var len = line * width > pem.Length
                                  ? pem.Length - startIndex
                                  : width;
                sb.AppendFormat("{0}\n",
                                pem.Substring(startIndex,
                                              len));
                line++;
            }

            sb.AppendFormat("-----END {0}-----\n",
                            keyType);
            return sb.ToString();
        }

        public static string PemToXml(string pem)
        {
            if(pem.StartsWith("-----BEGIN RSA PRIVATE KEY-----")
               || pem.StartsWith("-----BEGIN PRIVATE KEY-----"))
                return GetXmlRsaKey(pem,
                                    obj =>
                                    {
                                        if(obj as RsaPrivateCrtKeyParameters != null)
                                            return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters) obj);
                                        var keyPair = (AsymmetricCipherKeyPair) obj;
                                        return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters) keyPair.Private);
                                    },
                                    rsa => rsa.ToXmlString(true));

            if(pem.StartsWith("-----BEGIN PUBLIC KEY-----"))
                return GetXmlRsaKey(pem,
                                    obj =>
                                    {
                                        var publicKey = (RsaKeyParameters) obj;
                                        return DotNetUtilities.ToRSA(publicKey);
                                    },
                                    rsa => rsa.ToXmlString(false));

            throw new InvalidKeyException("Unsupported PEM format...");
        }

        private static string GetXmlRsaKey(string pem,
                                           Func<object, RSA> getRsa,
                                           Func<RSA, string> getKey)
        {
            using (var ms = new MemoryStream())
                using (var sw = new StreamWriter(ms))
                    using (var sr = new StreamReader(ms))
                    {
                        sw.Write(pem);
                        sw.Flush();
                        ms.Position = 0;
                        var pr = new PemReader(sr);
                        var keyPair = pr.ReadObject();
                        using (var rsa = getRsa(keyPair))
                        {
                            var xml = getKey(rsa);
                            return xml;
                        }
                    }
        }
    }
}
