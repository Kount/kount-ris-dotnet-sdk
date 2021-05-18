using Microsoft.Extensions.Configuration;
using System.Configuration;
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
            if (ConfigurationManager.AppSettings.AllKeys.Length != 0 && ConfigurationManager.AppSettings["Ris.MerchantId"] != null && ConfigurationManager.AppSettings["Ris.API.Key"] != null)
            {           

                config = new Configuration()
                {
                    MerchantId = ConfigurationManager.AppSettings["Ris.MerchantId"],
                    URL = ConfigurationManager.AppSettings["Ris.Url"],
                    ConfigKey = ConfigurationManager.AppSettings["Ris.Config.Key"],
                    ConnectTimeout = ConfigurationManager.AppSettings["Ris.Connect.Timeout"],
                    Version = ConfigurationManager.AppSettings["Ris.Version"],
                    ApiKey = ConfigurationManager.AppSettings["Ris.API.Key"],
                    CertificateFile = ConfigurationManager.AppSettings["Ris.CertificateFile"],
                    PrivateKeyPassword = ConfigurationManager.AppSettings["Ris.PrivateKeyPassword"],
                    LogSimpleElapsed = ConfigurationManager.AppSettings["LOG.SIMPLE.ELAPSED"]
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
                    CertificateFile = configuration.GetConnectionString("Ris.CertificateFile"),
                    PrivateKeyPassword = configuration.GetConnectionString("Ris.PrivateKeyPassword"),
                };
            }        

            return config;
        }

        /// <summary>
        /// Six digit identifier issued by Kount.
        /// </summary>
        public string MerchantId { get; set; }

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
    }
}