

using KountRisConfigTest;

namespace PaymentsFraudTest
{
    using Kount.Ris;
    using Xunit;
    using System;
    using System.Collections;

    
    public class PFIntegrationTests
    {
        [Fact]
        public void HappyPath()
        {
            Inquiry inquiry = TestHelper.CreateInquiry("4111111111111111", out string _sid, out string _orderNum);
           
            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("SPORTING_GOODS", "SG999999", "3000 CANDLEPOWER PLASMA FLASHLIGHT", 2, 68990));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse(true);

            var errors = response.GetErrors();
            Assert.True(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var warnings = response.GetWarnings();
            Assert.True(warnings.Count == 0, String.Join(Environment.NewLine, warnings, "There are warnings in response!"));

            var sid = response.GetSessionId();
            Assert.True(_sid.Equals(sid), "Inquiry failed! Wrong session ID");

            var orderNum = response.GetOrderNumber();
            Assert.True(_orderNum.Equals(orderNum), "Inquiry failed! Wrong order number.");
        }
    }
}