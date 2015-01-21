﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdClient.Shared.Helpers;
using RdClient.Shared.Test.Mock;
using RdClient.Shared.Test.Helpers;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class ResourceIdToLocalizedStringConverterTests
    {
        private ResourceIdToLocalizedStringConverter _converter;
        private IStringTable _stringTable;
        private TestData _testData;

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _stringTable = new Mock.LocalizedString();
            _converter = new ResourceIdToLocalizedStringConverter();
            _converter.LocalizedString = _stringTable;
        }

        [TestMethod]
        public void ConvertReturnsLocalizedString()
        {
            string inputString = _testData.NewRandomString();
            string expectedString = _stringTable.GetLocalizedString(inputString);
            Assert.AreEqual(expectedString, _converter.Convert(inputString, typeof(string), null, null));
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackThrows()
        {
            _converter.ConvertBack(_testData.NewRandomString(), typeof(string), null, null);
        }

        [TestMethod]
        public void ConvertNullReturnsUnsetValue()
        {
            Assert.AreEqual(DependencyProperty.UnsetValue, _converter.Convert(null, typeof(string), null, null));
        }

        [TestMethod]
        public void ConvertNonStringReturnsUnsetValue()
        {
            Assert.AreEqual(DependencyProperty.UnsetValue, _converter.Convert(new object(), typeof(string), null, null));
        }

        [TestMethod]
        public void ConvertReturnsUnsetValueIfLocalizedStringPropertyIsNull()
        {
            _converter.LocalizedString = null;
            string inputString = _testData.NewRandomString();
            Assert.AreEqual(DependencyProperty.UnsetValue, _converter.Convert(inputString, typeof(string), null, null));
        }
    }
}
