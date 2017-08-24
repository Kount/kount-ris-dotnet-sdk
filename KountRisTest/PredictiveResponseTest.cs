//-----------------------------------------------------------------------
// <copyright file="PredictiveResponseTest.cs" company="Kount Inc">
//     Copyright Kount Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace KountRisTest
{
    using Kount.Enums;
    using Kount.Ris;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections;
    using System.Configuration;

    /// <summary>
    /// Those tests ensuring the customer's credentials are valid,
    /// but doesn't depend on a RuleSet being in a specific state.
    /// Developer can set credentials in <c>App.Config</c><br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 0700 <br/>
    /// <b>Copyright:</b> 2017 Kount Inc. All Rights Reserved.<br/>
    /// </summary>
    [TestClass]
    public class PredictiveResponseTest
    {
        private const string API_KEY = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI5OTk2NjciLCJhdWQiOiJLb3VudC4xIiwiaWF0IjoxNDk0NTM1OTE2LCJzY3AiOnsia2EiOm51bGwsImtjIjpudWxsLCJhcGkiOmZhbHNlLCJyaXMiOnRydWV9fQ.KK3zG4dMIhTIaE5SeCbej1OAFhZifyBswMPyYFAVRrM";
        private const int MERCHANT_ID = 999667;

        /// <summary>
        /// Email of Merchant
        /// </summary>
        private const string EMAL = "predictive@kount.com";

        /// <summary>
        /// Name of Web site
        /// </summary>
        private const string SITE = "DEFAULT";

        /// <summary>
        /// Country of currency submitted on order
        /// </summary>
        private const string CURR = "USD";

        /// <summary>
        /// Merchants acknowledgement to ship/process the order
        /// </summary>
        private const char MACK = 'Y';

        /// <summary>
        /// Authorization Status returned to merchant from processor. 
        /// </summary>
        private const char AUTH = 'A';

        /// <summary>
        /// Dotted Decimal IPv4 address that the merchant sees coming from the customer.
        /// </summary>
        private const string IPAD = "131.206.45.21";

        /// <summary>
        /// Total amount in currency submitted in lowest currency factor
        /// </summary>
        private const int TOTL = 123456;

        /// <summary>
        /// Total cash amount in currency submitted
        /// </summary>
        private const int CASH = 4444;

        private string _sid;
        private string _orderNum;

        private Inquiry CreateInquiry()
        {
            Inquiry inquiry = new Inquiry(false);
            _sid = Guid.NewGuid().ToString().Replace("-", String.Empty);
            inquiry.SetSessionId(_sid);

            //var uniq = Guid.NewGuid().ToString().Replace("-", String.Empty);
            var uniq = _sid.Substring(0, 20); //Fist 20 chars from sid

            _orderNum = uniq.Substring(0, 10);
            inquiry.SetUnique(uniq);
            inquiry.SetOrderNumber(_orderNum);

            string apiKey = ConfigurationManager.AppSettings["Ris.API.Key"];
            if (String.IsNullOrEmpty(apiKey))
            {
                inquiry.SetMerchantId(MERCHANT_ID); // 999667
                inquiry.SetApiKey(API_KEY);
            }

            inquiry.SetEmail(EMAL);
            inquiry.SetMode(InquiryTypes.ModeQ);

            inquiry.SetNoPayment();

            inquiry.SetTotal(TOTL);
            inquiry.SetIpAddress(IPAD);

            inquiry.SetMack(MACK);

            inquiry.SetWebsite(SITE);
            inquiry.SetCurrency(CURR);

            return inquiry;
        }

        /// <summary>
        /// TEST 1
        /// Expected Score=42
        /// Input to predictive response, UDF[~K!_SCOR] = 42
        /// Produces RIS output, SCOR=42
        /// Expected Decision = R
        /// Input to predictive response, UDF[~K!_AUTO] = R
        /// Produces RIS output, AUTO=R
        /// Email input will need to be, EMAL=predictive @kount.com
        /// </summary>
        [TestMethod]
        public void PredictiveResponseScore42AutoR()
        {
            Inquiry inquiry = CreateInquiry();

            inquiry.SetUserDefinedField("~K!_SCOR", "42");
            inquiry.SetUserDefinedField("~K!_AUTO", "R");

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1000));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();
            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var score = response.GetScore();
            Assert.IsTrue("42".Equals(score), "Inquiry failed!   Expected Score=42.");

            var auto = response.GetAuto();
            Assert.IsTrue("R".Equals(auto), "Inquiry failed!  Expected Decision=R");
        }

        /// <summary>
        /// TEST 2
        /// Expected Score=42
        /// Input to predictive response, UDF[~K!_SCOR] = 42
        /// Produces RIS output, SCOR=42
        /// Expected Decision = D
        /// Input to predictive response, UDF[~K!_AUTO] = D
        /// Produces RIS output, AUTO=D
        /// UDF[~K!_GEOX]=NG
        /// Email input will need to be, EMAL=predictive @kount.com
        /// </summary>
        [TestMethod]
        public void PredictiveResponseScore42AutoD()
        {
            Inquiry inquiry = CreateInquiry();

            inquiry.SetUserDefinedField("~K!_SCOR", "42");
            inquiry.SetUserDefinedField("~K!_AUTO", "D");
            inquiry.SetUserDefinedField("~K!_GEOX", "NG");

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1000));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();
            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var score = response.GetScore();
            Assert.IsTrue("42".Equals(score), "Inquiry failed!   Expected Score=42.");

            var auto = response.GetAuto();
            Assert.IsTrue("D".Equals(auto), "Inquiry failed!  Expected Decision=D");

            var geox = response.GetGeox();
            Assert.IsTrue("NG".Equals(geox), "Inquiry failed!  Expected GEOX=NG");
        }

        /// <summary>
        /// TEST 3
        /// Expected Score=18
        /// Input to predictive response, UDF[~K!_SCOR] = 18
        /// Produces RIS output, MODE=E
        /// ERRO=601
        /// Email input will need to be, EMAL=predictive @kount.com
        /// </summary>
        [TestMethod]
        public void PredictiveResponseScore18ModeE()
        {
            Inquiry inquiry = CreateInquiry();

            inquiry.SetUserDefinedField("~K!_SCOR", "18");
            inquiry.SetUserDefinedField("~K!_AUTO", "E");
            inquiry.SetUserDefinedField("~K!_ERRO", "601");

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1000));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();
            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 1, String.Join(Environment.NewLine, errors, "Errors are not equals to 1!"));

            var mode = response.GetMode();
            Assert.IsTrue("E".Equals(mode), "Inquiry failed!  Expected Mode=E");

            var err0 = errors[0];
            string errCode = err0.Substring(0, 3);
            Assert.IsTrue("601".Equals(errCode), "Inquiry failed!  Expected ERRO=601");
        }
    }
}