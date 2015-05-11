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
        public void ValidateNullUsernameReturnsInvalid()
        {
            Assert.IsFalse(_rule.Validate(null).IsValid);
        }

        [TestMethod]
        public void ValidateEmptyUsernameReturnsInvalid()
        {
            Assert.IsFalse(_rule.Validate("").IsValid);
        }

        [TestMethod]
        public void ValidateNonEmptyUsernameReturnsValid()
        {
            Assert.IsTrue(_rule.Validate("!").IsValid);
        }
    }
}
