//-----------------------------------------------------------------------
// <copyright file="Request.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

namespace Kount.Ris
{
    using Kount.Enums;
    using Kount.Util;
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Web;
    using System.Xml;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Abstract parent class for request objects.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 8.0.0. <br/>
    /// <b>Copyright:</b> 2025 Equifax<br/>
    /// </summary>
    public abstract class Request : LoggingComponent
    {
        private const string CUSTOM_HEADER_MERCHANT_ID = "X-Kount-Merc-Id";
        private const string CUSTOM_HEADER_API_KEY = "X-Kount-Api-Key";
        private const string PF_AUTH_HEADER = "Authorization";
        
        private static BearerAuthResponse _bearerAuthResponse = new BearerAuthResponse();
        private static DateTimeOffset _bearerAuthResponseExpiration = DateTimeOffset.Now;
        private static ReaderWriterLock _bearerRefreshLock = new ReaderWriterLock();
        

        /// <summary>
        /// The RIS version
        /// </summary>
        private const string RisVersion = "0720";

        /// <summary>
        /// Hash table of request data.
        /// </summary>
        private Hashtable data = null;

        /// <summary>
        /// URL of the target RIS server.
        /// </summary>
        private string url = null;

        /// <summary>
        /// RIS connect timeout in milliseconds.
        /// </summary>
        private int connectTimeout;

        /// <summary>
        /// Absolute path of the certificate file. Deprecated in favor of API key.
        /// </summary>
        private string certificate = null;

        /// <summary>
        /// Password used to export the certificate. Deprecated in favor of API key.
        /// </summary>
        private string password = null;

        /// <summary>
        /// API key used for authentication to RIS server. Obtained from the AWC.
        /// </summary>
        private string apiKey = null;

        /// <summary>
        /// API key used for get configuration value of logTimeElapsed.
        /// </summary>
        private bool logTimeElapsed;

        /// <summary>
        /// Is Migration mode enabled
        /// </summary>
        private readonly bool _migrationModeEnabled;

        /// <summary>
        /// Construct a request object. Set the static setting from the
        /// web.config file.
        /// </summary>
        /// <param name="checkConfiguration">By default is true: will check config file if 
        /// `Ris.Url`, `Ris.MerchantId`, `Ris.Config.Key` and 
        /// `Ris.Connect.Timeout` are set.</param>
        /// <param name="configuration">Instance of configuration.</param>
        /// <param name="logger">ILogger object for logging output</param>
        /// <exception cref="Kount.Ris.RequestException">Thrown when there is
        /// static data missing for a RIS request.</exception>
         protected Request(bool checkConfiguration, Configuration configuration, ILogger logger = null) : base(logger, typeof(Request))
        {
            if (checkConfiguration)
            {
                this.CheckConfigurationParameter(configuration.MerchantId, nameof(configuration.MerchantId));
                this.CheckConfigurationParameter(configuration.URL, nameof(configuration.URL));
                this.CheckConfigurationParameter(configuration.ConfigKey, nameof(configuration.ConfigKey));
                this.CheckConfigurationParameter(configuration.ConnectTimeout, nameof(configuration.ConnectTimeout));
            }

            logTimeElapsed = !String.IsNullOrEmpty(configuration.LogSimpleElapsed) && 
                             configuration.LogSimpleElapsed.Trim().ToLower().Equals("on");

            _migrationModeEnabled = configuration.GetEnableMigrationMode();
            
            logger?.LogDebug("migration mode enabled: " + _migrationModeEnabled);

            this.data = new System.Collections.Hashtable();
            
            if (!_migrationModeEnabled)
            {
                this.SetMerchantId(Int64.Parse(configuration.MerchantId));
            }
            else
            {
                if (configuration.PaymentsFraudClientId != string.Empty)
                {
                    this.SetMerchantId(Int64.Parse(configuration.PaymentsFraudClientId));
                }
                else
                {
                    this.SetMerchantId(Int64.Parse(configuration.MerchantId));
                    logger?.LogWarning("Client ID is not set. Falling back to merchant id, this may not work as expected.");
                }
            }

            Khash.ConfigKey = Khash.GetBase85ConfigKey(configuration.ConfigKey);

            var risVersion = String.IsNullOrEmpty(configuration.Version)
                        ? RisVersion
                        : configuration.Version;

            this.SetVersion(risVersion);
            if (!_migrationModeEnabled)
            {
                this.SetUrl(configuration.URL);
            }
            else
            {
                this.SetUrl(configuration.PaymentsFraudApiUrl);
                if (_bearerAuthResponseExpiration <= DateTimeOffset.Now)
                {
                    RefreshAuthToken(configuration.PaymentsFraudAuthUrl, configuration.PaymentsFraudApiKey);
                }
            }

            this.connectTimeout = Int32.Parse(configuration.ConnectTimeout);

            if (!String.IsNullOrEmpty(configuration.ApiKey))
            {
                this.SetApiKey(configuration.ApiKey);
            }
            else
            {
                this.CheckConfigurationParameter(configuration.CertificateFile, nameof(configuration.CertificateFile));
                this.CheckConfigurationParameter(configuration.PrivateKeyPassword, nameof(configuration.PrivateKeyPassword));
                this.SetCertificate(
                    configuration.CertificateFile,
                    configuration.PrivateKeyPassword);
            }

            // KHASH payment encoding is set by default.
            this.SetKhashPaymentEncoding(true);
        }

        /// <summary>
        /// Construct a request object. Set the static setting from the web.config file.
        /// </summary>
        /// <param name="checkConfiguration">By default is true: will check config file if 
        /// <param name="logger">ILogger object for logging output</param>
        /// `Ris.Url`, `Ris.MerchantId`, `Ris.Config.Key` and 
        /// `Ris.Connect.Timeout` are set.</param>
        /// <exception cref="Kount.Ris.RequestException">Thrown when there is
        /// static data missing for a RIS request.</exception>
        protected Request(bool checkConfiguration = true, ILogger logger = null) : this(checkConfiguration, Configuration.FromAppSettings(), logger)
        {
        }

        /// <summary>
        /// Gets hashtable data
        /// </summary>
        protected Hashtable Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// Get the response from the RIS server.
        /// </summary>
        /// <param name="validate">default value is TRUE. If FALSE validate silently doesn't throw exception.</param>
        /// <returns>Kount.Ris.Response populated object.</returns>
        public Kount.Ris.Response GetResponse(bool validate = true)
        {
            logger.LogDebug($"Kount.Ris.Request.GetResponse() - RIS endpoint URL: {this.url}");
            logger.LogDebug($"PTOK [{this.SafeGet("PTOK")}]");
            string ptok = this.Data.ContainsKey("PTOK") ? (string)this.Data["PTOK"] : "";

            if (ptok.Equals("") && "KHASH".Equals((string)this.Data["PENC"]))
            {
                this.Data["PENC"] = "";
            }
           
            string post = "";
            foreach (DictionaryEntry param in this.Data)
            {
                string value = (param.Value != null) ? param.Value.ToString() : string.Empty;

                post = post + HttpUtility.UrlEncode(param.Key.ToString()) +
                    "=" + HttpUtility.UrlEncode(value) + "&";

                if (param.Key.ToString().Equals("PTOK"))
                {
                    value = "payment token hidden";
                };

                logger.LogDebug("[" + param.Key + "]=" + value);
            }

            post = post.TrimEnd('&');
            byte[] buffer = Encoding.ASCII.GetBytes(post);

            // Set up the request object
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(this.url);

            // Instead of forcing specific security protocols make sure
            // that deprecated security protocols are not being used
            AssertSecurityProtocol();

            webReq.Timeout = this.connectTimeout;
            webReq.Method = "POST";
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.ContentLength = buffer.Length;


            if (!_migrationModeEnabled)
            {
                logger.LogDebug("Setting merchant ID header.");
                webReq.Headers[CUSTOM_HEADER_MERCHANT_ID] = this.GetParam("MERC");
                if (null != this.apiKey)
                {
                    logger.LogDebug("Setting API key header.");
                    webReq.Headers[CUSTOM_HEADER_API_KEY] = this.apiKey;
                }
                else
                {
                    logger.LogDebug("API key header not found, setting certificate");
                    //// Add the RIS signed authentication certificate to the payload
                    //// See Kount Technical Specifications Guide for details on
                    //// requesting and exporting
                    //// from your browser
                    X509Certificate2 cert = new X509Certificate2();
                    cert.Import(
                        this.GetCertificateFile(),
                        this.GetPrivateKeyPassword(),
                        X509KeyStorageFlags.MachineKeySet);
                    X509CertificateCollection certs = webReq.ClientCertificates;
                    certs.Add(cert);
                    webReq.ClientCertificates.Add(cert);
                }
            }
            else
            {
                logger.LogDebug("Setting Payments Fraud API key header.");
                _bearerRefreshLock.AcquireReaderLock(TimeSpan.FromMilliseconds(10));
                webReq.Headers[PF_AUTH_HEADER] = $"{_bearerAuthResponse.TokenType} {_bearerAuthResponse.AccessToken}";
                _bearerRefreshLock.ReleaseReaderLock();
            }

            string risString = String.Empty;
            var stopwatch = new Stopwatch();

            // start measure elapsed time between request and response
            stopwatch.Start();
            try
            {
                // Call the RIS server and pass in the payload
                using (Stream postData = webReq.GetRequestStream())
                {
                    postData.Write(buffer, 0, buffer.Length);
                }
            }
            catch (WebException ex)
            {
                string error = String.Empty;
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    error = $"TIMEOUT = {webReq.Timeout}.";
                }
                else
                {
                    if (ex.Response == null)
                    {
                        error = $"Unable to contact server {this.url}. WebEXCEPTION Status = {ex.Status}.";
                    }
                    else
                    {
                        error = this.GetWebError(ex.Response);
                    }
                }
                logger.LogDebug("ERROR - The following web error occurred: " + error);
                throw new Kount.Ris.RequestException(error);
            }

            // stop measure request time 
            stopwatch.Stop();

            using (HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse())
            {
                // Read the RIS response string
                using (Stream answer = webResp.GetResponseStream())
                {
                    if (answer != null)
                    {
                        using (StreamReader risResponse = new StreamReader(answer))
                        {
                            risString = risResponse.ReadToEnd();
                        }
                    }
                    else
                    {
                        throw new Kount.Ris.RequestException("No response from server.");
                    }
                }
            }

            var elapsed = stopwatch.ElapsedMilliseconds.ToString();
            if (logTimeElapsed)
            {
                GetElapsedLogger(elapsed);
                #region Elapsed Logger
                //var builder = new StringBuilder();
                //builder.Append("MERC = ").Append(GetParam("MERC"));
                //builder.Append(" SESS = ").Append(GetParam("SESS"));
                //builder.Append(" SDK_ELAPSED = ").Append(elapsed).Append(" ms.");

                //this.logger.Debug(builder.ToString());
                #endregion

            }

            logger.LogDebug("End GetResponse()");
            return new Kount.Ris.Response(risString, logger);
        }
        

        private void GetElapsedLogger(string elapsed)
        {
            var builder = new StringBuilder();
            builder.Append("MERC = ").Append(GetParam("MERC"));
            builder.Append(" SESS = ").Append(GetParam("SESS"));
            builder.Append(" SDK_ELAPSED = ").Append(elapsed).Append(" ms.");

            logger.LogDebug(builder.ToString());
        }

        private static void AssertSecurityProtocol()
        {
            var securityProtocol = System.Net.ServicePointManager.SecurityProtocol;

            if ((securityProtocol & SecurityProtocolType.Ssl3) == SecurityProtocolType.Ssl3
                || (securityProtocol & SecurityProtocolType.Tls) == SecurityProtocolType.Tls)
            {
                throw new InvalidOperationException("We do not support SSL 3.0 and TLS 1.0. They have been deprecated. Make sure ServicePointManager.SecurityProtocol doesn't include deprecated security protocols. On .NET Framework 4.7 and higher one should prefer SecurityProtocolTyp.SystemDefault to allow the operating system to choose the best protocol to use.");
            }
        }

        /// <summary>
        /// Set parameters in the Response
        /// </summary>
        /// <param name="key">Parameter key</param>
        /// <param name="value">Parameter value</param>
        public void SetParameter(string key, string value)
        {
            this.Data[key] = value;
        }

        /// <summary>
        /// Set the mode of the transaction.
        /// </summary>
        /// <param name="mode">Depends on the request type.</param>
        protected abstract void SetMode(char mode);

        /// <summary>
        /// Set the mode of the transaction.
        /// </summary>
        /// <param name="inquiryType">Depends on the inquiry type.</param>
        public void SetMode(InquiryTypes inquiryType)
        {
            this.SetMode((char)inquiryType);
        }

        /// <summary>
        /// Set the mode of the transaction.
        /// </summary>
        /// <param name="updateType">Depends on the update type.</param>
        public void SetMode(UpdateTypes updateType)
        {
            this.SetMode((char)updateType);
        }

        /// <summary>
        /// Set the merchant Id.
        /// </summary>
        /// <param name="merchantId">Merchant Id.</param>
        public void SetMerchantId(long merchantId)
        {
            this.Data["MERC"] = merchantId;
        }

        /// <summary>
        /// Set the Kount Central customer Id.
        /// </summary>
        /// <param name="customerId">Kount Central customer Id.</param>
        public void SetKountCentralCustomerId(string customerId)
        {
            this.Data["CUSTOMER_ID"] = customerId;
        }

        /// <summary>
        /// Set the session ID of this session.
        /// </summary>
        /// <param name="sessionId">Session Id from the merchant.</param>
        public void SetSessionId(string sessionId)
        {
            this.Data["SESS"] = this.SafeGet(sessionId);
        }

        /// <summary>
        /// Set the merchant order number.
        /// </summary>
        /// <param name="orderNumber">Unique, up to 32 characters.</param>
        public void SetOrderNumber(string orderNumber)
        {
            this.Data["ORDR"] = this.SafeGet(orderNumber);
        }

        /// <summary>
        /// Set the merchant acknowledgement that this product will ship.
        /// </summary>
        /// <param name="mack">Set Y or N.</param>
        public void SetMack(char mack)
        {
            this.Data["MACK"] = mack;
        }

        /// <summary>
        /// Set the auth status of the payment.
        /// </summary>
        /// <param name="auth">Set A or D.</param>
        public void SetAuth(char auth)
        {
            this.Data["AUTH"] = auth;
        }

        /// <summary>
        /// Bankcard AVS ZIP CODE reply.
        /// </summary>
        /// <param name="avsz">M, N, or X.</param>
        public void SetAvsz(char avsz)
        {
            this.Data["AVSZ"] = avsz;
        }

        /// <summary>
        /// Bankcard AVS STREET ADDRESS reply.
        /// </summary>
        /// <param name="avst">M, N, or X.</param>
        public void SetAvst(char avst)
        {
            this.Data["AVST"] = avst;
        }

        /// <summary>
        /// Bankcard CVV/CVC/CVV2 reply.
        /// </summary>
        /// <param name="cvvr">M, N, or X.</param>
        public void SetCvvr(char cvvr)
        {
            this.Data["CVVR"] = cvvr;
        }

        /// <summary>
        /// Sets a card payment and masks the card number in the following way: <br/>
	    /// First 6 characters remain as they are, following characters up to the last 4 are
	    /// replaced with the 'X' character, last 4 characters remain as they are.
	    /// If the provided Payment parameter is not a card payment, standard encoding
	    /// will be applied.
        /// </summary>
        /// <example> card number 0007380568572514 is masked to 000738XXXXXX2514 </example>
        /// <param name="cardNumber">Raw credit card number</param>
        public void SetCardPaymentMasked(string cardNumber)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Card.GetValueAsString();
            this.Data["PENC"] = "MASK";

            string ptok = MaskToken(cardNumber);
            this.SetPaymentToken(this.SafeGet(ptok));
        }

        /// <summary>
        /// Set No Payment.
        /// </summary>
        public void SetNoPayment()
        {
            this.Data["PTYP"] = Enums.PaymentTypes.None.GetValueAsString();
            this.Data["PTOK"] = "";
        }

        /// <summary>
        /// Set a payment type and payment token. This method is Obsoleted.
        /// </summary>
        /// <param name="ptyp">Payment Type</param>
        /// <param name="ptok">Payment Token</param>
        [Obsolete("Version 6.5.0 Use Kount.Ris.Request.SetPayment(Enums.PaymentTypes paymentType, string payerId) : void")]
        public void SetPayment(string ptyp, string ptok)
        {
            logger.LogDebug("Kount.Ris.Request.SetPayment()");
            this.Data["PTYP"] = ptyp;
            this.SetPaymentToken(this.SafeGet(ptok));
        }

        /// <summary>
        /// Set a payment 
        /// </summary>
        /// <param name="paymentType">Payment Type</param>
        /// <param name="payerId">Payment Token</param>
        public void SetPayment(Enums.PaymentTypes paymentType, string payerId)
        {
            switch (paymentType)
            {
                case Enums.PaymentTypes.Apple:
                    this.SetApplePayment(payerId);
                    break;
                case Enums.PaymentTypes.Blml:
                    this.SetBillMeLaterPayment(payerId);
                    break;
                case Enums.PaymentTypes.Bpay:
                    this.SetBpayPayment(payerId);
                    break;
                case Enums.PaymentTypes.Card:
                    this.SetCardPayment(payerId);
                    break;
                case Enums.PaymentTypes.CarteBleue:
                    this.SetCarteBleuePayment(payerId);
                    break;
                case Enums.PaymentTypes.Check:
                    this.SetCheckPayment(payerId);
                    break;
                case Enums.PaymentTypes.Elv:
                    this.SetElvPayment(payerId);
                    break;
                case Enums.PaymentTypes.GreenDotMoneyPak:
                    this.SetGreenDotMoneyPakPayment(payerId);
                    break;
                case Enums.PaymentTypes.GiftCard:
                    this.SetGiftCardPayment(payerId);
                    break;
                case Enums.PaymentTypes.GiroPay:
                    this.SetGiroPayPayment(payerId);
                    break;
                case Enums.PaymentTypes.Google:
                    this.SetGooglePayment(payerId);
                    break;
                case Enums.PaymentTypes.Interac:
                    this.SetInteracPayment(payerId);
                    break;
                case Enums.PaymentTypes.MercadePago:
                    this.SetMercadePagoPayment(payerId);
                    break;
                case Enums.PaymentTypes.Neteller:
                    this.SetNetellerPayment(payerId);
                    break;
                case Enums.PaymentTypes.None:
                    this.SetNoPayment();
                    break;
                case Enums.PaymentTypes.Poli:
                    this.SetPoliPayment(payerId);
                    break;
                case Enums.PaymentTypes.Paypal:
                    this.SetPaypalPayment(payerId);
                    break;
                case Enums.PaymentTypes.SingleEuroPaymentsArea:
                    this.SetSepaPayment(payerId);
                    break;
                case Enums.PaymentTypes.Skrill:
                    this.SetSkrillPayment(payerId);
                    break;
                case Enums.PaymentTypes.Sofort:
                    this.SetSofortPayment(payerId);
                    break;
                case Enums.PaymentTypes.Token:
                    this.SetTokenPayment(payerId);
                    break;
                default:
                    throw new RequestException($"ERROR: such payment type not exist: {paymentType.ToString()}");
            }
        }

        /// <summary>
        /// Set a Green Dot MoneyPak payment.
        /// </summary>
        /// <param name="id">Green Dot MoneyPak payment ID number</param>
        public void SetGreenDotMoneyPakPayment(string id)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.GreenDotMoneyPak.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(id));
        }

        /// <summary>
        /// Get value from Data - Hashtable.
        /// </summary>
        /// <param name="param">Key string in hashtable</param>
        /// <returns></returns>
        public string GetParam(string param)
        {
            if (String.IsNullOrEmpty(param))
            {
                return String.Empty;
            }

            string res = this.Data[param] as string;
            if (res == null)
            {
                var val = this.Data[param] as int?;
                if (val.HasValue)
                {
                    res = val.Value.ToString();
                }
                var val2 = this.Data[param] as long?;
                if (val2.HasValue)
                {
                    res = val2.Value.ToString();
                }
            }
            return res ?? String.Empty;
        }

        /// <summary>
        /// Get the URL of the target RIS server.
        /// </summary>
        /// <returns>String of the target url.</returns>
        public string GetUrl()
        {
            return this.url;
        }

        /// <summary>
        /// Set the URL of the target RIS server.
        /// </summary>
        /// <param name="url">String of the target RIS server.</param>
        public void SetUrl(string url)
        {
            this.url = url;
        }
        
        /// <summary>
        /// Get the Connect Timeout.
        /// </summary>
        /// <returns>Connect Timeout</returns>
        public int GetConnectTimeOut()
        {
            return this.connectTimeout;
        }

        /// <summary>
        /// Set the Connect Timeout .
        /// </summary>
        /// <param name="connectTimeout">Connect Timeout.</param>
        public void SetConnectTimeOut(int connectTimeout)
        {
            this.connectTimeout = connectTimeout;
        }

        /// <summary>
        /// Set the RIS certificate information.
        /// </summary>
        /// <param name="certificate">Full path of the certificate pk12 or
        /// pfx file.</param>
        /// <param name="password">Password used to export the certificate.
        /// </param>
        public void SetCertificate(string certificate, string password)
        {
            this.certificate = certificate;
            this.password = password;
        }

        /// <summary>
        /// Set the API key.
        /// </summary>
        /// <param name="key">Key used to authenticate.</param>
        public void SetApiKey(string key)
        {
            this.apiKey = key;
        }

        /// <summary>
        /// Get the certificate file path.
        /// </summary>
        /// <returns>String of the certificate file path.</returns>
        public string GetCertificateFile()
        {
            return this.certificate;
        }

        /// <summary>
        /// Private key password used to export the certificate file.
        /// </summary>
        /// <returns>String of the certificate export password.</returns>
        public string GetPrivateKeyPassword()
        {
            return this.password;
        }

        /// <summary>
        /// Set the RIS payment encoding to KHASH.
        /// </summary>
        [Obsolete("Version 5.0.0. Use Kount.Ris.Request.SetKhashPaymentEncoding(bool) : void")]
        public void SetKhashPaymentEncoding()
        {
            string message = "The method " +
                "Kount.Ris.Request.SetKhashPaymentEncoding() is obsolete. " +
                "Use Kount.Ris.Request.SetKhashPaymentEncoding(bool) instead.";
            logger.LogInformation(message);
            this.Data["PENC"] = "KHASH";
        }

        /// <summary>
        /// Set the RIS payment encoding to KHASH.
        /// </summary>
        /// <param name="enabled">TRUE when enabled</param>
        public void SetKhashPaymentEncoding(bool enabled)
        {
            if (enabled)
            {
                this.Data["PENC"] = "KHASH";
            }
            else
            {
                this.Data["PENC"] = "";
            }
        }

        /// <summary>
        /// Set the last 4 characters of the payment token.
        /// </summary>
        /// <param name="last4">Last 4 characters</param>
        public void SetPaymentTokenLast4(string last4)
        {
            this.Data["LAST4"] = last4;
        }

        /// <summary>
        /// Set the version of the RIS response.
        /// </summary>
        /// <param name="version">Response version.</param>
        public void SetVersion(string version)
        {
            this.Data["VERS"] = version;
        }

        /// <summary>
        /// Check if KHASH payment encoding has been set.
        /// </summary>
        /// <returns>TRUE when set</returns>
        protected bool IsSetKhashPaymentEncoding()
        {
            return this.Data.ContainsKey("PENC") &&
                "KHASH".Equals(this.Data["PENC"]);
        }

        /// <summary>
        /// Set the payment token.
        /// </summary>
        /// <param name="token">Payment token</param>
        protected void SetPaymentToken(string token)
        {
            string raw = token;
            if (null != token && !this.Data.Contains("LAST4"))
            {
                if (token.Length > 4)
                {
                    this.Data["LAST4"] = token.Substring(token.Length - 4);
                }
                else
                {
                    this.Data["LAST4"] = token;
                }
            }

            if (this.IsSetKhashPaymentEncoding())
            {
                token = ("GIFT".Equals(this.Data["PTYP"])) ?
                    Khash.HashGiftCard((long)this.Data["MERC"], token) :
                    Khash.HashPaymentToken(token);
            }

            this.Data["PTOK"] = token;
        }

        /// <summary>
        /// Check configuration parameters for existence in application
        /// configuration.
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="parameter">Parameter name</param>
        /// <exception cref="Kount.Ris.RequestException">Thrown when parameter
        /// is missing</exception>
        protected void CheckConfigurationParameter(string value, string parameter)
        {
            if (null == value)
            {
                logger.LogError($"Configuration parameter [{parameter}] not defined.");
                throw new Kount.Ris.RequestException(
                    $"[{parameter}] must be defined in the application configuration file.");
            }
        }

        /// <summary>
        /// Sanitize a variable before return it.
        /// </summary>
        /// <param name="var">Raw variable</param>
        /// <returns>Sanitized variable</returns>
        protected string SafeGet(string var)
        {
            return (null == var) ? "" : var;
        }

        /// <summary>
        /// Set a Apple payment.
        /// </summary>
        /// <param name="appleId">Apple payer ID</param>
        private void SetApplePayment(string appleId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Apple.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(appleId));
        }

        /// <summary>
        /// Set a Bpay payment.
        /// </summary>
        /// <param name="bpayId">Bpay payer ID</param>
        private void SetBpayPayment(string bpayId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Bpay.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(bpayId));
        }

        /// <summary>
        /// Set a CarteBleue payment.
        /// </summary>
        /// <param name="carteBleueId">CarteBleue payer ID</param>
        private void SetCarteBleuePayment(string carteBleueId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.CarteBleue.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(carteBleueId));
        }

        /// <summary>
        /// Set a Elv payment.
        /// </summary>
        /// <param name="elvId">Elv payer ID</param>
        private void SetElvPayment(string elvId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Elv.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(elvId));
        }

        /// <summary>
        /// Set a GiroPay payment.
        /// </summary>
        /// <param name="giroPayId">GiroPay payer ID</param>
        private void SetGiroPayPayment(string giroPayId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.GiroPay.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(giroPayId));
        }

        /// <summary>
        /// Set a Interac payment.
        /// </summary>
        /// <param name="interacId">Interac payer ID</param>
        private void SetInteracPayment(string interacId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Interac.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(interacId));
        }

        /// <summary>
        /// Set a MercadePago payment.
        /// </summary>
        /// <param name="mercadePagoId">MercadePago payer ID</param>
        private void SetMercadePagoPayment(string mercadePagoId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.MercadePago.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(mercadePagoId));
        }

        /// <summary>
        /// Set a Neteller payment.
        /// </summary>
        /// <param name="netellerId">Neteller payer ID</param>
        private void SetNetellerPayment(string netellerId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Neteller.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(netellerId));
        }

        /// <summary>
        /// Set a Poli payment.
        /// </summary>
        /// <param name="poliId">Poli payer ID</param>
        private void SetPoliPayment(string poliId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Poli.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(poliId));
        }

        /// <summary>
        /// Set a Sepa payment.
        /// </summary>
        /// <param name="sepaId">Sepa payer ID</param>
        private void SetSepaPayment(string sepaId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.SingleEuroPaymentsArea.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(sepaId));
        }

        /// <summary>
        /// Set a Skrill payment.
        /// </summary>
        /// <param name="skrillId">Skrill payer ID</param>
        private void SetSkrillPayment(string skrillId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Skrill.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(skrillId));
        }

        /// <summary>
        /// Set a Sofort payment.
        /// </summary>
        /// <param name="sofortId">Sofort payer ID</param>
        private void SetSofortPayment(string sofortId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Sofort.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(sofortId));
        }

        /// <summary>
        /// Set a Token payment.
        /// </summary>
        /// <param name="tokenId">Token payer ID</param>
        private void SetTokenPayment(string tokenId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Token.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(tokenId));
        }

        /// <summary>
        /// Set a Paypal payment.
        /// </summary>
        /// <param name="paypalId">Paypal payer ID</param>
        private void SetPaypalPayment(string paypalId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Paypal.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(paypalId));
        }

        /// <summary>
        /// Set a google payment
        /// </summary>
        /// <param name="googleId">Google pay id</param>
        private void SetGooglePayment(string googleId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Google.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(googleId));
        }

        /// <summary>
        /// Set a check payment.
        /// </summary>
        /// <param name="micr">Micro number on the check.</param>
        private void SetCheckPayment(string micr)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Check.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(micr));
        }

        /// <summary>
        /// Set a Bill Me Later payment.
        /// </summary>
        /// <param name="blmlId">bill me later id</param>
        private void SetBillMeLaterPayment(string blmlId)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Blml.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(blmlId));
        }

        /// <summary>
        /// Set a credit card payment
        /// </summary>
        /// <param name="cardNumber">Raw credit card number</param>
        private void SetCardPayment(string cardNumber)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.Card.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(cardNumber));
        }

        /// <summary>
        /// Set a gift card payment
        /// </summary>
        /// <param name="giftCardNum">Gift card number</param>
        private void SetGiftCardPayment(string giftCardNum)
        {
            this.Data["PTYP"] = Enums.PaymentTypes.GiftCard.GetValueAsString();
            this.SetPaymentToken(this.SafeGet(giftCardNum));
        }

        /// <summary>
        /// Set the Bank Identification Number.
        /// </summary>
        /// <param name="lbin">Bank Identification Number.</param>
        public void SetLbin(string lbin)
        {
            this.Data["LBIN"] = lbin;
        }
       

        /// <summary>
        /// Get error description from webException
        /// </summary>
        /// <param name="exResponse">Response from web exception</param>
        /// <returns></returns>
        private string GetWebError(WebResponse exResponse)
        {
            string error = String.Empty;
            using (HttpWebResponse resp = exResponse as HttpWebResponse)
            {
                error = String.Concat(((int)resp.StatusCode).ToString(), ": ", resp.StatusDescription);
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.Unauthorized://401 
                        error = "Unable to log in. Unauthorized request.(401)";
                        break;

                    case HttpStatusCode.InternalServerError://500
                        error = "Unable to log in. There was an error logging in.(500)";
                        break;

                    case HttpStatusCode.NotImplemented://501 
                        error = "Unable to log in. Unauthorized request(using certificate).(501)";
                        break;

                    case HttpStatusCode.NotFound://404
                        error = "Unable to connect. The service was not available.(404)";
                        break;

                    case HttpStatusCode.ServiceUnavailable://503 
                        error = "Unable to connect. The service was not available.(503)";
                        break;

                    case HttpStatusCode.GatewayTimeout://504 
                        error = "Unable to connect. Timeout request.(504)";
                        break;

                    default:
                        break;
                }
            }

            return error;
        }

        /// <summary>
        /// Encodes the provided payment token according to the MASK encoding scheme
        /// </summary>
        /// <param name="token">CARD token</param>
        /// <returns>masked token</returns>
        private static string MaskToken(string token)
        {
            var builder = new StringBuilder();

            builder.Append(token.Substring(0, 6));
            for (int i = 6; i < (token.Length - 4); i++)
            {
                builder.Append('X');
            }

            builder.Append(token.Substring(token.Length - 4));

            return builder.ToString();
        }
        

        /// <summary>
        /// Fetch data parameters in arrays
        /// </summary>
        /// <param name="data">The data hashtable</param>
        /// <returns>A hashtable of array data</returns>
        private Hashtable FetchArrayParams(Hashtable data)
        {
            Hashtable arrayParams = new Hashtable();
            ArrayList prod_type = new ArrayList();
            ArrayList prod_item = new ArrayList();
            ArrayList prod_desc = new ArrayList();
            ArrayList prod_quant = new ArrayList();
            ArrayList prod_price = new ArrayList();

            foreach (string key in data.Keys)
            {
                if (key.StartsWith("PROD_TYPE"))
                {
                    prod_type.Add(key);
                }
                else if (key.StartsWith("PROD_ITEM"))
                {
                    prod_item.Add(key);
                }
                else if (key.StartsWith("PROD_DESC"))
                {
                    prod_desc.Add(key);
                }
                else if (key.StartsWith("PROD_QUANT"))
                {
                    prod_quant.Add(key);
                }
                else if (key.StartsWith("PROD_PRICE"))
                {
                    prod_price.Add(key);
                }
            }

            arrayParams.Add("PROD_TYPE", prod_type);
            arrayParams.Add("PROD_ITEM", prod_item);
            arrayParams.Add("PROD_DESC", prod_desc);
            arrayParams.Add("PROD_QUANT", prod_quant);
            arrayParams.Add("PROD_PRICE", prod_price);

            return arrayParams;
        }

        /// <summary>
        /// Refresh the bearer token
        /// </summary>
        /// <param name="authUrl"></param>
        /// <param name="apiKey"></param>
        /// <exception cref="RequestException"></exception>
        private static void RefreshAuthToken(string authUrl, string apiKey)
        {
            _bearerRefreshLock.AcquireWriterLock(TimeSpan.FromSeconds(30));
            
            // short circuit if another thread as already refreshed the token
            if (_bearerAuthResponseExpiration >= DateTimeOffset.Now)
            {
                _bearerRefreshLock.ReleaseWriterLock();
                return;
            }
            
            string tokenUrl = authUrl + "?grant_type=client_credentials&scope=k1_integration_api";
            
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(tokenUrl);
            webReq.Method = "POST";
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.Headers[PF_AUTH_HEADER] = "Basic " + apiKey;


            string responseString = string.Empty;
            using (HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse())
            {
                // Read the token response string
                using (Stream responseStream = webResp.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader tokenResponse = new StreamReader(responseStream))
                        {
                            responseString = tokenResponse.ReadToEnd();
                        }
                    }
                }
            }
            
            BearerAuthResponse authResponse = JsonSerializer.Deserialize<BearerAuthResponse>(responseString);
            if (authResponse != null && authResponse.ExpiresIn != 0)
            {
                _bearerRefreshLock.AcquireReaderLock(TimeSpan.FromSeconds(5));
                _bearerAuthResponseExpiration = DateTime.Now.AddSeconds(authResponse.ExpiresIn);
                _bearerAuthResponse = authResponse;
                _bearerRefreshLock.ReleaseReaderLock();
            }
            else
            {
                _bearerRefreshLock.ReleaseWriterLock();
                throw new Kount.Ris.RequestException("Failed to update the bearer token invalid response");
            }
            
            _bearerRefreshLock.ReleaseWriterLock();
        }
    }
}