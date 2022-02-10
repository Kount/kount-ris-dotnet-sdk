
namespace KountRisTest
{
    using Xunit;
    using Kount.Ris;
    using Microsoft.Extensions.Configuration;
    using System.Configuration;
    using System.IO;

    public class TokensTransformationTest
    {
        /// <summary>
        /// Payment Token
        /// </summary>
        private const string PTOK = "0007380568572514";    

      

        [Fact]
        public void TestMaskingCorrectUsage()
        {
            Request request = new Inquiry(true, TestHelper.GetConfiguration());

            request.SetCardPaymentMasked(PTOK);

            Assert.True("000738XXXXXX2514".Equals(request.GetParam("PTOK")), "Test failed! Masked token is wrong.");
            Assert.True("MASK".Equals(request.GetParam("PENC")), "Test failed! PENC param is wrong.");
            Assert.True("2514".Equals(request.GetParam("LAST4")), "Test failed! LAST4 param is wrong.");
        }

        [Fact]
        public void TestIncorrectMasking()
        {
            Inquiry request = new Inquiry(true, TestHelper.GetConfiguration());

            request.SetPayment(Kount.Enums.PaymentTypes.Card, "000738XXXXXX2514");

            var ptok = request.GetParam("PTOK");
            Assert.False("000738XXXXXX2514".Equals(ptok), "Test failed! Masked token is wrong.");
            Assert.False("MASK".Equals(request.GetParam("PENC")), "Test failed! PENC param is wrong.");
            Assert.True("2514".Equals(request.GetParam("LAST4")), "Test failed! LAST4 param is wrong.");
        }
    }
}
