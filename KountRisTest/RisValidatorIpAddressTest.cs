//-----------------------------------------------------------------------
// <copyright file="RisValidatorIPADTest.cs" company="Keynetics Inc">
//     Copyright  Kount Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace KountRisTest
{
    using Kount.Ris;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test RisValidator for IPv4 and IPv6 address validity.
    /// </summary>
    [TestClass]
    public class RisValidatorIpAddressTest
    {
        //Fields
        private string _sid = "";
        private string _orderNum = "";
        private Inquiry inquiry;

        [TestInitialize]
        public void Initialize()
        {
            this.inquiry = TestHelper.DefaultInquiry(out _sid, out _orderNum);
            this.inquiry.SetNoPayment();
        }

        #region IPv6

        [TestMethod]
        public void TestPassingIpv6_ValidWithNoCollapse()
        {
            // Arrange
            // Shows a 128-bit address in eight 16-bit blocks in the format global:subnet:interface.
            inquiry.SetIpAddress("FE80:0000:0000:0000:0202:B3FF:FE1E:8329");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects no exception
        }
        
        [TestMethod]
        public void TestPassingIpv6_ValidWithCollapse()
        {
            // Arrange
            // The :: (consecutive colons) notation can be used to represent four successive 16-bit blocks that contain zeros.
            inquiry.SetIpAddress("FE80::0202:B3FF:FE1E:8329");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects no exception
        }

        [TestMethod]
        public void TestPassingIpv6_LoopbackCompressed()
        {
            // Arrange
            // loopback, compressed, non-routable 
            inquiry.SetIpAddress("::1");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        public void TestPassingIpv6_IPv4AddressesAsDottedQuads()
        {
            // Arrange
            // IPv4 addresses as dotted-quads
            inquiry.SetIpAddress("1:2:3:4:5:6:1.2.3.4");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        public void TestPassingIpv6_IPv4MappedIPv6Address()
        {
            // Arrange
            // IPv4-mapped IPv6 address, 45 characters - max length 
            inquiry.SetIpAddress("ABCD:ABCD:ABCD:ABCD:ABCD:ABCD:192.168.158.190");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        public void TestPassingIpv6_IPv4MappedIPv6AddressCompressed()
        {
            // Arrange
            // IPv4-mapped IPv6 address, compressed 
            inquiry.SetIpAddress("::FFFF:129.144.52.38");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv6_NotAllowedCharacters()
        {
            // Arrange
            // Characters that are not allowed
            inquiry.SetIpAddress("2001:0:3238:mech:63::FEFB");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv6_ExtraZerroNotAllowed()
        {
            // Arrange
            // extra 0 not allowed
            inquiry.SetIpAddress("02001:0000:1234:0000:0000:C1C0:ABCD:0876");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv6_JunkAfterValidAddress()
        {
            // Arrange
            // junk after valid address
            inquiry.SetIpAddress("2001:0000:1234:0000:0000:C1C0:ABCD:0876  0");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv6_InternalSpace()
        {
            // Arrange
            // internal space
            inquiry.SetIpAddress("2001:0000:1234:0000:0000:C1C0:ABCD:0876  0");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv6_SevenSegments()
        {
            // Arrange
            // seven segments
            inquiry.SetIpAddress("3ffe:0b00:0000:0001:0000:0000:000a");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv6_NineSegments()
        {
            // Arrange
            // Nine segments
            inquiry.SetIpAddress("FF02:0000:0000:0000:0000:0000:0000:0000:0001");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv6_DoubleColons()
        {
            // Arrange
            // double "::"
            inquiry.SetIpAddress("3ffe:b00::1::a");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv6_DoubleColonsStartAndEnd()
        {
            // Arrange
            // double "::"
            inquiry.SetIpAddress("::1111:2222:3333:4444:5555:6666::");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        #endregion

        #region IPv4
        
        [TestMethod]
        public void TestPassingIpv4Local()
        {
            // Arrange
            inquiry.SetIpAddress("192.168.100.200");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects no exception
        }
        
        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv4Local()
        {
            // Arrange
            inquiry.SetIpAddress("192.1.100.2048");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }
        
        [TestMethod]
        public void TestPassingIpv4()
        {
            // Arrange
            inquiry.SetIpAddress("8.8.8.8");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects no exception
        }
        
        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv4_ThreeSegments()
        {
            // Arrange
            // Three segments
            inquiry.SetIpAddress("8.8.8");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(Kount.Ris.ValidationException))]
        public void TestFailingIpv4_OutOfRange()
        {
            // Arrange
            // 256 is out of range
            inquiry.SetIpAddress("127.0.0.256");

            // Act
            Response response = inquiry.GetResponse();

            // Assert - Expects exception
        }

        #endregion
    }
}
