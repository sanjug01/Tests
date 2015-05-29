namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ValidationRules;
    using System.Text;

    [TestClass]
    public class GatewayEqualityComparerTests
    {
        private GatewayModel _testGateway;
        private GatewayEqualityComparer _comparer;

        private readonly string TEST_NAME               = "TestGateway_aBc123";
        private readonly string TEST_NAME_CHANGED_CASE  = "tESTGateWAY_AbC123";

        [TestInitialize]
        public void TestSetup()
        {
            _testGateway = new GatewayModel()
            {
                HostName = TEST_NAME
            };

            _comparer = new GatewayEqualityComparer();
        }

        [TestMethod]
        public void VerifyEqualsGatewayHostname()
        {
            Assert.IsTrue(_comparer.Equals(_testGateway, TEST_NAME) );
        }

        [TestMethod]
        public void VerifyNotEqualsGatewayHostname()
        {
            string itemName = TEST_NAME + "_01";
            Assert.IsFalse(_comparer.Equals(_testGateway, itemName));

            itemName = string.Empty;
            Assert.IsFalse(_comparer.Equals(_testGateway, itemName));

            itemName = null;
            Assert.IsFalse(_comparer.Equals(_testGateway, itemName));
        }

        [TestMethod]
        public void VerifyEqualsGatewayHostnameIgnoresCase()
        {
            Assert.IsTrue(_comparer.Equals(_testGateway, TEST_NAME_CHANGED_CASE));
        }

    }
}
