namespace RdClient.Shared.Test.ValidationRules
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.ValidationRules;

    [TestClass]
    public class HostNameValidationRuleTests
    {
        // list of illegal characters 
        private const string _illegalCharacters = "`~!#@$%^&*()=+{}\\|;'\",< >/?";

        [TestMethod]
        public void ValidHostNames_ShouldBeValidated()
        {
            HostNameValidationRule rule = new HostNameValidationRule();

            string hostName = "aBC";
            Assert.IsTrue(rule.Validate(hostName).IsValid);

            hostName = "abc.mydomain.com";
            Assert.IsTrue(rule.Validate(hostName).IsValid);

            hostName = "_myHost123Cd";
            Assert.IsTrue(rule.Validate(hostName).IsValid);

            hostName = "23.24.11.22";
            Assert.IsTrue(rule.Validate(hostName).IsValid);

            hostName = "mypc.com";
            Assert.IsTrue(rule.Validate(hostName).IsValid);
        }

        [TestMethod]
        public void HostNamesHasInvalidCharacters_ShouldFailValidation()
        {
            HostNameValidationRule rule = new HostNameValidationRule();

            string prefix = "a1b";
            string suffix = "_2df";
            string core;
            string hostName;
            for (int i = 0; i < _illegalCharacters.Length; i++)
            {
                core = _illegalCharacters[i].ToString();
                hostName = prefix + core + suffix;
                Assert.IsFalse(rule.Validate(hostName).IsValid);
            }
        }

        [TestMethod]
        public void HostNamesStartsWithInvalidCharacters_ShouldFailValidation()
        {
            HostNameValidationRule rule = new HostNameValidationRule();

            string prefix;
            string suffix = "d3f";
            string core = "xy_";
            string hostName;
            for (int i = 0; i < _illegalCharacters.Length; i++)
            {
                prefix = _illegalCharacters[i].ToString();
                hostName = prefix + core + suffix;
                Assert.IsFalse(rule.Validate(hostName).IsValid);
            }
        }

        [TestMethod]
        public void HostNameEndsWithInvalidCharacters_ShouldFailValidation()
        {
            HostNameValidationRule rule = new HostNameValidationRule();

            string prefix = "pq";
            string suffix;
            string core = "_tv_";
            string hostName;
            for (int i = 0; i < _illegalCharacters.Length; i++)
            {
                suffix = _illegalCharacters[i].ToString();
                hostName = prefix + core + suffix;
                Assert.IsFalse(rule.Validate(hostName).IsValid);
            }
        }
    }
}
