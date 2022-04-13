
namespace KountRisTest
{
    using Kount.Ris;
    using Microsoft.Extensions.Configuration;
    using System.Configuration;
    using System.IO;
    using Xunit;



    public class ConfigurationTest
    {
        public Kount.Ris.Configuration SUT;

        
        public ConfigurationTest()
        {        

            SUT = Kount.Ris.Configuration.FromAppSettings();
        }

         [Fact]
        public void FromAppSettings_assigns_Connect_Timeout()
        {
            Assert.Equal("10000", SUT.ConnectTimeout);
        }

         [Fact]
        public void FromAppSettings_assigns_MerchantId()
        {
            Assert.NotNull(SUT.MerchantId);
        }

         [Fact]
        public void FromAppSettings_assigns_API_Key()
        {
            Assert.NotNull(SUT.ApiKey);
        }

         [Fact]
        public void FromAppSettings_assigns_Version()
        {
            Assert.Equal("0720", SUT.Version);
        }

         [Fact]
        public void FromAppSettings_assigns_Url()
        {
            Assert.Equal("https://risk.test.kount.net", SUT.URL);
        }

         [Fact]
        public void FromAppSettings_assigns_CertificateFile()
        {
            Assert.Equal("certificate.pfx", SUT.CertificateFile);
        }

         [Fact]
        public void FromAppSettings_assigns_PrivateKeyPassword()
        {
            Assert.Equal("11111111111111111", SUT.PrivateKeyPassword);
        }

         [Fact]
        public void FromAppSettings_assigns_ConfigKey()
        {
            Assert.NotNull(SUT.ConfigKey);
        }
    }
}