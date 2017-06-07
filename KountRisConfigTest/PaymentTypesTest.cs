namespace KountRisConfigTest
{
    using Kount.Ris;
    using Kount.Enums;
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

            inquiry.SetPayment(PaymentTypes.Token, TOKEN_ID_1);

            Assert.IsTrue("TOKEN".Equals(inquiry.GetParam("PTYP")), "Test failed! Payment type is wrong.");
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

            inquiry.SetPayment(PaymentTypes.Token, TOKEN_ID_2);

            Assert.IsTrue("TOKEN".Equals(inquiry.GetParam("PTYP")), "Test failed! Payment type is wrong.");
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

            inquiry.SetPayment(PaymentTypes.CarteBleue, CARTE_BLEU);

            Assert.IsTrue("CARTE_BLEUE".Equals(inquiry.GetParam("PTYP")), "Test failed! Payment type is wrong.");
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

            inquiry.SetPayment(PaymentTypes.Skrill, SKRILL_ID);

            Assert.IsTrue("SKRILL".Equals(inquiry.GetParam("PTYP")), "Test failed! Payment type is wrong.");
            Assert.IsTrue("XYZ1230L2VYV3P815Q2I".Equals(inquiry.GetParam("PTOK")), "Test failed! Hash token is wrong.");
            Assert.IsTrue("KHASH".Equals(inquiry.GetParam("PENC")), "Test failed! PENC param is wrong.");
        }
    }
}