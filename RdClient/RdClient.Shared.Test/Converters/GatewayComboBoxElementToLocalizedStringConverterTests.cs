using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Converters;
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
    public class GatewayComboBoxElementToLocalizedStringConverterTests
    {
        private GatewayComboBoxElementToLocalizedStringConverter _converter;
        private IStringTable _stringTable;
        private TestData _testData;
        private IModelContainer<GatewayModel> _gateway;
        private GatewayComboBoxElement _comboBoxElement;

        [TestInitialize]
        public void TestSetup()
        {
            _stringTable = new Mock.LocalizedString();
            _converter = new GatewayComboBoxElementToLocalizedStringConverter();
            _converter.LocalizedString = _stringTable;
            _testData = new TestData();
            _gateway = _testData.NewValidGateway();
            _comboBoxElement = new GatewayComboBoxElement(GatewayComboBoxType.Gateway, _gateway);
        }

        [TestMethod]
        public void ConvertAddNewComboBoxDoesNotReturnHostname()
        {
            Assert.AreNotEqual(_gateway.Model.HostName, _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertNoGatewayComboBoxDoesNotReturnHostname()
        {
            _comboBoxElement = new GatewayComboBoxElement(GatewayComboBoxType.None, _gateway);
            Assert.AreNotEqual(_gateway.Model.HostName, _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertGatewayComboBoxDoesReturnHostname()
        {
            _comboBoxElement = new GatewayComboBoxElement(GatewayComboBoxType.Gateway, _gateway);
            Assert.AreEqual(_gateway.Model.HostName, _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertGatewayComboBoxWithNullGatewayReturnsEmptyString()
        {
            _comboBoxElement = new GatewayComboBoxElement(GatewayComboBoxType.Gateway, null);
            Assert.AreEqual("", _converter.Convert(_comboBoxElement, null, null, null));
        }

        [TestMethod]
        public void ConvertGatewayComboBoxWithNullHostnameReturnsNull()
        {
            _gateway.Model.HostName = null;
            _comboBoxElement = new GatewayComboBoxElement(GatewayComboBoxType.Gateway, _gateway);
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
