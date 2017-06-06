namespace KountRisTest
{
    using Kount.Ris;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TokensTransformationTest
    {
        /// <summary>
        /// Payment Token
        /// </summary>
        private const string PTOK = "0007380568572514";

        /// <summary>
        ///
        /// </summary>
        [TestMethod]
        public void TestMaskingCorrectUsage()
        {
            Request request = new Inquiry(false);

            request.SetCardPaymentMasked(PTOK);

            Assert.IsTrue("000738XXXXXX2514".Equals(request.GetParam("PTOK")), "Test failed! Masked token is wrong.");
            Assert.IsTrue("MASK".Equals(request.GetParam("PENC")), "Test failed! PENC param is wrong.");
            Assert.IsTrue("2514".Equals(request.GetParam("LAST4")), "Test failed! LAST4 param is wrong.");
        }

        [TestMethod]
        public void TestIncorrectMasking()
        {
            Inquiry request = new Inquiry(false);

            request.SetPayment(Kount.Enums.PaymentTypes.Card, "000738XXXXXX2514");

            var ptok = request.GetParam("PTOK");
            Assert.IsFalse("000738XXXXXX2514".Equals(ptok), "Test failed! Masked token is wrong.");
            Assert.IsFalse("MASK".Equals(request.GetParam("PENC")), "Test failed! PENC param is wrong.");
            Assert.IsTrue("2514".Equals(request.GetParam("LAST4")), "Test failed! LAST4 param is wrong.");
        }
    }
}