using Microsoft.Extensions.Configuration;
using systemConfiguaration =System.Configuration;
using System.IO;

namespace Kount.Ris
{
    /// <summary>
    /// Containing configuration values
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets configuration values from app settings.
        /// </summary>
        /// <returns>Configuration class with raw values.</returns>
        public static Configuration FromAppSettings()
        {
            var config = new Configuration();
            if (systemConfiguaration.ConfigurationManager.AppSettings.AllKeys.Length != 0 && systemConfiguaration.ConfigurationManager.AppSettings["Ris.MerchantId"] != null && systemConfiguaration.ConfigurationManager.AppSettings["Ris.API.Key"] != null)
            {
                
                config = new Configuration()
                {
                    MerchantId = systemConfiguaration.ConfigurationManager.AppSettings["Ris.MerchantId"],
                    URL = systemConfiguaration.ConfigurationManager.AppSettings["Ris.Url"],
                    ConfigKey = systemConfiguaration.ConfigurationManager.AppSettings["Ris.Config.Key"],
                    ConnectTimeout = systemConfiguaration.ConfigurationManager.AppSettings["Ris.Connect.Timeout"],
                    Version = systemConfiguaration.ConfigurationManager.AppSettings["Ris.Version"],
                    ApiKey = systemConfiguaration.ConfigurationManager.AppSettings["Ris.API.Key"],
                    EnableMigrationMode = systemConfiguaration.ConfigurationManager.AppSettings["Ris.EnableMigrationMode"],
                    CertificateFile = systemConfiguaration.ConfigurationManager.AppSettings["Ris.CertificateFile"],
                    PrivateKeyPassword = systemConfiguaration.ConfigurationManager.AppSettings["Ris.PrivateKeyPassword"],
                    LogSimpleElapsed = systemConfiguaration.ConfigurationManager.AppSettings["LOG.SIMPLE.ELAPSED"],
                    PaymentsFraudApiKey = systemConfiguaration.ConfigurationManager.AppSettings["PaymentsFraud.Api.Key"],
                    PaymentsFraudApiUrl = systemConfiguaration.ConfigurationManager.AppSettings["PaymentsFraud.Api.Url"],
                    PaymentsFraudClientId = systemConfiguaration.ConfigurationManager.AppSettings["PaymentsFraud.ClientId"],
                    PaymentsFraudAuthUrl = systemConfiguaration.ConfigurationManager.AppSettings["PaymentsFraud.Auth.Url"]
                };
            }
            else if (File.Exists("appsettings.json"))
            {
                var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();

                config = new Configuration()
                {
                    MerchantId = configuration.GetConnectionString("Ris.MerchantId"),
                    URL = configuration.GetConnectionString("Ris.Url"),
                    ConfigKey = configuration.GetConnectionString("Ris.Config.Key"),
                    ConnectTimeout = configuration.GetConnectionString("Ris.Connect.Timeout"),
                    Version = configuration.GetConnectionString("Ris.Version"),
                    ApiKey = configuration.GetConnectionString("Ris.API.Key"),
                    EnableMigrationMode = configuration.GetConnectionString("Ris.EnableMigrationMode"),
                    CertificateFile = configuration.GetConnectionString("Ris.CertificateFile"),
                    PrivateKeyPassword = configuration.GetConnectionString("Ris.PrivateKeyPassword"),
                    PaymentsFraudApiKey = configuration.GetConnectionString("PaymentsFraud.Api.Key"),
                    PaymentsFraudApiUrl = configuration.GetConnectionString("PaymentsFraud.Api.Url"),
                    PaymentsFraudClientId = configuration.GetConnectionString("PaymentsFraud.ClientId"),
                    PaymentsFraudAuthUrl = configuration.GetConnectionString("PaymentsFraud.Auth.Url")
                };
            }        
            
            if (string.IsNullOrEmpty(config.EnableMigrationMode))
            {
                config.EnableMigrationMode = "false";
            }
            if (string.IsNullOrEmpty(config.MerchantId))
            {
                config.MerchantId = "1234567";
            }

            return config;
        }

        /// <summary>
        /// Six digit identifier issued by Kount.
        /// </summary>
        public string MerchantId { get; set; } = "1234567";

        /// <summary>
        /// HTTPS URL path to the company's servers provided in boarding documentation from Kount.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Config Key used in hashing method.
        /// </summary>
        public string ConfigKey { get; set; }

        /// <summary>
        /// RIS connect timeout value measured in milliseconds.
        /// </summary>
        public string ConnectTimeout { get; set; }

        /// <summary>
        /// RIS version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// API Key value from API Key page within Agent Web Console.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Full path of the certificate pk12 or pfx file.
        /// </summary>
        public string CertificateFile { get; set; }

        /// <summary>
        /// Password used to export the certificate
        /// </summary>
        public string PrivateKeyPassword { get; set; }

        /// <summary>
        /// Read LogElapsedTime from config
        /// </summary>
        public string LogSimpleElapsed { get; set; }

        /// <summary>
        ///  Boolean value to enable migration mode
        /// </summary>
        public string EnableMigrationMode { get; set; } = "false";
        
        /// <summary>
        /// Api Key for Payments Fraud on Kount 360
        /// </summary>
        public string PaymentsFraudApiKey { get; set; } = string.Empty;
        
        /// <summary>
        /// Url for Payments Fraud on Kount 360 RIS compatible endpoint
        /// </summary>
        public string PaymentsFraudApiUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Auth Url for Payments Fraud on Kount 360 RIS compatible endpoint
        /// </summary>
        public string PaymentsFraudAuthUrl { get; set; } = string.Empty;

        /// <summary>
        /// Client ID for Payments Fraud on Kount 360
        /// </summary>
        public string PaymentsFraudClientId { get; set; } = string.Empty;
        
        /// <summary>
        /// Coerce EnableMigrationMode to a boolean and return it
        /// </summary>
        /// <returns></returns>
        public bool GetEnableMigrationMode()
        {
            if(bool.TryParse(EnableMigrationMode, out bool result))
            {
                return result;
            }
            
            return false;
        }
    }
}