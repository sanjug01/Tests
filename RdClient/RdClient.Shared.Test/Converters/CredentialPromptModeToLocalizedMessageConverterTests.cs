using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using RdClient.Shared.Helpers;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class CredentialPromptModeToLocalizedMessageConverterTests
    {
        private CredentialPromptModeToLocalizedMessageConverter _converter;
        private IStringTable _stringTable;
        private TestData _testData;
        private IList<CredentialPromptMode> _modesWithMessages;

        [TestInitialize]
        public void TestSetup()
        {
            _stringTable = new Mock.LocalizedString();
            _converter = new CredentialPromptModeToLocalizedMessageConverter();
            _converter.LocalizedString = _stringTable;
            _testData = new TestData();
            _modesWithMessages = new List<CredentialPromptMode>() { CredentialPromptMode.FreshCredentialsNeeded, CredentialPromptMode.InvalidCredentials };
        }

        [TestMethod]
        public void ModesWithMessagesReturnLocalizedStrings()
        {
            string prefix = _stringTable.GetLocalizedString(""); //Mock.LocalizedString simply returns the key passed in with a prefix added
            foreach(CredentialPromptMode mode in _modesWithMessages)
            {
                string localizedString = _converter.Convert(mode, null, null, null) as string;
                Assert.IsNotNull(localizedString, "Convert " + mode.ToString() + " should not return null");
                Assert.IsTrue(localizedString.StartsWith(prefix), "Convert" + mode.ToString() + " is not localized");
            }
        }

        [TestMethod]
        public void ModesWithoutMessagesReturnEmptyStrings()
        {            
            foreach (CredentialPromptMode mode in Enum.GetValues(typeof(CredentialPromptMode)))
            {
                if (!_modesWithMessages.Contains(mode))
                {
                    string localizedString = _converter.Convert(mode, null, null, null) as string;
                    Assert.AreEqual("", localizedString, "Convert " + mode.ToString() + " should return empty string");
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertNullThrows()
        {
            _converter.Convert(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertWrongTypeThrows()
        {
            _converter.Convert(new object(), null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertThrowsIfLocalizedStringPropertyIsNull()
        {
            _converter.LocalizedString = null;
            _converter.Convert(CredentialPromptMode.EnterCredentials, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertBackThrows()
        {
            _converter.ConvertBack("a string", null, null, null);
        }
    }
}
