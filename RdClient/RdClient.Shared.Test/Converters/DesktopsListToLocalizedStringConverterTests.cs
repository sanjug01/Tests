using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Converters;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.Test.UAP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class DesktopsListToLocalizedStringConverterTests
    {
        private DesktopsListToLocalizedStringConverter _converter;
        private IStringTable _stringTable;
        private TestData _testData;
        private IList<DesktopModel> _desktops;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _stringTable = new Mock.LocalizedString();
            _converter = new DesktopsListToLocalizedStringConverter();
            _converter.LocalizedString = _stringTable;
            _desktops = _testData.NewSmallListOfDesktops(_testData.NewSmallListOfCredentials());
        }

        [TestMethod]
        public void ConvertListWithSingleDesktopReturnsHostname()
        {
            DesktopModel desktop = _desktops[0];
            Assert.AreEqual(desktop.HostName, _converter.Convert(new List<DesktopModel>() { desktop }, null, null, null));
        }

        [TestMethod]
        public void ConvertListReturnsHostnamesSeparatedByCorrectSeparator()
        {
            string separator = _stringTable.GetLocalizedString(DesktopsListToLocalizedStringConverter.itemSeparatorStringId);
            string output = (string)_converter.Convert(_desktops, null, null, null);
            string[] outputNames = output.Split(new string[] {separator}, StringSplitOptions.None);
            CollectionAssert.AreEqual(_desktops.Select(d => d.HostName).ToArray(), outputNames);
        }

        [TestMethod]
        public void ConvertNullReturnsListIsEmptyString()
        {
            string listIsEmptyString = _stringTable.GetLocalizedString(DesktopsListToLocalizedStringConverter.emptyDesktopListStringId);
            Assert.AreEqual(listIsEmptyString, _converter.Convert(null, null, null, null));
        }

        [TestMethod]
        public void ConvertWrongTypeReturnsListIsEmptyString()
        {
            string listIsEmptyString = _stringTable.GetLocalizedString(DesktopsListToLocalizedStringConverter.emptyDesktopListStringId);
            Assert.AreEqual(listIsEmptyString, _converter.Convert(new object(), null, null, null));
        }
        [TestMethod]
        public void ConvertThrowsIfLocalizedStringPropertyIsNull()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() => {
                _converter.LocalizedString = null;
                _converter.Convert(_desktops, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertBackThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converter.ConvertBack(_testData.NewRandomString(), null, null, null);
            }));
        }

    }
}
