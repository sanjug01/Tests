using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class UserComboBoxElementToLocalizedStringConverterTests
    {
        private UserComboBoxElementToLocalizedStringConverter _converter;
        private IStringTable _stringTable;
        private TestData _testData;
        private Credentials _cred;
        private UserComboBoxElement _comboBoxElement;

        [TestInitialize]
        public void TestSetup()
        {
            _stringTable = new Mock.LocalizedString();
            _converter = new UserComboBoxElementToLocalizedStringConverter();
            _converter.LocalizedString = _stringTable;
            _testData = new TestData();
            _cred = _testData.NewValidCredential();
            _comboBoxElement = new UserComboBoxElement(UserComboBoxType.Credentials, _cred);
        }

        [TestMethod]
        public void ConvertAddNewComboBoxDoesNotReturnUsername()
        {
            _comboBoxElement = new UserComboBoxElement(UserComboBoxType.AddNew, _cred);
            Assert.AreNotEqual(_cred.Username, _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertAskEveryTimeComboBoxDoesNotReturnUsername()
        {
            _comboBoxElement = new UserComboBoxElement(UserComboBoxType.AskEveryTime, _cred);
            Assert.AreNotEqual(_cred.Username, _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertCredentialComboBoxDoesReturnUsername()
        {
            _comboBoxElement = new UserComboBoxElement(UserComboBoxType.Credentials, _cred);
            Assert.AreEqual(_cred.Username, _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertCredentialComboBoxWithNullCredReturnsEmptyString()
        {
            _comboBoxElement = new UserComboBoxElement(UserComboBoxType.Credentials, null);
            Assert.AreEqual("", _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertCredentialComboBoxWithNullUsernameReturnsNull()
        {
            _cred.Username = null;
            _comboBoxElement = new UserComboBoxElement(UserComboBoxType.Credentials, _cred);
            Assert.AreEqual(null, _converter.Convert(_comboBoxElement, null, null, null));
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
            _converter.Convert(_comboBoxElement, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertBackThrows()
        {
            _converter.ConvertBack(_comboBoxElement, null, null, null);
        }
    }
}
