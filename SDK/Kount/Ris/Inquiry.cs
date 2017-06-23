//-----------------------------------------------------------------------
// <copyright file="Inquiry.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Ris
{

    /// <summary>
    /// Inquiry class. A bunch of setters for sending initial transaction
    /// data to a Kount RIS server.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 6.5.1. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class Inquiry : Kount.Ris.Request
    {
        /// <summary>
        /// Constructor. Sets the mode to 'Q', the currency to 'USD' and sets
        /// the SDK identifier value. Use SetMode(char) and SetCurrency(string)
        /// to change the RIS mode and currency respectively.
        /// </summary>
        public Inquiry() : base()
        {
            this.SetMode(Enums.InquiryTypes.ModeQ);
            this.SetCurrency("USD");
            this.Data["SDK"] = ".NET";
            this.SetSdkVersion("Sdk-Ris-Dotnet-0651");
        }

        /// <summary>
        /// Constructor. Sets the mode to 'Q', the currency to 'USD' and sets
        /// the SDK identifier value. Use SetMode(char) and SetCurrency(string)
        /// to change the RIS mode and currency respectively.
        /// </summary>
        /// <param name="checkConfiguration">If is true: will check config file if 
        /// `Ris.Url`, 
        /// `Ris.MerchantId`, 
        /// `Ris.Khash.Salt` and `Ris.Connect.Timeout` are set.</param>
        public Inquiry(bool checkConfiguration) : base(checkConfiguration)
        {
            this.SetMode(Enums.InquiryTypes.ModeQ);
            this.SetCurrency("USD");
            this.Data["SDK"] = ".NET";
            this.SetSdkVersion("Sdk-Ris-Dotnet-0651");
        }

        /// <summary>
        /// Set the mode of the inquiry.
        /// </summary>
        /// <param name="mode">Set mode Q or P</param>
        protected override void SetMode(char mode)
        {
            this.Data["MODE"] = mode;
        }

        /// <summary>
        /// Set the date of birth in the format YYYY-MM-DD
        /// </summary>
        /// <param name="dob">Date of birth</param>
        public void SetDateOfBirth(string dob)
        {
            this.Data["DOB"] = dob;
        }

        /// <summary>
        /// Set the gender
        /// </summary>
        /// <param name="gender">M(ale) or F(emale)</param>
        public void SetGender(char gender)
        {
            this.Data["GENDER"] = gender;
        }

        /// <summary>
        /// Set the value of a named user defined field
        /// </summary>
        /// <param name="label">Label of the user defined field</param>
        /// <param name="value">Value of the user defined field</param>
        public void SetUserDefinedField(string label, string value)
        {
            string index = "UDF[" + label + "]";
            this.Data[index] = value;
        }

        /// <summary>
        /// Set the three character ISO-4217 currency code.
        /// </summary>
        /// <param name="currency">Three character code e.g. USD.</param>
        public void SetCurrency(string currency)
        {
            this.Data["CURR"] = this.SafeGet(currency);
        }

        /// <summary>
        /// Set the current SDK version SDK_Type-RIS_VERSION-SDK_BUILD_DATETIMESTAMP.
        /// e.g. Sdk-Ris-Dnet-0651-20170505T1458
        /// </summary>
        /// <param name="sdkVersion">Three character code e.g. USD.</param>
        public void SetSdkVersion(string sdkVersion)
        {
            this.Data["SDK_VERSION"] = this.SafeGet(sdkVersion);
        }

        /// <summary>
        /// Set the total amount of the transaction.
        /// </summary>
        /// <param name="total">Total transaction amount in pennies.</param>
        public void SetTotal(int total)
        {
            this.Data["TOTL"] = total;
        }

        /// <summary>
        /// Set the IP address of the client.
        /// </summary>
        /// <param name="address">IP address of the client.</param>
        public void SetIpAddress(string address)
        {
            this.Data["IPAD"] = this.SafeGet(address);
        }

        /// <summary>
        /// Set the user agent of the client.
        /// </summary>
        /// <param name="userAgent"> User agent string of the client.</param>
        public void SetUserAgent(string userAgent)
        {
            this.Data["UAGT"] = this.SafeGet(userAgent);
        }

        /// <summary>
        /// Set the email address of the client.
        /// </summary>
        /// <param name="email">Email address.</param>
        public void SetEmail(string email)
        {
            this.Data["EMAL"] = this.SafeGet(email);
        }

        /// <summary>
        /// Set the name of the client.
        /// </summary>
        /// <param name="name">Client name.</param>
        public void SetName(string name)
        {
            this.Data["NAME"] = this.SafeGet(name);
        }

        /// <summary>
        /// Set the uniqe (cookie identifier, etc.) of the client.
        /// </summary>
        /// <param name="unique">Unique client identifier</param>
        public void SetUnique(string unique)
        {
            this.Data["UNIQ"] = this.SafeGet(unique);
        }

        /// <summary>
        /// Set the timestamp (in seconds) since the UNIX epoch for when the
        /// UNIQ value was set.
        /// </summary>
        /// <param name="timeStamp">Set the timestamp</param>
        public void SetEpoch(long timeStamp)
        {
            this.Data["EPOC"] = timeStamp;
        }

        /// <summary>
        /// Set the cash value of any fencible goods.
        /// </summary>
        /// <param name="cash">Cash value in pennies.</param>
        public void SetCash(int cash)
        {
            this.Data["CASH"] = cash;
        }

        /// <summary>
        /// Set the shipping type.
        /// </summary>
        /// <param name="shipType">Ship type: e.g. ST, ND, etc.</param>
        public void SetShipType(string shipType)
        {
            this.Data["SHTP"] = this.SafeGet(shipType);
        }

        /// <summary>
        /// Set the billing address.
        /// </summary>
        /// <param name="address1">Address line 1.</param>
        /// <param name="address2">Address line 2.</param>
        /// <param name="city">Set the city.</param>
        /// <param name="state">Set the state.</param>
        /// <param name="postalCode">Set the postal code.</param>
        /// <param name="country">The the country.</param>
        public void SetBillingAddress(
            string address1,
            string address2,
            string city,
            string state,
            string postalCode,
            string country)
        {
            this.Data["B2A1"] = this.SafeGet(address1);
            this.Data["B2A2"] = this.SafeGet(address2);
            this.Data["B2CI"] = this.SafeGet(city);
            this.Data["B2ST"] = this.SafeGet(state);
            this.Data["B2PC"] = this.SafeGet(postalCode);
            this.Data["B2CC"] = this.SafeGet(country);
        }

        /// <summary>
        /// Set the billing address.
        /// </summary>
        /// <param name="address1">Address line 1.</param>
        /// <param name="address2">Address line 2.</param>
        /// <param name="city">Set the city.</param>
        /// <param name="state">Set the state.</param>
        /// <param name="postalCode">Set the postal code.</param>
        /// <param name="country">Set the country.</param>
        /// <param name="premise">Set the premise.</param>
        /// <param name="street">Set the street.</param>
        public void SetBillingAddress(
            string address1,
            string address2,
            string city,
            string state,
            string postalCode,
            string country,
            string premise,
            string street)
        {
            this.Data["B2A1"] = this.SafeGet(address1);
            this.Data["B2A2"] = this.SafeGet(address2);
            this.Data["B2CI"] = this.SafeGet(city);
            this.Data["B2ST"] = this.SafeGet(state);
            this.Data["B2PC"] = this.SafeGet(postalCode);
            this.Data["B2CC"] = this.SafeGet(country);
            this.Data["BPREMISE"] = this.SafeGet(premise);
            this.Data["BSTREET"] = this.SafeGet(street);
        }

        /// <summary>
        /// Set the billing phone number.
        /// </summary>
        /// <param name="phoneNumber">Phone number.</param>
        public void SetBillingPhoneNumber(string phoneNumber)
        {
            this.Data["B2PN"] = this.SafeGet(phoneNumber);
        }

        /// <summary>
        /// Set the shipping address.
        /// </summary>
        /// <param name="address1">Address line 1.</param>
        /// <param name="address2">Address line 2.</param>
        /// <param name="city">Set the city.</param>
        /// <param name="state">Set the state.</param>
        /// <param name="postalCode">Set the postal code.</param>
        /// <param name="country">Set the country.</param>
        public void SetShippingAddress(
            string address1,
            string address2,
            string city,
            string state,
            string postalCode,
            string country)
        {
            this.Data["S2A1"] = this.SafeGet(address1);
            this.Data["S2A2"] = this.SafeGet(address2);
            this.Data["S2CI"] = this.SafeGet(city);
            this.Data["S2ST"] = this.SafeGet(state);
            this.Data["S2PC"] = this.SafeGet(postalCode);
            this.Data["S2CC"] = this.SafeGet(country);
        }

        /// <summary>
        /// Set the shipping address.
        /// </summary>
        /// <param name="address1">Address line 1.</param>
        /// <param name="address2">Address line 2.</param>
        /// <param name="city">Set the city.</param>
        /// <param name="state">Set the state.</param>
        /// <param name="postalCode">Set the postal code.</param>
        /// <param name="country">Set the country.</param>
        /// <param name="premise">Set the premise.</param>
        /// <param name="street">Set the street.</param>
        public void SetShippingAddress(
            string address1,
            string address2,
            string city,
            string state,
            string postalCode,
            string country,
            string premise,
            string street)
        {
            this.Data["S2A1"] = this.SafeGet(address1);
            this.Data["S2A2"] = this.SafeGet(address2);
            this.Data["S2CI"] = this.SafeGet(city);
            this.Data["S2ST"] = this.SafeGet(state);
            this.Data["S2PC"] = this.SafeGet(postalCode);
            this.Data["S2CC"] = this.SafeGet(country);
            this.Data["SPREMISE"] = this.SafeGet(premise);
            this.Data["SSTREET"] = this.SafeGet(street);
        }

        /// <summary>
        /// Set the shipping phone number.
        /// </summary>
        /// <param name="phoneNumber">Phone number.</param>
        public void SetShippingPhoneNumber(string phoneNumber)
        {
            this.Data["S2PN"] = this.SafeGet(phoneNumber);
        }

        /// <summary>
        /// Set the shipping name.
        /// </summary>
        /// <param name="shipName">Shipping Name.</param>
        public void SetShippingName(string shipName)
        {
            this.Data["S2NM"] = this.SafeGet(shipName);
        }

        /// <summary>
        /// Set the shipping email.
        /// </summary>
        /// <param name="shipEmail">Shipping Email.</param>
        public void SetShippingEmail(string shipEmail)
        {
            this.Data["S2EM"] = this.SafeGet(shipEmail);
        }

        /// <summary>
        /// Set the Anid of the client.
        /// </summary>
        /// <param name="anid">Anid of the client.</param>
        public void SetAnid(string anid)
        {
            this.Data["ANID"] = anid;
        }

        /// <summary>
        /// Website id associated with the transaction
        /// </summary>
        /// <param name="site">String 1 - 8 characters long</param>
        public void SetWebsite(string site)
        {
            this.Data["SITE"] = site;
        }

        /// <summary>
        /// Optional expiration month on the credit card
        /// </summary>
        /// <param name="month">String MM signifying the month</param>
        public void SetExpirationMonth(string month)
        {
            this.Data["CCMM"] = month;
        }

        /// <summary>
        /// Optional expiration year on the credit card
        /// </summary>
        /// <param name="year">String YYYY signifying the year</param>
        public void SetExpirationYear(string year)
        {
            this.Data["CCYY"] = year;
        }

        /// <summary>
        /// Set a shoppinng cart
        /// </summary>
        /// <param name="cart">ArrayList of Kount.Ris.CartItem objects</param>
        public void SetCart(System.Collections.ArrayList cart)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                Kount.Ris.CartItem item = (Kount.Ris.CartItem)(cart[i]);
                this.Data[$"PROD_TYPE[{i}]"] = item.ProductType;
                this.Data[$"PROD_ITEM[{i}]"] = item.ProductItem;
                this.Data[$"PROD_DESC[{i}]"] = item.ProductDescription;
                this.Data[$"PROD_QUANT[{i}]"] = item.ProductQuantity;
                this.Data[$"PROD_PRICE[{i}]"] = item.ProductPrice;
            }
        }
    }
}