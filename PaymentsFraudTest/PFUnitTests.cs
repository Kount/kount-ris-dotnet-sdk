namespace PaymentsFraudTest 
{
    using Kount.Ris;
    using Xunit;    

    public class PFUnitTests
    {
        [Fact]
        public void FromAppSettings_valid_Migration_Mode()
        {
            Configuration config = Configuration.FromAppSettings();
            
            config.EnableMigrationMode = "true";
            Assert.True(config.GetEnableMigrationMode());
            
            config.EnableMigrationMode = "false";
            Assert.False(config.GetEnableMigrationMode());
            
            config.EnableMigrationMode = "asdf1234";
            Assert.False(config.GetEnableMigrationMode());
            
            config.EnableMigrationMode = "";
            Assert.False(config.GetEnableMigrationMode());
        }
    }
}