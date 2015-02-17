using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using RdClient.Shared.Converters;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertBackThrows()
        {
            _converter.ConvertBack(new List<string>(), null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertNullThrows()
        {
            _converter.Convert(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertCertificateWithNullErrorPropertyThrows()
        {
            _cert.Error = null;
            _converter.Convert(_cert, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertThrowsIfLocalizedStringPropertyIsNull()
        {
            _converter.TypeToLocalizedStringConverter = null;
            _converter.Convert(_cert, null, null, null);
        }
    }
}
