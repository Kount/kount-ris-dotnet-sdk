//-----------------------------------------------------------------------
// <copyright file="InquiryMaskTest.cs" company="Keynetics Inc">
//     Copyright  Kount Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace KountRisTest
{
    using Kount.Ris;
    using Kount.Enums;
    using Kount.Util;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Inquiry Test samples with MASK parameter 
    /// PENC=MASK and PTYP=CARD
    /// <b>MerchantId:</b> 999666
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 0700 <br/>
    /// <b>Copyright:</b> 2017 Kount Inc. All Rights Reserved<br/>
    /// </summary>
    [TestClass]
    public class MaskInquiryTest
    {
        /// <summary>
        /// Payment Token
        /// </summary>
        private const string PTOK = "0007380568572514";
        private const string PTOK_2 = "370070538959797";

        //Fields
        private string _sid = "";
        private string _orderNum = "";

        /// <summary>
        /// <b>TEST 1</b>
        /// Mode Q call,
        /// One cart item, one rule triggered,
        /// approval status of REVIEW is returned
        /// </summary>
        [TestMethod]
        public void MaskRisQOneItemRequiredFieldsOneRuleReview()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("SPORTING_GOODS", "SG999999", "3000 CANDLEPOWER PLASMA FLASHLIGHT", 2, 68990));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();

            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var auto = response.GetAuto();
            Assert.IsTrue("R".Equals(auto), "Inquiry failed!  Approval Status is not equal to R");

            var warnings = response.GetWarnings();
            Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, "There are warnings in response!"));

            var sid = response.GetSessionId();
            Assert.IsTrue(_sid.Equals(sid), "Inquiry failed! Wrong session ID");

            var orderNum = response.GetOrderNumber();
            Assert.IsTrue(_orderNum.Equals(orderNum), "Inquiry failed! Wrong order number.");

            var rulesTrigg = response.GetRulesTriggered();
            Assert.IsTrue(rulesTrigg.Count == 1, "Inquiry failed! RULES TRIGGERED is not 1");
        }

        /// <summary>
        /// <b>TEST 2</b>
        /// Mode Q call with multiple items in cart, two rules triggered, an optional fields included,
        /// approval status of DECLINED is returned
        /// </summary>
        [TestMethod]
        public void MaskRisQMultiCartItemsTwoOptionalFieldsTwoRulesDecline()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            inquiry.SetUserAgent(TestHelper.UAGT);
            inquiry.SetTotal(123456789); //1000000

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1000));
            cart.Add(new CartItem("cart item 1 type", "cart item 1", "cart item 1 description", 11, 1001));
            cart.Add(new CartItem("cart item 2 type", "cart item 2", "cart item 2 description", 12, 1002));
            inquiry.SetCart(cart);

            //SET Customer User-Agent HTTP header UAGT
            Response response = inquiry.GetResponse();
            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var auto = response.GetAuto();
            Assert.IsTrue("D".Equals(auto), "Inquiry failed! Approval Status is not equal to D");

            var warnings = response.GetWarnings();
            Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, "There are warnings in response!"));

            var rulesTrigg = response.GetRulesTriggered();
            Assert.IsTrue(rulesTrigg.Count == 2, "Inquiry failed! RULES TRIGGERED is not 2");
        }

        /// <summary>
        /// TEST 3
        /// Ris mode Q with user defined fields
        /// </summary>
        [TestMethod]
        public void MaskRisQWithUserDefinedFields()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            inquiry.SetUserDefinedField("ARBITRARY_ALPHANUM_UDF", "alphanumeric trigger value");
            inquiry.SetUserDefinedField("ARBITRARY_NUMERIC_UDF", "777");

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1000));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();

            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var auto = response.GetAuto();
            Assert.IsTrue("R".Equals(auto), "Inquiry failed!  Approval Status is not equal to R");

            var warnings = response.GetWarnings();
            Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, "There are warnings in response!"));

            var rulesTrigg = response.GetRulesTriggered();
            Assert.IsTrue(rulesTrigg != null && rulesTrigg.Count != 0, "Inquiry failed! There no RULES_TRIGGERED.");

            List<string> listResponce = new List<string>(Regex.Split(response.ToString(), "[\r\n]+"));
            var filteredList = listResponce.FindAll(i => i.Contains("RULE_DESCRIPTION"));
            Assert.IsTrue(rulesTrigg.Count == filteredList.Count, "Inquiry failed! Count of RULES_TRIGGERED is wrong!");

            var r1 = filteredList.Find(r => r.Contains("review if ARBITRARY_ALPHANUM_UDF contains \"trigger\""));
            var r2 = filteredList.Find(r => r.Contains("review if ARBITRARY_NUMERIC_UDF == 777"));
            Assert.IsTrue(r1 != null && r2 != null, "Inquiry failed! The content of triggered rules are wrong!");
        }

        /// <summary>
        /// TEST 4
        /// Invalid value for a required field is sent, hard error returned
        /// </summary>
        [TestMethod]
        public void MaskRisQHardErrorExpected()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            //inquiry.SetParameter("PTOK", Khash.HashPaymentToken("BADPTOK"));
            inquiry.SetParameter("PTOK", "BADPTOK");
            inquiry.SetGender('M');

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1000));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();

            var mode = response.GetMode();
            Assert.IsTrue("E".Equals(mode), $"Update failed! Wrong response mode {mode}.");

            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 1, "Wrong responce expected error_num: 332, ERROR_COUNT=1");

            var err0 = errors[0];
            string errCode = err0.Substring(0, 3);
            Assert.IsTrue(err0.Contains(@"340 BAD_MASK Cause: [value [BADPTOK] did not match regex /^\d{6}X{5,9}\d{1,4}$/], Field: [PTOK], Value: [BADPTOK]"), $"Wrong error value: {err0}, expected 340");
        }

        /// <summary>
        /// TEST 5
        /// Warning reported but status of APPROVED returned
        /// </summary>
        [TestMethod]
        public void MaskRisQWarningApproved()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            //inquiry.SetPaymentTokenLast4("1111");
            inquiry.SetTotal(1000);
            inquiry.SetUserDefinedField("UDF_DOESNOTEXIST", "throw a warning please!");

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1234));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();

            var sessID = response.GetSessionId();
            var tranID = response.GetTransactionId();
            var ordNum = response.GetOrderNumber();

            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var auto = response.GetAuto();
            Assert.IsTrue("A".Equals(auto), $"Inquiry failed! Approval status {auto} is not equal to A");

            var warnings = response.GetWarnings();
            Assert.IsTrue(warnings.Count == 2, $"TranID: {tranID} - Wrong number of warnings: {warnings.Count}, expected 2.");

            List<string> listResponce = new List<string>(Regex.Split(response.ToString(), "[\r\n]+"));
            var filteredList = listResponce.FindAll(i => i.Contains("WARNING_"));
            var w1 = filteredList.Find(r => r.Contains("[UDF_DOESNOTEXIST=>throw a warning please!]"));
            var w2 = filteredList.Find(r => r.Contains("[The label [UDF_DOESNOTEXIST] is not defined for merchant ID [999666].]"));

            Assert.IsTrue(w1 != null, $"Inquiry failed! The value {warnings[0]} of warning is wrong!");
            Assert.IsTrue(w2 != null, $"Inquiry failed! The value {warnings[1]} of warning is wrong!");
        }

        /// <summary>
        /// TEST 6
        /// One hard error triggered, one warning triggered
        /// </summary>
        [TestMethod]
        public void MaskRisQHardSoftErrorsExpected()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            inquiry.SetPayment(TestHelper.PTYP_CARD, "BADPTOK");
            //inquiry.SetParameter("PTOK", Khash.HashPaymentToken("BADPTOK"));
            inquiry.SetUserDefinedField("UDF_DOESNOTEXIST", "throw a warning please!");

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1000));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse(false);

            var mode = response.GetMode();
            Assert.IsTrue("E".Equals(mode), $"Update failed! Wrong response mode {mode}.");

            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 1, "Wrong responce expected error_num: 332, ERROR_COUNT=1");

            var err0 = errors[0];
            //340 BAD_MASK Cause: [value [BADPTOK] did not match regex /^\d{6}X{5,9}\d{1,4}$/], Field: [PTOK], Value: [BADPTOK]
            Assert.IsTrue(err0.Contains(@"340 BAD_MASK Cause: [value [BADPTOK] did not match regex /^\d{6}X{5,9}\d{1,4}$/], Field: [PTOK], Value: [BADPTOK]"), $"Wrong error content: {err0}, expected 332.");

            var warnings = response.GetWarnings();
            Assert.IsTrue(warnings.Count == 3, $"Wrong number of warnings: {warnings.Count}, expected 2.");

            List<string> listResponce = new List<string>(Regex.Split(response.ToString(), "[\r\n]+"));
            var filteredList = listResponce.FindAll(i => i.Contains("WARNING_"));
            var w1 = filteredList.Find(r => r.Contains("[UDF_DOESNOTEXIST=>throw a warning please!]"));
            var w2 = filteredList.Find(r => r.Contains("[The label [UDF_DOESNOTEXIST] is not defined for merchant ID [999666].]"));
            var w3 = filteredList.Find(r => r.Contains("[LAST4 does not match last 4 characters in payment token]"));

            Assert.IsTrue(w1 != null, $"Inquiry failed! The value {warnings[0]} of warning is wrong!");
            Assert.IsTrue(w2 != null, $"Inquiry failed! The value {warnings[1]} of warning is wrong!");
            Assert.IsTrue(w3 != null, $"Inquiry failed! The value {warnings[2]} of warning is wrong!");
        }

        /// <summary>
        /// TEST 7
        /// No Kount Complete rules triggered,
        /// two Kount Central rules triggered,
        /// Kount Central status of REVIEW
        /// </summary>
        [TestMethod]
        public void MaskRisWTwoKCRulesReview()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            inquiry.SetMode(InquiryTypes.ModeW);
            inquiry.SetTotal(10001);
            inquiry.SetKountCentralCustomerId("KCentralCustomerOne");

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1234));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();

            var sessID = response.GetSessionId();
            var tranID = response.GetTransactionId();
            var ordNum = response.GetOrderNumber();

            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var mode = response.GetMode();
            Assert.IsTrue("W".Equals(mode), $"Update failed! Wrong response mode {mode}.");

            var warnings = response.GetWarnings();
            Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, "There are warnings in response!"));

            /*
                "KC_TRIGGERED_COUNT": 2
                "KC_WARNING_COUNT": 0
                "KC_DECISION": "R"
                "KC_EVENT_1_CODE": "billingToShippingAddressReview"
                "KC_EVENT_1_DECISION": "R"
                "KC_EVENT_2_CODE": "orderTotalReview"
                "KC_EVENT_2_DECISION": "R"
            */
            var kcCustId = response.GetKountCentralCustomerId();
            var kcDecision = response.GetKountCentralDecision();
            Assert.IsTrue("R".Equals(kcDecision), $"Inquiry failed! KC Decision {kcDecision} is not equal to R");

            var kcErrs = response.GetKountCentralErrors();
            Assert.IsTrue(kcErrs.Count == 0, $"Inquiry failed! KC Errors: {String.Join(Environment.NewLine, kcErrs)}");

            var kcWarn = response.GetKountCentralWarnings();
            Assert.IsTrue(kcWarn.Count == 0, $"Inquiry failed! KC Warnings: {String.Join(Environment.NewLine, kcWarn)}");

            var kcEvents = response.GetKountCentralThresholdEvents();
            Assert.IsTrue(kcEvents.Count == 2, $"Inquiry failed! KC Events: {kcEvents.Count} are not 2.");

            Assert.IsTrue("R".Equals(kcEvents[0].Decision), $"Inquiry failed! Wrong decisions d1 = {kcEvents[0].Decision}, d2 = {kcEvents[1].Decision} by Kount Central ThresholdEvents.");
            Assert.IsTrue("R".Equals(kcEvents[1].Decision), $"Inquiry failed! Wrong decisions d1 = {kcEvents[0].Decision}, d2 = {kcEvents[1].Decision} by Kount Central ThresholdEvents.");

            var code1 = kcEvents[0].Code;
            var code2 = kcEvents[1].Code;
            Assert.IsTrue("billingToShippingAddressReview".Equals(code1) || "billingToShippingAddressReview".Equals(code2),
                            $"Inquiry failed! Wrong  KC codes: {code1}, {code2} expected billingToShippingAddressReview.");

            Assert.IsTrue("orderTotalReview".Equals(code1) || "orderTotalReview".Equals(code2),
                        $"Inquiry failed! Wrong  KC codes: {code1}, {code2} expected orderTotalReview.");
        }

        /// <summary>
        /// TEST 8
        /// Mode J call one threshold triggered.
        /// "KC_EVENT_1_CODE": "orderTotalDecline",
        /// "KC_EVENT_1_DECISION": "D"
        /// </summary>
        [TestMethod]
        public void MaskRisJOneKountCentralRuleDecline()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            inquiry.SetMode(InquiryTypes.ModeJ);
            inquiry.SetTotal(1000);
            inquiry.SetKountCentralCustomerId("KCentralCustomerDeclineMe");

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1234));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();
            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var mode = response.GetMode();
            Assert.IsTrue("J".Equals(mode), $"Update failed! Wrong response mode {mode}.");

            var warnings = response.GetWarnings();
            Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, "There are warnings in response!"));
            /*
              "KC_TRIGGERED_COUNT": 1,
              "KC_WARNING_COUNT": 0,
              "KC_DECISION": "D",
              "KC_EVENT_1_CODE": "orderTotalDecline",
              "KC_EVENT_1_DECISION": "D"
             */
            var kcCustId = response.GetKountCentralCustomerId();
            var kcDecision = response.GetKountCentralDecision();
            Assert.IsTrue("D".Equals(kcDecision), $"Inquiry failed! KC Decision {kcDecision} is not equal to D");

            var kcErrs = response.GetKountCentralErrors();
            Assert.IsTrue(kcErrs.Count == 0, $"Inquiry failed! KC Errors: {String.Join(Environment.NewLine, kcErrs)}");

            var kcWarn = response.GetKountCentralWarnings();
            Assert.IsTrue(kcWarn.Count == 0, $"Inquiry failed! KC Warnings: {String.Join(Environment.NewLine, kcWarn)}");

            var kcEvents = response.GetKountCentralThresholdEvents();
            Assert.IsTrue(kcEvents.Count == 1, $"Inquiry failed! KC Events: {kcEvents.Count} are not 1.");
            Assert.IsTrue("D".Equals(kcEvents[0].Decision), $"Inquiry failed! Wrong decisions d1 = {kcEvents[0].Decision} by Kount Central ThresholdEvents.");

            var code1 = kcEvents[0].Code;
            Assert.IsTrue("orderTotalDecline".Equals(code1), $"Inquiry failed! Wrong  KC codes: {code1}, expected orderTotalDecline.");
        }

        /// <summary>
        /// TEST 9
        /// Mode U call submits updated values, but return values do not include the re-evalued transaction results.
        /// Default values mode Q transaction, capture TRAN, SESS, ORDR values, use those to submit a mode U
        /// </summary>
        [TestMethod]
        public void MaskModeUAfterModeQ()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1234));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();
            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var sessID = response.GetSessionId();
            var tranID = response.GetTransactionId();
            var ordNum = response.GetOrderNumber();

            Update update = new Update(false);
            update.SetMode(UpdateTypes.ModeU);
            update.SetVersion("0695");
            update.SetMerchantId(TestHelper.TEST_MERCHANT_ID);
            update.SetApiKey(TestHelper.TEST_API_KEY);
            update.SetSessionId(sessID);
            update.SetTransactionId(tranID);
            update.SetOrderNumber(ordNum);
            update.SetParameter("PTOK", Khash.HashPaymentToken("5386460135176807"));
            update.SetPaymentTokenLast4("6807");
            update.SetMack('Y');
            update.SetAuth('A');
            update.SetAvst('M');
            update.SetAvsz('M');
            update.SetCvvr('M');

            response = update.GetResponse();
            errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var mode = response.GetMode();
            Assert.IsTrue("U".Equals(mode), $"Update failed! Wrong response mode {mode}.");

            Assert.IsTrue(sessID.Equals(response.GetSessionId()), $"Update failed! Wrong session ID  {sessID}.");

            Assert.IsTrue(tranID.Equals(response.GetTransactionId()), $"Update failed! Wrong Transaction Id  {tranID}.");

            var ordU = response.GetOrderNumber(); // orderNum is null

            var auto = response.GetAuto();
            Assert.IsTrue(auto == null, $"Inquiry failed! Approval status {auto} is not null");

            var scor = response.GetScore();
            Assert.IsTrue(scor == null, $"Inquiry failed! Score {scor} is not null");

            var geox = response.GetGeox();
            Assert.IsTrue(geox == null, $"Inquiry failed! GEOX {geox} is not null");
        }

        /// <summary>
        /// TEST 10
        /// Mode X call submits updated values, and return values include all mode Q values, re-evaluated for updated data.
        /// To test, submit a default value mode Q transaction, capture TRAN, SESS, ORDR values, and then use those to submit a mode X
        /// </summary>
        [TestMethod]
        public void MaskModeXAfterModeQ()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1234));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();
            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var sessID = response.GetSessionId();
            var tranID = response.GetTransactionId();
            var ordNum = response.GetOrderNumber();
            Update update = new Update(false);
            update.SetMode(UpdateTypes.ModeX);
            update.SetVersion("0695");

            update.SetMerchantId(TestHelper.TEST_MERCHANT_ID);
            update.SetApiKey(TestHelper.TEST_API_KEY);

            update.SetSessionId(sessID);
            update.SetTransactionId(tranID);
            update.SetOrderNumber(ordNum);
            update.SetPaymentTokenLast4("6807");
            update.SetMack('Y');
            update.SetAuth('A');
            update.SetAvst('M');
            update.SetAvsz('M');
            update.SetCvvr('M');

            response = update.GetResponse();
            errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var mode = response.GetMode();
            Assert.IsTrue("X".Equals(mode), $"Update failed! Wrong response mode  {mode}.");

            var sID = response.GetSessionId();
            Assert.IsTrue(sessID.Equals(sID), $"Update failed! Wrong session ID  {sID}.");

            var tId = response.GetTransactionId();
            Assert.IsTrue(tranID.Equals(tId), $"Update failed! Wrong Transaction Id  {tranID}.");

            var ordU = response.GetOrderNumber();
            Assert.IsTrue(ordNum.Equals(ordU), $"Update failed! Wrong Order Number {ordNum}.");

            var auto = response.GetAuto();
            Assert.IsTrue(auto != null, $"Update failed! AUTO not presented in response.");

            var scor = response.GetScore();
            Assert.IsTrue(scor != null, $"Update failed! SCOR not presented in response.");

            var geox = response.GetGeox();
            Assert.IsTrue(geox != null, $"Update failed! GEOX not presented in response.");
        }

        /// <summary>
        /// TEST 11
        /// Approval status of APPROVED returned
        /// </summary>
        [TestMethod]
        public void MaskModeP()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK, out _sid, out _orderNum);

            inquiry.SetAnid("2085551212");
            inquiry.SetMode(InquiryTypes.ModeP);
            inquiry.SetTotal(1000);
            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 10, 1234));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();
            // optional getter
            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var mode = response.GetMode();
            Assert.IsTrue("P".Equals(mode), $"Update failed! Wrong response mode {mode}.");

            var auto = response.GetAuto();
            Assert.IsTrue("A".Equals(auto), $"Inquiry failed! Approval status {auto} is wrong, expected 'A'.");
        }

        /// <summary>
        /// TEST 12
        /// Mode Q call using payment encoding Mask with valid format
        /// </summary>
        [TestMethod]
        public void RisQUsingPaymentEncodingMaskValid()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK_2, out _sid, out _orderNum);

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("SPORTING_GOODS", "SG999999", "3000 CANDLEPOWER PLASMA FLASHLIGHT", 2, 68990));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();

            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var brdn = response.GetBrand();
            Assert.IsTrue("AMEX".Equals(brdn), "Inquiry failed!  Approval Status is not equal to R");
        }

        /// <summary>
        /// TEST 13
        /// Mode Q call using payment encoding Mask with invalid format
        /// </summary>
        [TestMethod]
        public void RisQUsingPaymentEncodingMaskError()
        {
            Inquiry inquiry = TestHelper.CreateInquiryMasked(PTOK_2, out _sid, out _orderNum);

            //replace masked "370070XXXXX9797" with "370070538959797"(invalid format)
            inquiry.SetParameter("PTOK", PTOK_2);

            // set CART with one item
            var cart = new ArrayList();
            cart.Add(new CartItem("SPORTING_GOODS", "SG999999", "3000 CANDLEPOWER PLASMA FLASHLIGHT", 2, 68990));
            inquiry.SetCart(cart);

            Response response = inquiry.GetResponse();

            var errors = response.GetErrors();
            Assert.IsTrue(errors.Count == 1, String.Join(Environment.NewLine, errors, "There are errors in response!"));

            var err0 = errors[0];
            //340 BAD_MASK Cause: [value [370070538959797] did not match regex /^\d{6}X{5,9}\d{1,4}$/], Field: [PTOK], Value: [370070538959797]
            Assert.IsTrue(err0.Contains(@"340 BAD_MASK Cause: [value [370070538959797] did not match regex /^\d{6}X{5,9}\d{1,4}$/], Field: [PTOK], Value: [370070538959797]"), $"Wrong error content: {err0}, expected 332.");

        }
    }

}