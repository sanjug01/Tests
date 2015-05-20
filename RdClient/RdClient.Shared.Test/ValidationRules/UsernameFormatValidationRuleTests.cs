namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.ValidationRules;

    [TestClass]
    public class UsernameFormatValidationRuleTests
    {
        UsernameFormatValidationRule _rule;

        [TestInitialize]
        public void TestSetup()
        {
            _rule = new UsernameFormatValidationRule();
        }

        [TestMethod]
        public void ValidateNullUsernameReturnsNullOrEmpty()
        {
            Assert.IsTrue(_rule.Validate(null).Status == ValidationResultStatus.Empty);
        }

        [TestMethod]
        public void ValidateEmptyUsernameReturnsNullOrEmpty()
        {
            Assert.IsTrue(_rule.Validate("").Status == ValidationResultStatus.Empty);
        }

        [TestMethod]
        public void ValidateNonEmptyUsernameReturnsValid()
        {
            Assert.IsTrue(_rule.Validate("!").Status == ValidationResultStatus.Valid);
        }
    }
}
