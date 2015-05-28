namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Models;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ValidationRules;
    using System.Text;

    [TestClass]
    public class CredentialsEqualityComparerTests
    {
        private CredentialsModel _testCredentials;
        private CredentialsEqualityComparer _comparer;

        private readonly string TEST_NAME = "TestUser_aBc123";
        private readonly string TEST_NAME_CHANGED_CASE = "tESTuSER_AbC123";

        [TestInitialize]
        public void TestSetup()
        {

            _testCredentials = new CredentialsModel()
            {
                Username = TEST_NAME,
                Password = "1234AbCd"
            };

            _comparer = new CredentialsEqualityComparer();
        }

        [TestMethod]
        public void VerifyEqualsCredentialsUsername()
        {
            Assert.IsTrue(_comparer.Equals(_testCredentials, TEST_NAME));
        }

        [TestMethod]
        public void VerifyNotEqualsCredentialsUsername()
        {
            string itemName = TEST_NAME + "_01";
            Assert.IsFalse(_comparer.Equals(_testCredentials, itemName));

            itemName = string.Empty;
            Assert.IsFalse(_comparer.Equals(_testCredentials, itemName));

            itemName = null;
            Assert.IsFalse(_comparer.Equals(_testCredentials, itemName));
        }

        [TestMethod]
        public void VerifyEqualsCredentialsUsernameIgnoresCase()
        {
            Assert.IsTrue(_comparer.Equals(_testCredentials, TEST_NAME_CHANGED_CASE));
        }

    }
}
