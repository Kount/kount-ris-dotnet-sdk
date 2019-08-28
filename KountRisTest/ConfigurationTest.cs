using Kount.Ris;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KountRisTest
{
    [TestClass]
    public class ConfigurationTest
    {
        public Configuration SUT;

        [TestInitialize]
        public void Setup()
        {
            SUT = Configuration.FromAppSettings();
        }

        [TestMethod]
        public void FromAppSettings_assigns_Connect_Timeout()
        {
            Assert.AreEqual(SUT.ConnectTimeout, "10000");
        }

        [TestMethod]
        public void FromAppSettings_assigns_MerchantId()
        {
            Assert.AreEqual(SUT.MerchantId, "000000");
        }

        [TestMethod]
        public void FromAppSettings_assigns_API_Key()
        {
            Assert.AreEqual(SUT.ApiKey, "");
        }

        [TestMethod]
        public void FromAppSettings_assigns_Version()
        {
            Assert.AreEqual(SUT.Version, "0700");
        }

        [TestMethod]
        public void FromAppSettings_assigns_Url()
        {
            Assert.AreEqual(SUT.URL, "https://risk.beta.kount.net");
        }

        [TestMethod]
        public void FromAppSettings_assigns_CertificateFile()
        {
            Assert.AreEqual(SUT.CertificateFile, "certificate.pfx");
        }

        [TestMethod]
        public void FromAppSettings_assigns_PrivateKeyPassword()
        {
            Assert.AreEqual(SUT.PrivateKeyPassword, "11111111111111111");
        }

        [TestMethod]
        public void FromAppSettings_assigns_ConfigKey()
        {
            Assert.AreEqual(SUT.ConfigKey, null);
        }
    }
}
