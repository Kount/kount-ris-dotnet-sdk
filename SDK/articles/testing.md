Testing
=====================

Test Project
------------

Current SDK includes `KountRisTest.proj` - consists implementation of all integration tests.

#### Test Helper  

Common definitions for all tests (Please refer to [TestHelper class](testHelper_class.md) for details).

> [!NOTE]
> When using the `Kount SDK` all credit card information, by default, uses the `KHASH` encryption method
> where the credit card information is irreversibly hashed prior to transmission from the merchant to Kount.

    Output : BIN + 14 alpha-numeric characters.
    Example: "123456A12C34E56G7DFG"

In the `Test project` are implemented, both encodings the `KHASH` and the `MASK`.
* **KHASH**
```cs
public static Inquiry CreateInquiry(string cardNumber, out string sid, out string orderNum)
{
    // create inquiry with default settings
    Inquiry inquiry = DefaultInquiry(out sid, out orderNum);

    inquiry.SetCardPayment(cardNumber);

    return inquiry;
}
```

*  **MASK**
```cs
public static Inquiry CreateInquiryMasked(string cardNumber, out string sid, out string orderNum)
{
    // create inquiry with default settings
    Inquiry inquiry = DefaultInquiry(out sid, out orderNum);

    // newly implemented method in SDK
    inquiry.SetCardPaymentMasked(cardNumber);

    return inquiry;
}
```

> [!NOTE]
> Diference is only by calling `SetCardPayment` and `SetCardPaymentMasked`.

Basic Connectivity Tests
------------------------

### Basic connectivity credentials

These are the credentials used for the most basic "does it work?" connection test.  
This is intended to simulate a new merchant providing their own credentials and verifying 
they can hit the Kount endpoint and receive a valid response.  

**Merchant Id :** 999666

**Authentication key:** provided by Kount

### 1. Mode Q call with one cart item. 
* One rule triggered, 
* approval status of `REVIEW` is returned

```cs
public void RisQOneItemRequiredFieldsOneRuleReview()
{
	// create inquiry	
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("SPORTING_GOODS", "SG999999", "3000 CANDLEPOWER PLASMA FLASHLIGHT", 
                            2, 68990));
    inquiry.SetCart(cart);

	//get response
    Response response = inquiry.GetResponse();

    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
                                            "There are errors in response!"));

    var auto = response.GetAuto();
    Assert.IsTrue("R".Equals(auto), "Inquiry failed!  Approval Status is not equal to R");

    var warnings = response.GetWarnings();
    Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, 
                                            "There are warnings in response!"));

    var sid = response.GetSessionId();
    Assert.IsTrue(_sid.Equals(sid), "Inquiry failed! Wrong session ID");

    var orderNum = response.GetOrderNumber();
    Assert.IsTrue(_orderNum.Equals(orderNum), "Inquiry failed! Wrong order number.");

    var rulesTrigg = response.GetRulesTriggered();
    Assert.IsTrue(rulesTrigg.Count == 1, "Inquiry failed! RULES TRIGGERED is not 1");
}

```

### 2. Mode Q call with multiple items in cart.
* two rules triggered,
* an optional fields included,
* approval status of `DECLINED` is returned

```cs
public void RisQMultiCartItemsTwoOptionalFieldsTwoRulesDecline()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    inquiry.SetUserAgent(TestHelper.UAGT);
    inquiry.SetTotal(123456789); 

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                            10, 1000));
    cart.Add(new CartItem("cart item 1 type", "cart item 1", "cart item 1 description", 
                            11, 1001));
    cart.Add(new CartItem("cart item 2 type", "cart item 2", "cart item 2 description", 
                            12, 1002));
    inquiry.SetCart(cart);

    //SET Customer User-Agent HTTP header UAGT
    Response response = inquiry.GetResponse();
    // optional getter
    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
                                    "There are errors in response!"));

    var auto = response.GetAuto();
    Assert.IsTrue("D".Equals(auto), "Inquiry failed! Approval Status is not equal to D");

    var warnings = response.GetWarnings();
    Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, 
                                    "There are warnings in response!"));

    var rulesTrigg = response.GetRulesTriggered();
    Assert.IsTrue(rulesTrigg.Count == 2, "Inquiry failed! RULES TRIGGERED is not 2");
}

```
### 3. Ris mode Q with user defined fields.
* approval status of `REVIEW` is returned

```cs
public void RisQWithUserDefinedFields()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    inquiry.SetUserDefinedField("ARBITRARY_ALPHANUM_UDF", "alphanumeric trigger value");
    inquiry.SetUserDefinedField("ARBITRARY_NUMERIC_UDF", "777");

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                            10, 1000));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse();

    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
                                        "There are errors in response!"));

    var auto = response.GetAuto();
    Assert.IsTrue("R".Equals(auto), "Inquiry failed!  Approval Status is not equal to R");

    var warnings = response.GetWarnings();
    Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, 
                                        "There are warnings in response!"));

    var rulesTrigg = response.GetRulesTriggered();
    Assert.IsTrue(rulesTrigg != null && rulesTrigg.Count != 0, 
                        "Inquiry failed! There no RULES_TRIGGERED.");

    List<string> listResponce = new List<string>(Regex.Split(response.ToString(), 
                        "[\r\n]+"));
    var filteredList = listResponce.FindAll(i => i.Contains("RULE_DESCRIPTION"));
    Assert.IsTrue(rulesTrigg.Count == filteredList.Count, 
                        "Inquiry failed! Count of RULES_TRIGGERED is wrong!");

    var r1 = filteredList.Find(r => 
                        r.Contains("review if ARBITRARY_ALPHANUM_UDF contains \"trigger\""));
    var r2 = filteredList.Find(r => r.Contains("review if ARBITRARY_NUMERIC_UDF == 777"));
    Assert.IsTrue(r1 != null && r2 != null, 
                        "Inquiry failed! The content of triggered rules are wrong!");
}

```
### 4. Invalid value for a required field is sent, 
* hard error returned

```cs
public void RisQHardErrorExpected()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    inquiry.SetParameter("PTOK", Khash.HashPaymentToken("BADPTOK"));
    inquiry.SetGender('M');

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                            10, 1000));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse();

    var mode = response.GetMode();
    Assert.IsTrue("E".Equals(mode), $"Update failed! Wrong response mode {mode}.");

    // optional getter
    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 1, "Wrong responce expected error_num: 332, ERROR_COUNT=1");

    var err0 = errors[0];
    string errCode = err0.Substring(0, 3);
    Assert.IsTrue(err0.Contains("332 BAD_CARD Cause: [PTOK invalid format], Field: [PTOK], 
                                 Value: [hidden]"), $"Wrong error value: {err0}, expected 332");
}
```
### 5. Warning reported but status of `APPROVED` returned.
* two warnings are returned.
	* `[UDF_DOESNOTEXIST=>throw a warning please!]`
	* `[The label [UDF_DOESNOTEXIST] is not defined for merchant ID [999666].]`
```cs
public void RisQWarningApproved()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    inquiry.SetTotal(1000);
    inquiry.SetUserDefinedField("UDF_DOESNOTEXIST", "throw a warning please!");

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                            10, 1234));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse();

    var sessID = response.GetSessionId();
    var tranID = response.GetTransactionId();
    var ordNum = response.GetOrderNumber();

    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
                                        "There are errors in response!"));

    var auto = response.GetAuto();
    Assert.IsTrue("A".Equals(auto), 
               $"Inquiry failed! Approval status {auto} is not equal to A");

    var warnings = response.GetWarnings();
    Assert.IsTrue(warnings.Count == 2, 
               $"TranID: {tranID} - Wrong number of warnings: {warnings.Count}, expected 2.");

    List<string> listResponce = new List<string>(Regex.Split(response.ToString(), "[\r\n]+"));
    var filteredList = listResponce.FindAll(i => i.Contains("WARNING_"));
    var w1 = filteredList.Find(r => r.Contains("[UDF_DOESNOTEXIST=>throw a warning please!]"));
    var w2 = filteredList.Find(r => 
        r.Contains("[The label [UDF_DOESNOTEXIST] is not defined for merchant ID [999666].]"));

    Assert.IsTrue(w1 != null, $"Inquiry failed! The value {warnings[0]} of warning is wrong!");
    Assert.IsTrue(w2 != null, $"Inquiry failed! The value {warnings[1]} of warning is wrong!");
}

```
### 6. One hard error triggered, one warning triggered.
* one error
	* `332 BAD_CARD Cause: [PTOK invalid format], Field: [PTOK], Value: [hidden]`
* two warnings are returned.
	* `[UDF_DOESNOTEXIST=>throw a warning please!]`
	* `[The label [UDF_DOESNOTEXIST] is not defined for merchant ID [999666].]`

```cs
public void RisQHardSoftErrorsExpected()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    inquiry.SetParameter("PTOK", Khash.HashPaymentToken("BADPTOK"));
    inquiry.SetUserDefinedField("UDF_DOESNOTEXIST", "throw a warning please!");

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                    10, 1000));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse(false);

    var mode = response.GetMode();
    Assert.IsTrue("E".Equals(mode), $"Update failed! Wrong response mode {mode}.");

    // optional getter
    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 1, 
                    "Wrong responce expected error_num: 332, ERROR_COUNT=1");

    var err0 = errors[0];
    Assert.IsTrue(err0.Contains("332 BAD_CARD Cause: [PTOK invalid format], 
              Field: [PTOK], Value: [hidden]"), $"Wrong error content: {err0}, expected 332.");

    var warnings = response.GetWarnings();
    Assert.IsTrue(warnings.Count == 2, 
              $"Wrong number of warnings: {warnings.Count}, expected 2.");

    List<string> listResponce = new List<string>(Regex.Split(response.ToString(), "[\r\n]+"));
    var filteredList = listResponce.FindAll(i => i.Contains("WARNING_"));
    var w1 = filteredList.Find(r => r.Contains("[UDF_DOESNOTEXIST=>throw a warning please!]"));
    var w2 = filteredList.Find(r => 
        r.Contains("[The label [UDF_DOESNOTEXIST] is not defined for merchant ID [999666].]"));

    Assert.IsTrue(w1 != null, $"Inquiry failed! The value {warnings[0]} of warning is wrong!");
    Assert.IsTrue(w2 != null, $"Inquiry failed! The value {warnings[1]} of warning is wrong!");
}

```
### 7. No Kount Complete rules triggered,
* two Kount Central `rules` triggered,
* Kount Central status of `REVIEW`

                KC_TRIGGERED_COUNT : 2
                KC_WARNING_COUNT   : 0
                KC_DECISION        : "R"
                KC_EVENT_1_CODE    : "billingToShippingAddressReview"
                KC_EVENT_1_DECISION: "R"
                KC_EVENT_2_CODE    : "orderTotalReview"
                KC_EVENT_2_DECISION: "R"


```cs
public void RisWTwoKCRulesReview()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    inquiry.SetMode('W');
    inquiry.SetTotal(10001);
    inquiry.SetKountCentralCustomerId("KCentralCustomerOne");

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                    10, 1234));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse();

    var sessID = response.GetSessionId();
    var tranID = response.GetTransactionId();
    var ordNum = response.GetOrderNumber();

    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
                "There are errors in response!"));

    var mode = response.GetMode();
    Assert.IsTrue("W".Equals(mode), $"Update failed! Wrong response mode {mode}.");

    var warnings = response.GetWarnings();
    Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, 
                "There are warnings in response!"));

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
    Assert.IsTrue("R".Equals(kcDecision), 
                    $"Inquiry failed! KC Decision {kcDecision} is not equal to R");

    var kcErrs = response.GetKountCentralErrors();
    Assert.IsTrue(kcErrs.Count == 0, 
                    $"Inquiry failed! KC Errors: {String.Join(Environment.NewLine, kcErrs)}");

    var kcWarn = response.GetKountCentralWarnings();
    Assert.IsTrue(kcWarn.Count == 0, 
                    $"Inquiry failed! KC Warnings: {String.Join(Environment.NewLine, kcWarn)}");

    var kcEvents = response.GetKountCentralThresholdEvents();
    Assert.IsTrue(kcEvents.Count == 2, $"Inquiry failed! KC Events: 
                                                   {kcEvents.Count} are not 2.");

    Assert.IsTrue("R".Equals(kcEvents[0].Decision), $"Inquiry failed! Wrong decisions d1 = 
      {kcEvents[0].Decision}, d2 = {kcEvents[1].Decision} by Kount Central ThresholdEvents.");
    Assert.IsTrue("R".Equals(kcEvents[1].Decision), $"Inquiry failed! Wrong decisions d1 = 
      {kcEvents[0].Decision}, d2 = {kcEvents[1].Decision} by Kount Central ThresholdEvents.");

    var code1 = kcEvents[0].Code;
    var code2 = kcEvents[1].Code;
    Assert.IsTrue("billingToShippingAddressReview".Equals(code1) 
            || "billingToShippingAddressReview".Equals(code2),
      $"Inquiry failed! Wrong  KC codes: {code1}, {code2} expected billingToShippingAddressReview.");

    Assert.IsTrue("orderTotalReview".Equals(code1) || "orderTotalReview".Equals(code2),
                $"Inquiry failed! Wrong  KC codes: {code1}, {code2} expected orderTotalReview.");
}

```
### 8. Mode J call one threshold triggered.
* mode `J` is returned.

              KC_TRIGGERED_COUNT : 1
              KC_WARNING_COUNT	 : 0
              KC_DECISION		 : "D"
              KC_EVENT_1_CODE	 : "orderTotalDecline"
              KC_EVENT_1_DECISION: "D"

```cs
public void RisJOneKountCentralRuleDecline()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    inquiry.SetMode('J');
    inquiry.SetTotal(1000);
    inquiry.SetKountCentralCustomerId("KCentralCustomerDeclineMe");

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                    10, 1234));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse();
    // optional getter
    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
                        "There are errors in response!"));

    var mode = response.GetMode();
    Assert.IsTrue("J".Equals(mode), $"Update failed! Wrong response mode {mode}.");

    var warnings = response.GetWarnings();
    Assert.IsTrue(warnings.Count == 0, String.Join(Environment.NewLine, warnings, 
                        "There are warnings in response!"));
    /*
      "KC_TRIGGERED_COUNT": 1,
      "KC_WARNING_COUNT": 0,
      "KC_DECISION": "D",
      "KC_EVENT_1_CODE": "orderTotalDecline",
      "KC_EVENT_1_DECISION": "D"
     */
    var kcCustId = response.GetKountCentralCustomerId();
    var kcDecision = response.GetKountCentralDecision();
    Assert.IsTrue("D".Equals(kcDecision), 
			$"Inquiry failed! KC Decision {kcDecision} is not equal to D");

    var kcErrs = response.GetKountCentralErrors();
    Assert.IsTrue(kcErrs.Count == 0, 
			$"Inquiry failed! KC Errors: {String.Join(Environment.NewLine, kcErrs)}");

    var kcWarn = response.GetKountCentralWarnings();
    Assert.IsTrue(kcWarn.Count == 0, 
			$"Inquiry failed! KC Warnings: {String.Join(Environment.NewLine, kcWarn)}");

    var kcEvents = response.GetKountCentralThresholdEvents();
    Assert.IsTrue(kcEvents.Count == 1, $"Inquiry failed! KC Events: {kcEvents.Count} are not 1.");
    Assert.IsTrue("D".Equals(kcEvents[0].Decision), 
			$"Inquiry failed! Wrong decisions d1 = 
                    {kcEvents[0].Decision} by Kount Central ThresholdEvents.");

    var code1 = kcEvents[0].Code;
    Assert.IsTrue("orderTotalDecline".Equals(code1), 
			$"Inquiry failed! Wrong  KC codes: {code1}, expected orderTotalDecline.");
}

```
### 9. Mode U call submits updated values.
* return values do not include the re-evalued transaction results,

```cs
public void ModeUAfterModeQ()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                    10, 1234));
    inquiry.SetCart(cart);

    // get response
    Response response = inquiry.GetResponse();

    // optional getter
    var errors = response.GetErrors();
    if (errors.Count > 0)
    {
        Assert.IsTrue(false, String.Join(Environment.NewLine, errors));
        return;
    }

    var sessID = response.GetSessionId();
    var tranID = response.GetTransactionId();
    var ordNum = response.GetOrderNumber();

    // create Update
    Update update = new Update();
    update.SetMode('U');
    update.SetVersion("0700");
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

    Assert.IsTrue(sessID.Equals(response.GetSessionId()), 
									$"Update failed! Wrong session ID  {sessID}.");

    Assert.IsTrue(tranID.Equals(response.GetTransactionId()), 
									$"Update failed! Wrong Transaction Id  {tranID}.");

    var ordU = response.GetOrderNumber(); // orderNum is null

    var auto = response.GetAuto();
    Assert.IsTrue(auto == null, $"Inquiry failed! Approval status {auto} is not null");

    var scor = response.GetScore();
    Assert.IsTrue(scor == null, $"Inquiry failed! Score {scor} is not null");

    var geox = response.GetGeox();
    Assert.IsTrue(geox == null, $"Inquiry failed! GEOX {geox} is not null");
}

```
### 10.  Mode `X` call submits updated values.
* return values include all mode `Q` values, re-evaluated for updated data,
* To test, submit a default value mode `Q` transaction, capture `TRAN`, `SESS`, `ORDR` values, and then use those to submit a mode `X`

```cs
public void ModeXAfterModeQ()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", 
							"cart item 0 description", 10, 1234));
    inquiry.SetCart(cart);

    // get response
    Response response = inquiry.GetResponse();
    
    // optional getter
    var errors = response.GetErrors();
    if (errors.Count > 0)
    {
        Assert.IsTrue(false, String.Join(Environment.NewLine, errors));
        return;
    }

    var sessID = response.GetSessionId();
    var tranID = response.GetTransactionId();
    var ordNum = response.GetOrderNumber();
    // create update
    Update update = new Update();
    update.SetMode('X');
    update.SetVersion("0700");

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
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
									"There are errors in response!"));

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

```
### 11. Approval status of `APPROVED` returned.
* mode `P` is returned.

```cs
public void ModeP()
{
    Inquiry inquiry = TestHelper.CreateInquiry(PTOK, out _sid, out _orderNum);

    inquiry.SetAnid("2085551212");
    inquiry.SetMode('P');
    inquiry.SetTotal(1000);
    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", 
							"cart item 0 description", 10, 1234));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse();
    // optional getter
    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
						"There are errors in response!"));

    var mode = response.GetMode();
    Assert.IsTrue("P".Equals(mode), $"Update failed! Wrong response mode {mode}.");

    var auto = response.GetAuto();
    Assert.IsTrue("A".Equals(auto), 
			$"Inquiry failed! Approval status {auto} is wrong, expected 'A'.");
}

```

### 12. Create Inquiry object without need of app.config settings

```cs
public void CreateInquiryWithoutNeedOfAppConfigSettings()
{
    Configuration configuration = new Configuration();
    configuration.MerchantId = "1234567";
    configuration.ApiKey = "api_key_str";
    configuration.URL = "url_str";
    configuration.ConnectTimeout = "10000";
    Inquiry inquiry = new Inquiry(false, configuration);
            
    Assert.IsTrue(inquiry.GetParam("MERC") == configuration.MerchantId, "MerchantId is not set correct.");
    Assert.IsTrue(inquiry.GetUrl() == configuration.URL, "URL is not set correct.");
}

```

Predictive Response Tests
-------------------------

Predictive Response is a mechanism that can be used by Kount merchants to submit test requests and
receive back predictable RIS responses. This means that a merchant, in order to test RIS, can generate
a particular request that is designed to provide one or more specific RIS responses and/or errors. The
predictive response inquiries are not actual RIS inquiries, which means the data will never be submitted
to the database and will not be displayed in the Agent Web Console.

The primary reason for having Predictive Response functionality is to diagnose error responses being
received from RIS. For instance, a merchant may receive a large number of different error codes after
submitting a RIS request. Most of these errors can be reliably reproduced by passing malformed, missing,
or additional data in the RIS request. However, some of the errors are extremely difficult or even
impossible to reproduce through simple means. There is no way to re-create these errors in a systematic
or predictable fashion using RIS request input, rules, and/or Data Collector.

**Merchant Id :** 999667

**Authentication key:** provided by Kount

### 1. Expected `Score=42` and expected `Decision = R`
* Input to predictive response, `UDF[~K!_SCOR] = 42`
  * Produces `RIS` output, `SCOR=42`
* Input to predictive response, `UDF[~K!_AUTO] = R`
  * Produces `RIS` output, `AUTO=R`
* Email input will need to be, `EMAL=predictive @kount.com`

```cs
public void PredictiveResponseScore42AutoR()
{
    Inquiry inquiry = CreateInquiry();

    inquiry.SetUserDefinedField("~K!_SCOR", "42");
    inquiry.SetUserDefinedField("~K!_AUTO", "R");

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                10, 1000));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse();
    // optional getter
    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
                    "There are errors in response!"));

    var score = response.GetScore();
    Assert.IsTrue("42".Equals(score), "Inquiry failed!   Expected Score=42.");

    var auto = response.GetAuto();
    Assert.IsTrue("R".Equals(auto), "Inquiry failed!  Expected Decision=R");
}

```
### 2. Expected `Score=42` and expected `Decision = D`
* Input to predictive response, `UDF[~K!_SCOR] = 42`
  * Produces `RIS` output, `SCOR=42` 
* Input to predictive response, `UDF[~K!_AUTO] = D`
  * Produces `RIS` output, `AUTO=D`
* Input to predictive response, `UDF[~K!_GEOX]=NG` 
* Email input will need to be, `EMAL=predictive @kount.com`

```cs
public void PredictiveResponseScore42AutoD()
{
    Inquiry inquiry = CreateInquiry();

    inquiry.SetUserDefinedField("~K!_SCOR", "42");
    inquiry.SetUserDefinedField("~K!_AUTO", "D");
    inquiry.SetUserDefinedField("~K!_GEOX", "NG");

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                10, 1000));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse();
    // optional getter
    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 0, String.Join(Environment.NewLine, errors, 
                    "There are errors in response!"));

    var score = response.GetScore();
    Assert.IsTrue("42".Equals(score), "Inquiry failed!   Expected Score=42.");

    var auto = response.GetAuto();
    Assert.IsTrue("D".Equals(auto), "Inquiry failed!  Expected Decision=D");

    var geox = response.GetGeox();
    Assert.IsTrue("NG".Equals(geox), "Inquiry failed!  Expected GEOX=NG");
}

```

### 3. Expected Score=18
* Input to predictive response, `UDF[~K!_SCOR] = 18`
  * Produces `RIS` output, `MODE=E` and `ERRO=601`
* Email input will need to be, `EMAL=predictive @kount.com`

```cs
public void PredictiveResponseScore18ModeE()
{
    Inquiry inquiry = CreateInquiry();

    inquiry.SetUserDefinedField("~K!_SCOR", "18");
    inquiry.SetUserDefinedField("~K!_AUTO", "E");
    inquiry.SetUserDefinedField("~K!_ERRO", "601");

    // set CART with one item
    var cart = new ArrayList();
    cart.Add(new CartItem("cart item 0 type", "cart item 0", "cart item 0 description", 
                    10, 1000));
    inquiry.SetCart(cart);

    Response response = inquiry.GetResponse();
    // optional getter
    var errors = response.GetErrors();
    Assert.IsTrue(errors.Count == 1, String.Join(Environment.NewLine, errors, 
                                    "Errors are not equals to 1!"));

    var mode = response.GetMode();
    Assert.IsTrue("E".Equals(mode), "Inquiry failed!  Expected Mode=E");

    var err0 = errors[0];
    string errCode = err0.Substring(0, 3);
    Assert.IsTrue("601".Equals(errCode), "Inquiry failed!  Expected ERRO=601");
}

```
Khash SALT Tests
----------------

Test examples to verifying that KHASH produces the correct value.

**Test Ideas**:
* Verify salt is configured with correct value
* Verify Khash can retrieve salt from the configuration
* Verify Khash produces expected results for a set of test cards

### 1. Credit Card `4111111111111111`
* Input raw: `4111111111111111`
* Expected Khash: `411111WMS5YA6FUZA1KC`

```cs
public void TestKhashCreditCard1()
{
    string _sid = null;
    string _orderNum = null;

    Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_1, out _sid, out _orderNum);

    var hashCardNum = inquiry.GetParam("PTOK");

    Assert.IsTrue("411111WMS5YA6FUZA1KC".Equals(hashCardNum), 
               $"Wrong result: {hashCardNum}. Expected KHASH 411111WMS5YA6FUZA1KC.");
}
```

### 2. Credit Card `5199185454061655`
* Input raw: `5199185454061655`
* Expected Khash: `5199182NOQRXNKTTFL11`

```cs
public void TestKhashCreditCard2()
{
     string _sid = null;
     string _orderNum = null;

     Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_2, out _sid, out _orderNum);

     var hashCardNum = inquiry.GetParam("PTOK");

     Assert.IsTrue("5199182NOQRXNKTTFL11".Equals(hashCardNum), 
        $"Wrong result: {hashCardNum}. Expected KHASH 5199182NOQRXNKTTFL11.");
}
```

### 3. Credit Card `4259344583883`
* Input raw: `4259344583883`
* Expected Khash: `425934FEXQI1QS6TH2O5`

```cs
public void TestKhashCreditCard3()
{
    string _sid = null;
    string _orderNum = null;

    Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_3, out _sid, out _orderNum);

    var hashCardNum = inquiry.GetParam("PTOK");

    Assert.IsTrue("425934FEXQI1QS6TH2O5".Equals(hashCardNum), $"Wrong result: {hashCardNum}. Expected KHASH 425934FEXQI1QS6TH2O5.");
}
```

### 4. Gift Card `4111111111111111`
* Input raw: `4111111111111111`  MerchantId: `666666`
* Expected Khash: `666666WMS5YA6FUZA1KC`

```cs
public void TestKhashGiftCard1()
{
    string _sid = null;
    string _orderNum = null;

    Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_1, out _sid, out _orderNum);

    inquiry.SetMerchantId(MERCHANT_ID);

    inquiry.SetGiftCardPayment(CARD_NUM_1);

    var hashCardNum = inquiry.GetParam("PTOK");

    Assert.IsTrue("666666WMS5YA6FUZA1KC".Equals(hashCardNum), 
                $"Wrong result: {hashCardNum}. Expected KHASH 666666WMS5YA6FUZA1KC.");
}
```

### 5. Gift Card `5199185454061655`
* Input raw: `5199185454061655` MerchantId: `666666`
* Expected Khash: `6666662NOQRXNKTTFL11`

```cs
public void TestKhashGiftCard2()
{
    string _sid = null;
    string _orderNum = null;

    Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_2, out _sid, out _orderNum);

    inquiry.SetMerchantId(MERCHANT_ID);

    inquiry.SetGiftCardPayment(CARD_NUM_2);

    var hashCardNum = inquiry.GetParam("PTOK");

    Assert.IsTrue("6666662NOQRXNKTTFL11".Equals(hashCardNum), 
                    $"Wrong result: {hashCardNum}. Expected KHASH 6666662NOQRXNKTTFL11.");
}
```

### 6. Gift Card `4259344583883`
* Input raw: `4259344583883` MerchantId: `666666`
* Expected Khash: `666666FEXQI1QS6TH2O5`

```cs
public void TestKhashGiftCard3()
{
    string _sid = null;
    string _orderNum = null;

    Inquiry inquiry = TestHelper.CreateInquiry(CARD_NUM_3, out _sid, out _orderNum);

    inquiry.SetMerchantId(MERCHANT_ID);

    inquiry.SetGiftCardPayment(CARD_NUM_3);

    var hashCardNum = inquiry.GetParam("PTOK");

    Assert.IsTrue("666666FEXQI1QS6TH2O5".Equals(hashCardNum), 
                    $"Wrong result: {hashCardNum}. Expected KHASH 666666FEXQI1QS6TH2O5.");
}
```


Custom Merchant ID and API Key
------------------------------

One integration test allows a Developer to set customer(new merchant) credentials in config project file. 
This test is geared toward ensuring the customer's credentials are valid, but doesn't depend on a RuleSet being in a specific state.
