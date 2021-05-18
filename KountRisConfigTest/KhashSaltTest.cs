
namespace KountRisConfigTest
{
    using Kount.Ris;
    using Xunit;



    /// <summary>
    /// Khash Salt Test samples
    /// <b>MerchantId:</b> 999666
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 0700 <br/>
    /// <b>Copyright:</b> 2019 Kount Inc. All Rights Reserved<br/>
    /// </summary> 



    public class KhashSaltTest
    {
        private const string CARD_NUM_1 = "4111111111111111";
        private const string CARD_NUM_2 = "5199185454061655";
        private const string CARD_NUM_3 = "4259344583883";

        private const int MERCHANT_ID = 666666;

        /// <summary>
        /// <b>TEST 1</b>
        /// raw input 4111111111111111,
        /// expected KHASH 411111WMS5YA6FUZA1KC
        /// </summary>
       
        [Fact]
        public void TestKhashCreditCard1()
        {
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_1, out _sid, out _orderNum);

            var hashCardNum = inquiry.GetParam("PTOK");

            Assert.True("411111WMS5YA6FUZA1KC".Equals(hashCardNum), $"Wrong result: {hashCardNum}. Expected KHASH 411111WMS5YA6FUZA1KC.");

        }

        /// <summary>
        /// <b>TEST 2</b>
        /// raw input 5199185454061655,
        /// expected KHASH 5199182NOQRXNKTTFL11
        /// </summary>

        [Fact]
        public void TestKhashCreditCard2()
        {
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_2, out _sid, out _orderNum);

            var hashCardNum = inquiry.GetParam("PTOK");

            Assert.True("5199182NOQRXNKTTFL11".Equals(hashCardNum), $"Wrong result: {hashCardNum}. Expected KHASH 5199182NOQRXNKTTFL11.");
        }

        /// <summary>
        /// <b>TEST 3</b>
        /// raw input 4259344583883,
        /// expected KHASH 425934FEXQI1QS6TH2O5
        /// </summary>

        [Fact]
        public void TestKhashCreditCard3()
        {
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_3, out _sid, out _orderNum);

            var hashCardNum = inquiry.GetParam("PTOK");

            Assert.True("425934FEXQI1QS6TH2O5".Equals(hashCardNum), $"Wrong result: {hashCardNum}. Expected KHASH 425934FEXQI1QS6TH2O5.");
        }

        /// <summary>
        /// <b>TEST 4</b>
        /// raw input 4111111111111111,
        /// expected KHASH 666666WMS5YA6FUZA1KC
        /// </summary>
        [Fact]
        public void TestKhashGiftCard1()
        {
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_1, out _sid, out _orderNum);

            inquiry.SetMerchantId(MERCHANT_ID);

            inquiry.SetPayment(Kount.Enums.PaymentTypes.GiftCard, CARD_NUM_1);

            var hashCardNum = inquiry.GetParam("PTOK");

            Assert.True("666666WMS5YA6FUZA1KC".Equals(hashCardNum), $"Wrong result: {hashCardNum}. Expected KHASH 666666WMS5YA6FUZA1KC.");
        }

        /// <summary>
        /// <b>TEST 5</b>
        /// raw input 5199185454061655,
        /// expected KHASH 6666662NOQRXNKTTFL11
        /// </summary>
        [Fact]
        public void TestKhashGiftCard2()
        {
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_2, out _sid, out _orderNum);

            inquiry.SetMerchantId(MERCHANT_ID);

            inquiry.SetPayment(Kount.Enums.PaymentTypes.GiftCard, CARD_NUM_2);

            var hashCardNum = inquiry.GetParam("PTOK");

            Assert.True("6666662NOQRXNKTTFL11".Equals(hashCardNum), $"Wrong result: {hashCardNum}. Expected KHASH 6666662NOQRXNKTTFL11.");
        }

        /// <summary>
        /// <b>TEST 6</b>
        /// raw input 4259344583883,
        /// expected KHASH 666666FEXQI1QS6TH2O5
        /// </summary>
        [Fact]
        public void TestKhashGiftCard3()
        {
            string _sid = null;
            string _orderNum = null;

            Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_3, out _sid, out _orderNum);

            inquiry.SetMerchantId(MERCHANT_ID);

            inquiry.SetPayment(Kount.Enums.PaymentTypes.GiftCard, CARD_NUM_3);

            var hashCardNum = inquiry.GetParam("PTOK");

            Assert.True("666666FEXQI1QS6TH2O5".Equals(hashCardNum), $"Wrong result: {hashCardNum}. Expected KHASH 666666FEXQI1QS6TH2O5.");
        }



    }
}
