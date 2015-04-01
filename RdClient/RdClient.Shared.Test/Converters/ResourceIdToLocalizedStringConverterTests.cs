using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Converters;
using RdClient.Shared.Helpers;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.Test.UAP;
using System;

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
        public void ConvertBackThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converter.ConvertBack(_testData.NewRandomString(), typeof(string), null, null);
            }));
        }

        [TestMethod]
        public void ConvertNullThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converter.Convert(null, typeof(string), null, null);
            }));
        }

        [TestMethod]
        public void ConvertNonStringThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<ArgumentException>(() =>
            {
                _converter.Convert(new object(), typeof(string), null, null);
            }));
        }

        [TestMethod]
        public void ConvertThrowsIfLocalizedStringPropertyIsNull()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converter.LocalizedString = null;
                string inputString = _testData.NewRandomString();
                _converter.Convert(inputString, typeof(string), null, null);
            }));
        }
    }
}
