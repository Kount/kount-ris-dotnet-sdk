namespace KountRisConfigTest
{
    using Kount.Ris;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PaymentTypesTest
    {
        private const string TOKEN_ID_1 = "6011476613608633";
        private const string TOKEN_ID_2 = "1A2B3C6613608633";
        private const string CARTE_BLEU = "AABBCC661360DDD";
        private const string SKRILL_ID = "XYZ123661360SKMB";

        [TestMethod]
        public void TestTokenPayment()
        {
            //Request request = new Inquiry(false);
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(TOKEN_ID_1, out _sid, out _orderNum);

            inquiry.SetTokenPayment(TOKEN_ID_1);

            Assert.IsTrue(PaymentTypes.TokenType.Equals(inquiry.GetParam("PTYP")), "Test failed! Payment type is wrong.");
            Assert.IsTrue("601147IF86FKXJTM5K8Z".Equals(inquiry.GetParam("PTOK")), "Test failed! Hash token is wrong.");
            Assert.IsTrue("KHASH".Equals(inquiry.GetParam("PENC")), "Test failed! PENC param is wrong.");
        }

        [TestMethod]
        public void TestToken2Payment()
        {
            //Request request = new Inquiry(false);
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(TOKEN_ID_2, out _sid, out _orderNum);

            inquiry.SetTokenPayment(TOKEN_ID_2);

            Assert.IsTrue(PaymentTypes.TokenType.Equals(inquiry.GetParam("PTYP")), "Test failed! Payment type is wrong.");
            Assert.IsTrue("1A2B3C6SYWXNDI5GN77V".Equals(inquiry.GetParam("PTOK")), "Test failed! Hash token is wrong.");
            Assert.IsTrue("KHASH".Equals(inquiry.GetParam("PENC")), "Test failed! PENC param is wrong.");
        }

        [TestMethod]
        public void TestCarteBleuPayment()
        {
            //Request request = new Inquiry(false);
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(CARTE_BLEU, out _sid, out _orderNum);

            inquiry.SetCarteBleuePayment(CARTE_BLEU);

            Assert.IsTrue(PaymentTypes.CarteBleueType.Equals(inquiry.GetParam("PTYP")), "Test failed! Payment type is wrong.");
            Assert.IsTrue("AABBCCG297U47WC6J0BC".Equals(inquiry.GetParam("PTOK")), "Test failed! Hash token is wrong.");
            Assert.IsTrue("KHASH".Equals(inquiry.GetParam("PENC")), "Test failed! PENC param is wrong.");
        }

        [TestMethod]
        public void TestScrillPayment()
        {
            //Request request = new Inquiry(false);
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(SKRILL_ID, out _sid, out _orderNum);

            inquiry.SetSkrillPayment(SKRILL_ID);

            Assert.IsTrue(PaymentTypes.SkrillType.Equals(inquiry.GetParam("PTYP")), "Test failed! Payment type is wrong.");
            Assert.IsTrue("XYZ1230L2VYV3P815Q2I".Equals(inquiry.GetParam("PTOK")), "Test failed! Hash token is wrong.");
            Assert.IsTrue("KHASH".Equals(inquiry.GetParam("PENC")), "Test failed! PENC param is wrong.");
        }
    }
}