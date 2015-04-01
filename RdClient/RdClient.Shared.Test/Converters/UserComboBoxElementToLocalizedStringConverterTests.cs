using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Converters;
using RdClient.Shared.Data;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.Test.UAP;
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
        private IModelContainer<CredentialsModel> _cred;
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
            Assert.AreNotEqual(_cred.Model.Username, _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertAskEveryTimeComboBoxDoesNotReturnUsername()
        {
            _comboBoxElement = new UserComboBoxElement(UserComboBoxType.AskEveryTime, _cred);
            Assert.AreNotEqual(_cred.Model.Username, _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertCredentialComboBoxDoesReturnUsername()
        {
            _comboBoxElement = new UserComboBoxElement(UserComboBoxType.Credentials, _cred);
            Assert.AreEqual(_cred.Model.Username, _converter.Convert(_comboBoxElement, null, null, null));
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
            _cred.Model.Username = null;
            _comboBoxElement = new UserComboBoxElement(UserComboBoxType.Credentials, _cred);
            Assert.AreEqual(null, _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertNullThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converter.Convert(null, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertWrongTypeThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converter.Convert(new object(), null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertThrowsIfLocalizedStringPropertyIsNull()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converter.LocalizedString = null;
                _converter.Convert(_comboBoxElement, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertBackThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converter.ConvertBack(_comboBoxElement, null, null, null);
            }));
        }
    }
}
