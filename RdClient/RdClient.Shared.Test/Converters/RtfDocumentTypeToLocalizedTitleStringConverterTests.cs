using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Converters;
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
    public class RtfDocumentTypeToLocalizedTitleStringConverterTests
    {
        private RtfDocumentTypeToLocalizedTitleStringConverter _converter;
        private IStringTable _stringTable;
        InternalDocType _testDocType;

        [TestInitialize]
        public void TestSetup()
        {
            _stringTable = new Mock.LocalizedString();
            _stringTable = new Mock.LocalizedString();
            _converter = new RtfDocumentTypeToLocalizedTitleStringConverter();
            TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
            ttlsc.LocalizedString = _stringTable;
            _converter.TypeToLocalizedStringConverter = ttlsc;

            _testDocType = InternalDocType.PrivacyDoc;
        }

        [TestMethod]
        public void ConvertValidRtfDocumentTypeType()
        {
            InternalDocType docType = InternalDocType.EulaDoc;
            object result;

            result = (string) _converter.Convert(docType, typeof(string), null, "");
            Assert.IsInstanceOfType(result, typeof(string));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ConvertNullThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<NullReferenceException>(() =>
            {
                _converter.Convert(null, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertWrongTypeThrows()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidCastException>(() =>
            {
                _converter.Convert(new object(), null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertThrowsIfLocalizedStringPropertyIsNull()
        {
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converter.TypeToLocalizedStringConverter = null;
                _converter.Convert(_testDocType, null, null, null);
            }));
        }

        [TestMethod]
        public void ConvertBackThrows()
        {
            string title = "Some title";
            Assert.IsTrue(ExceptionExpecter.ExpectException<InvalidOperationException>(() =>
            {
                _converter.ConvertBack(title, null, null, null);
            }));
        }
    }
}
