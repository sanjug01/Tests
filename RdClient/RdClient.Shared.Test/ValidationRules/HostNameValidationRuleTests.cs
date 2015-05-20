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
            HostnameValidationRule rule = new HostnameValidationRule();

            string hostName = "aBC";
            Assert.IsTrue(rule.Validate(hostName).Status == ValidationResultStatus.Valid);

            hostName = "abc.mydomain.com";
            Assert.IsTrue(rule.Validate(hostName).Status == ValidationResultStatus.Valid);

            hostName = "_myHost123Cd";
            Assert.IsTrue(rule.Validate(hostName).Status == ValidationResultStatus.Valid);

            hostName = "23.24.11.22";
            Assert.IsTrue(rule.Validate(hostName).Status == ValidationResultStatus.Valid);

            hostName = "mypc.com";
            Assert.IsTrue(rule.Validate(hostName).Status == ValidationResultStatus.Valid);
        }

        [TestMethod]
        public void HostNamesHasInvalidCharacters_ShouldFailValidation()
        {
            HostnameValidationRule rule = new HostnameValidationRule();

            string prefix = "a1b";
            string suffix = "_2df";
            string core;
            string hostName;
            for (int i = 0; i < _illegalCharacters.Length; i++)
            {
                core = _illegalCharacters[i].ToString();
                hostName = prefix + core + suffix;
                Assert.IsTrue(rule.Validate(hostName).Status == ValidationResultStatus.Invalid);
            }
        }

        [TestMethod]
        public void HostNamesStartsWithInvalidCharacters_ShouldFailValidation()
        {
            HostnameValidationRule rule = new HostnameValidationRule();

            string prefix;
            string suffix = "d3f";
            string core = "xy_";
            string hostName;
            for (int i = 0; i < _illegalCharacters.Length; i++)
            {
                prefix = _illegalCharacters[i].ToString();
                hostName = prefix + core + suffix;
                Assert.IsTrue(rule.Validate(hostName).Status == ValidationResultStatus.Invalid);
            }
        }

        [TestMethod]
        public void HostNameEndsWithInvalidCharacters_ShouldFailValidation()
        {
            HostnameValidationRule rule = new HostnameValidationRule();

            string prefix = "pq";
            string suffix;
            string core = "_tv_";
            string hostName;
            for (int i = 0; i < _illegalCharacters.Length; i++)
            {
                suffix = _illegalCharacters[i].ToString();
                hostName = prefix + core + suffix;
                Assert.IsTrue(rule.Validate(hostName).Status == ValidationResultStatus.Invalid);
            }
        }
    }
}
