using System;
using System.Diagnostics;
using System.Windows.Forms;
using Newtonsoft.Json;
using vnyi.Momo;
using vnyi.Momo.Models.WebPayment;

namespace vnyi.MomoTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender,
                                   EventArgs e)
        {
            string result = MomoHelper.SendPayOffline(txtPartnerId.Text,
                                                      Guid.NewGuid()
                                                          .ToString(),
                                                      txtAmount.Text,
                                                      txtPaymentCode.Text,
                                                      txtStoreId.Text,
                                                      txtStoreName.Text,
                                                      txtDescription.Text,
                                                      txtPublicKey.Text,
                                                      true);
            txtResult.Text = result;
        }

        private void btnOnlinePayment_Click(object sender,
                                            EventArgs e)
        {
            string result = MomoHelper.SendPayOnline(txtPartnerCode_Web.Text,
                                                     txtAccessKey_Web.Text,
                                                     txtSecretKey_Web.Text,
                                                     txtOrderInfo_Web.Text,
                                                     txtReturnUrl_Web.Text,
                                                     txtNotifyUrl_Web.Text,
                                                     txtAmount_Web.Text,
                                                     Guid.NewGuid()
                                                         .ToString(),
                                                     true);

            WebPaymentResult rawResult = JsonConvert.DeserializeObject<WebPaymentResult>(result);
            if(rawResult != null)
            {
                txtResult_Web.Text = rawResult.RawResponse;
                txtQrCodeUrl_Web.Text = rawResult.QrCodeImg;
                txtPayUrl_Web.Text = rawResult.PayUrl;

                if(!string.IsNullOrEmpty(rawResult.PayUrl))
                {
                    Process.Start(rawResult.PayUrl);
                }
            }
        }

        private void btnQuery_Click(object sender,
                                    EventArgs e)
        {
            string result = MomoHelper.QueryTransactionStatusPayOffline(txtPartnerCode_Query.Text,
                                                                        txtMerchantRefId_Query.Text,
                                                                        txtPublicKey_Query.Text,
                                                                        Guid.NewGuid()
                                                                            .ToString(),
                                                                        txtDescription_Query.Text,
                                                                        true);
            txtResult_Query.Text = result;
        }

        private void btnRefund_Click(object sender,
                                     EventArgs e)
        {
            string result = MomoHelper.RefundTransactionPayOffline(txtPartnerCode_Refund.Text,
                                                                   Guid.NewGuid()
                                                                       .ToString(),
                                                                   txtMerchantRefId_Refund.Text,
                                                                   txtAmount_Refund.Text,
                                                                   txtMomoTransId_Refund.Text,
                                                                   txtDescription_Refund.Text,
                                                                   txtPublicKey_Refund.Text,
                                                                   true);
            txtResult_Refund.Text = result;
        }

        private void btnSend_QueryOnl_Click(object sender,
                                            EventArgs e)
        {
            string result = MomoHelper.QueryTransactionStatusPayOnline(txtPartnerCode_QueryOnl.Text,
                                                                       txtAccessKey_QueryOnl.Text,
                                                                       txtSecretKey_QueryOnl.Text,
                                                                       txtOrderId_QueryOnl.Text,
                                                                       true);
            txtResult_QueryOnl.Text = result;
        }

        private void btnRefundOnl_Click(object sender,
                                        EventArgs e)
        {
            string result = MomoHelper.RefundTransactionPayOnline(txtPartnerCode_RefundOnl.Text,
                                                                  txtAccessKey_RefundOnl.Text,
                                                                  txtSecretKey_RefundOnl.Text,
                                                                  txtAmount_RefundOnl.Text,
                                                                  txtTransId_RefundOnl.Text,
                                                                  txtOrderId_RefundOnl.Text,
                                                                  true);
            txtResult_RefundOnl.Text = result;
        }

        private void btnQueryRefundOnl_Click(object sender, EventArgs e)
        {
            string result = MomoHelper.QueryRefundTransactionStatusOnline(txtPartnerCode_QueryRefundOnl.Text,
                                                                          txtAccessKey_QueryRefundOnl.Text,
                                                                          txtSecretKey_QueryRefundOnl.Text,
                                                                          txtOrderId_QueryRefundOnl.Text,
                                                                          true);
            txtResult_QueryRefundOnl.Text = result;
        }
    }
}
