using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Converters;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Test.UAP;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class RdpCertificateToErrorListConverterTests
    {
        private RdpCertificateToErrorListConverter _converter;
        private IStringTable _stringTable;
        private Mock.RdpCertificate _cert;
        private IList<CertificateError> _handledErrors;

        [TestInitialize]
        public void TestSetup()
        {            
            _stringTable = new Mock.LocalizedString();
            _converter = new RdpCertificateToErrorListConverter();
            TypeToLocalizedStringConverter ttlsc = new TypeToLocalizedStringConverter();
            ttlsc.LocalizedString = _stringTable;
            _converter.TypeToLocalizedStringConverter = ttlsc;
            _cert = new Mock.RdpCertificate();
            _cert.Error = new Mock.RdpCertificateError();
            _handledErrors = new List<CertificateError>()
            {
                CertificateError.Expired,
                CertificateError.NameMismatch,
                CertificateError.UntrustedRoot,
                CertificateError.Revoked,
                CertificateError.RevocationUnknown,
                CertificateError.MismatchedCert,
                CertificateError.WrongEKU,
                CertificateError.Critical
            };
        }

        [TestMethod]
        public void ConvertCertWithAllHandledErrorsReturnsCorrectNumberOfErrorStrings()
        {
            CertificateError allErrors = _handledErrors[0];
            foreach (CertificateError certErrorFlag in _handledErrors)
            {
                allErrors |= certErrorFlag;
            }
            _cert.Error = new Mock.RdpCertificateError(1, allErrors);
            IList<string> output = (IList<string>) _converter.Convert(_cert, null, null, null);
            Assert.AreEqual(_handledErrors.Count, output.Count);
        }

        [TestMethod]
        public void ConvertBackThrows()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _converter.ConvertBack(new List<string>(), null, null, null);
            });
        }

        [TestMethod]
        public void ConvertNullReturnsEmptyErrorList()
        {
            var errorList = _converter.Convert(null, null, null, null) as IList<string>;
            Assert.IsNotNull(errorList);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void ConvertCertificateWithNullErrorPropertyReturnsEmptyErrorList()
        {
            _cert.Error = null;
            var errorList = _converter.Convert(_cert, null, null, null) as IList<string>;
            Assert.IsNotNull(errorList);
            Assert.AreEqual(0, errorList.Count);
        }

        [TestMethod]
        public void ConvertThrowsIfLocalizedStringPropertyIsNull()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                _converter.TypeToLocalizedStringConverter = null;
                _converter.Convert(_cert, null, null, null);
            });
        }
    }
}
