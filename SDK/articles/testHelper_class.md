```cs
public class TestHelper
{

    /// <summary>
    /// Payment Type
    /// </summary>
    public const string PTYP_CARD = "CARD";

    /// <summary>
    /// Customer User-Agent HTTP header
    /// </summary>
    public const string UAGT = "Mozilla/5.0(Macintosh; Intel Mac OSX 10_9_5)AppleWebKit/537.36(KHTML, like Gecko)Chrome/37.0.2062.124Safari/537.36";

    /// <summary>
    /// Authorization Status returned to merchant from processor.
    /// </summary>
    private const char AUTH = 'A';

    /// <summary>
    /// Address Verification System Street verification response returned to merchant from processor.
    /// </summary>
    private const char AVST = 'M';

    /// <summary>
    /// Address Verification System Zip Code verification response returned to merchant from processor.
    /// </summary>
    private const char AVSZ = 'M';

    /// <summary>
    /// Billing street address - Line 1
    /// </summary>
    private const string B2A1 = "1234 North B2A1 Tree Lane South";

    /// <summary>
    /// Billing street address - Line 2
    /// </summary>
    private const string B2A2 = "";

    /// <summary>
    /// Billing address - Counry Code
    /// </summary>
    private const string B2CC = "US";

    /// <summary>
    /// Billing address - City
    /// </summary>
    private const string B2CI = "Albuquerque";

    /// <summary>
    /// Billing address - Postal Code
    /// </summary>
    private const string B2PC = "87101";

    /// <summary>
    /// Billing address - Phone Number
    /// </summary>
    private const string B2PN = "555-867-5309";

    /// <summary>
    /// Billing address - State/Province
    /// </summary>
    private const string B2ST = "NM";

    /// <summary>
    /// Total cash amount in currency submitted
    /// </summary>
    private const int CASH = 4444;

    /// <summary>
    /// Currency
    /// </summary>
    private const string CURR = "USD";

    /// <summary>
    /// Card Verification Value response returned to merchant from processor.
    /// </summary>
    private const char CVVR = 'M';

    /// <summary>
    /// Email
    /// </summary>
    private const string EMAIL = "sdkTest@kountsdktestdomain.com";

    /// <summary>
    /// Dotted Decimal IPv4 address that the merchant sees coming from the customer.
    /// </summary>
    private const string IPAD = "131.206.45.21";

    /// <summary>
    /// Last 4 numbers of Credit Card Value
    /// </summary>
    private const string LAST4 = "2514";

    /// <summary>
    /// Merchants acknowledgement to ship/process the order
    /// </summary>
    private const char MACK = 'Y';

    /// <summary>
    /// Risk Inquiry Service Mode
    /// </summary>
    private const char MODE = 'Q';

    /// <summary>
    /// Name of Merchant
    /// </summary>
    private const string NAME = "SdkTestFirstName SdkTestLastName";

    /// <summary>
    /// Card definitions
    /// </summary>
    private const string PROD_DESCR_0 = "3000 CANDLEPOWER PLASMA FLASHLIGHT";
    private const string PROD_ITEM_0 = "SG999999";
    private const long PROD_PRICE_0 = 68990;
    private const long PROD_QUANT_0 = 2;

    private const string PROD_TYPE_0 = "SPORTING_GOODS";

    /// <summary>
    /// Shipping Address Recipient address - Line 1
    /// </summary>
    private const string S2A1 = "567 West S2A1 Court North";

    /// <summary>
    /// Shipping Recipient street address - Line 2
    /// </summary>
    private const string S2A2 = "";

    /// <summary>
    /// Shipping Address Recipient - Counry Code
    /// </summary>
    private const string S2CC = "US";

    /// <summary>
    /// Shipping Address Recipient - City
    /// </summary>
    private const string S2CI = "Gnome";

    /// <summary>
    /// Shipping Address - Email address of Recipient
    /// </summary>
    private const string S2EM = "sdkTestShipToEmail@kountsdktestdomain.com";

    /// <summary>
    /// Shipping Address - Name of Recipient
    /// </summary>
    private const string S2NM = "SdkShipToFN SdkShipToLN";

    /// <summary>
    /// Shipping Address Recipient - Postal Code
    /// </summary>
    private const string S2PC = "99762";

    /// <summary>
    /// Shipping Address Recipient - Phone Number
    /// </summary>
    private const string S2PN = "555-777-1212";

    /// <summary>
    /// Shipping Address Recipient - State/Province
    /// </summary>
    private const string S2ST = "AK";

    /// <summary>
    /// Name of Web site
    /// </summary>
    private const string SITE = "DEFAULT";

    /// <summary>
    /// Total amount in currency submitted in lowest currency factor
    /// </summary>
    private const int TOTL = 123456;

	
    /// <summary>
    /// Create inquiry
    /// </summary>
    /// <param name="cardNumber">card number</param>
    /// <param name="sid">session id</param>
    /// <param name="orderNum">order number</param>
    /// <returns>inquiry</returns>
    public static Inquiry CreateInquiry(string cardNumber, out string sid, out string orderNum)
    {
        Inquiry inquiry = DefaultInquiry(out sid, out orderNum);

        inquiry.SetCardPayment(cardNumber);

        return inquiry;
    }

    /// <summary>
    /// Create masked inquiry
    /// </summary>
    /// <param name="cardNumber">card number</param>
    /// <param name="sid">session id</param>
    /// <param name="orderNum">order number</param>
    /// <returns>masked inquiry</returns>
    public static Inquiry CreateInquiryMasked(string cardNumber, out string sid, out string orderNum)
    {
        Inquiry inquiry = DefaultInquiry(out sid, out orderNum);

        inquiry.SetCardPaymentMasked(cardNumber);

        return inquiry;
    }

    /// <summary>
    /// Create inquiry with default settings
    /// </summary>
    /// <param name="sid">session id</param>
    /// <param name="orderNum">order number</param>
    /// <returns>inquiry with default settings</returns>
    public static Inquiry DefaultInquiry(out string sid, out string orderNum)
    {
        Inquiry inquiry = new Inquiry();

        sid = Guid.NewGuid().ToString().Replace("-", String.Empty);
        inquiry.SetSessionId(sid);

        var uniq = sid.Substring(0, 20); //Fist 20 chars from sid

        orderNum = uniq.Substring(0, 10);
        inquiry.SetUnique(uniq);
        inquiry.SetOrderNumber(orderNum);

        inquiry.SetName(NAME);
        inquiry.SetEmail(EMAIL);

        //set billing address
        inquiry.SetBillingAddress(B2A1, B2A2, B2CI, B2ST, B2PC, B2CC);
        inquiry.SetBillingPhoneNumber(B2PN);

        //set shipping address
        inquiry.SetShippingAddress(S2A1, S2A2, S2CI, S2ST, S2PC, S2CC);
        inquiry.SetShippingPhoneNumber(S2PN);

        inquiry.SetMode(MODE);

        inquiry.SetTotal(TOTL);
        inquiry.SetCash(CASH);
        inquiry.SetIpAddress(IPAD);

        inquiry.SetMack(MACK);
        inquiry.SetAuth(AUTH);
        inquiry.SetAvst(AVST);
        inquiry.SetAvsz(AVSZ);
        inquiry.SetCvvr(CVVR);

        inquiry.SetWebsite(SITE);
        inquiry.SetCurrency(CURR);

        return inquiry;
    }

}

```