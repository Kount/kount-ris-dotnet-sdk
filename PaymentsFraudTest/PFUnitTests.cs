namespace PaymentsFraudTest 
{
    using Kount.Ris;
    using Xunit;    

    public class PFUnitTests
    {
        private Configuration _config = Configuration.FromAppSettings();

        [Fact]
        public void FromAppSettings_assigns_FraudClientId()
        {
            var clientId = _config.PaymentsFraudClientId?.Trim();
            Assert.NotNull(clientId);
            Assert.True(clientId.Length == 6 || clientId.Length == 15);
        }

        [Fact]
        public void FromAppSettings_assigns_API_Key()
        {
            Assert.NotNull(_config.PaymentsFraudApiKey);
        }

        [Fact]
        public void FromAppSettings_assigns_API_Url()
        {
            Assert.NotNull(_config.PaymentsFraudApiUrl);
        }

        [Fact]
        public void FromAppSettings_assigns_Auth_Url()
        {
            Assert.NotNull(_config.PaymentsFraudAuthUrl);
        }
        
        [Fact]
        public void FromAppSettings_valid_Migration_Mode()
        {
            Assert.NotNull(_config.PaymentsFraudAuthUrl);
            
            _config.EnableMigrationMode = "true";
            Assert.True(_config.GetEnableMigrationMode());
            
            _config.EnableMigrationMode = "false";
            Assert.False(_config.GetEnableMigrationMode());
            
            _config.EnableMigrationMode = "asdf1234";
            Assert.False(_config.GetEnableMigrationMode());
            
            _config.EnableMigrationMode = "";
            Assert.False(_config.GetEnableMigrationMode());
        }
    }
}