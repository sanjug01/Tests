using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

using RdClient.Shared.ValidationRules;

namespace RdClient.Shared.Test.ValidationRules
{
    [TestClass]
    public class HostNameValidationRuleTests
    {
        // list of illegal caracters 
        private const string _illegalCharacters = "`~!#@$%^&*()=+{}\\|;'\",< >/?";

        [TestMethod]
        public void ValidHostNames_ShouldBeValidated()
        {
            string hostName = "aBC";
            Assert.IsTrue(HostNameValidationRule.Validate(hostName, CultureInfo.CurrentCulture));

            hostName = "abc.mydomain.com";
            Assert.IsTrue(HostNameValidationRule.Validate(hostName, CultureInfo.CurrentCulture));

            hostName = "_myHost123Cd";
            Assert.IsTrue(HostNameValidationRule.Validate(hostName, CultureInfo.CurrentCulture));

            hostName = "23.24.11.22";
            Assert.IsTrue(HostNameValidationRule.Validate(hostName, CultureInfo.CurrentCulture));

            hostName = "mypc.com";
            Assert.IsTrue(HostNameValidationRule.Validate(hostName, CultureInfo.CurrentCulture));
        }

        [TestMethod]
        public void HostNamesHasInvalidCharacters_ShouldFailValidation()
        {
            string prefix = "a1b";
            string suffix = "_2df";
            string core;
            string hostName;
            for(int i = 0 ; i< _illegalCharacters.Length ; i++)
            {
                core = _illegalCharacters[i].ToString();
                hostName = prefix + core + suffix;
                Assert.IsFalse(HostNameValidationRule.Validate(hostName, CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void HostNamesStartsWithInvalidCharacters_ShouldFailValidation()
        {
            string prefix ;
            string suffix = "d3f";
            string core = "xy_";
            string hostName;
            for (int i = 0; i < _illegalCharacters.Length; i++)
            {
                prefix = _illegalCharacters[i].ToString();
                hostName = prefix + core + suffix;
                Assert.IsFalse(HostNameValidationRule.Validate(hostName, CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void HostNameEndsWithInvalidCharacters_ShouldFailValidation()
        {
            string prefix="pq";
            string suffix ;
            string core = "_tv_";
            string hostName;
            for (int i = 0; i < _illegalCharacters.Length; i++)
            {
                suffix = _illegalCharacters[i].ToString();
                hostName = prefix + core + suffix;
                Assert.IsFalse(HostNameValidationRule.Validate(hostName, CultureInfo.CurrentCulture));
            }
        }
    }
}
