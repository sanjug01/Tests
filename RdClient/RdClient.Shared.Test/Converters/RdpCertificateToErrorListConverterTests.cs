using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Converters;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RdClient.Shared.Test.Converters
{
    [TestClass]
    public class RdpCertificateToErrorListConverterTests
    {
        private RdpCertificateToErrorListConverter _converter;
        private IStringTable _stringTable;
        private Mock.RdpCertificate _cert;
        private IList<CertificateErrors> _handledErrors;

        [TestInitialize]
        public void TestSetup()
        {            
            _stringTable = new Mock.LocalizedString();
            _converter = new RdpCertificateToErrorListConverter();
            _converter.LocalizedString = _stringTable;
            _cert = new Mock.RdpCertificate();
            _cert.Error = new Mock.RdpCertificateError();
            _handledErrors = new List<CertificateErrors>()
            {
                CertificateErrors.Expired,
                CertificateErrors.NameMismatch,
                CertificateErrors.UntrustedRoot,
                CertificateErrors.Revoked,
                CertificateErrors.RevocationUnknown,
                CertificateErrors.MismatchedCert,
                CertificateErrors.WrongEKU,
                CertificateErrors.Critical
            };
        }

        [TestMethod]
        public void ConvertCertWithAllHandledErrorsReturnsCorrectNumberOfErrorStrings()
        {
            CertificateErrors allErrors = _handledErrors[0];
            foreach (CertificateErrors certErrorFlag in _handledErrors)
            {
                allErrors |= certErrorFlag;
            }
            _cert.Error = new Mock.RdpCertificateError(1, allErrors);
            IList<string> output = (IList<string>) _converter.Convert(_cert, null, null, null);
            Assert.AreEqual(_handledErrors.Count, output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBackThrows()
        {
            _converter.ConvertBack(new List<string>(), null, null, null);
        }

        [TestMethod]
        public void ConvertNullReturnsUnsetValue()
        {
            Assert.AreEqual(DependencyProperty.UnsetValue, _converter.Convert(null, null, null, null));
        }

        [TestMethod]
        public void ConvertCertificateWithNullErrorPropertyReturnsUnsetValue()
        {
            _cert.Error = null;
            Assert.AreEqual(DependencyProperty.UnsetValue, _converter.Convert(_cert, null, null, null));
        }

        [TestMethod]
        public void ConvertReturnsUnsetValueIfLocalizedStringPropertyIsNull()
        {
            _converter.LocalizedString = null;
            Assert.AreEqual(DependencyProperty.UnsetValue, _converter.Convert(_cert, null, null, null));
        }
    }
}
