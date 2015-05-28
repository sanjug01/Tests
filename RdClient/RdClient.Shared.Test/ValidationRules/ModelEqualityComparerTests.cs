namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ValidationRules;
    using System.Text;

    [TestClass]
    public class ModelEqualityComparerTests
    {
        private TestData _testData;
        private GatewayModel _testGateway;
        private CredentialsModel _testCredentials;
        private GatewayEqualityComparer _gatewayComparer;
        private CredentialsEqualityComparer _credComparer;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _testGateway = new GatewayModel()
            {
                HostName = _testData.NewRandomString() + "aBc",
            };

            _testCredentials = new CredentialsModel()
            {
                Username = _testData.NewRandomString() + "AbCd",
                Password = _testData.NewRandomString()
            };

            _gatewayComparer = new GatewayEqualityComparer();
            _credComparer = new CredentialsEqualityComparer();
        }

        /* **********************************************
        *           GatewayEqualityComparer tests
        *********************************************** */
        [TestMethod]
        public void VerifyEqualsGatewayHostname()
        {
            string itemName = _testGateway.HostName;
            Assert.IsTrue(_gatewayComparer.Equals(_testGateway, itemName) );
        }

        [TestMethod]
        public void VerifyNotEqualsGatewayHostname()
        {
            string itemName = _testGateway.HostName + "_01";
            Assert.IsFalse(_gatewayComparer.Equals(_testGateway, itemName));

            itemName = string.Empty;
            Assert.IsFalse(_gatewayComparer.Equals(_testGateway, itemName));

            itemName = null;
            Assert.IsFalse(_gatewayComparer.Equals(_testGateway, itemName));
        }

        [TestMethod]
        public void VerifyEqualsGatewayHostnameIgnoresCase()
        {
            string itemName = ChangeCase(_testGateway.HostName);
            Assert.IsTrue(_gatewayComparer.Equals(_testGateway, itemName));
        }

        /* **********************************************
        *           CredentialsEqualityComparer tests
        *********************************************** */
        [TestMethod]
        public void VerifyEqualsCredentialsUsername()
        {
            string itemName = _testCredentials.Username;
            Assert.IsTrue(_credComparer.Equals(_testCredentials, itemName));
        }

        [TestMethod]
        public void VerifyNotEqualsCredentialsUsername()
        {
            string itemName = _testCredentials.Username + "_01";
            Assert.IsFalse(_credComparer.Equals(_testCredentials, itemName));

            itemName = string.Empty;
            Assert.IsFalse(_credComparer.Equals(_testCredentials, itemName));

            itemName = null;
            Assert.IsFalse(_credComparer.Equals(_testCredentials, itemName));
        }

        [TestMethod]
        public void VerifyEqualsCredentialsUsernameIgnoresCase()
        {
            string itemName = ChangeCase(_testCredentials.Username);
            Assert.IsTrue(_credComparer.Equals(_testCredentials, itemName));
        }

        // Create a new itemName by switching the case of all letters
        private string ChangeCase(string itemName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in itemName)
            {
                char newC = c;
                if (char.IsLetter(c))
                {
                    if (char.IsUpper(c))
                    {
                        newC = char.ToLower(c);
                    }
                    else
                    {
                        newC = char.ToUpper(c);
                    }
                }
                sb.Append(newC);
            }

            return sb.ToString();
        }
    }
}
