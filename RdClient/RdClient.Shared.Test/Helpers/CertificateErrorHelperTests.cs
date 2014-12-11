namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.CxWrappers;
    using System;

    [TestClass]
    public sealed class CertificateErrorHelperTests
    {

        private CertificateErrors[] _allFlags;
        private CertificateErrors[] _someFlags;
        private CertificateErrors[] _someOtherFlags;


        [TestInitialize]
        public void TestSetUp()
        {
            _allFlags = new CertificateErrors[] { 
                CertificateErrors.Expired, 
                CertificateErrors.NameMismatch,
                CertificateErrors.UntrustedRoot,
                CertificateErrors.Revoked,
                CertificateErrors.RevocationUnknown,
                CertificateErrors.CertOrChainInvalid,
                CertificateErrors.MismatchedCert,
                CertificateErrors.WrongEKU,
                CertificateErrors.Critical };

            // exclusive flawgs, avoid Critical
            _someFlags = new CertificateErrors[] { 
                CertificateErrors.Expired, 
                CertificateErrors.NameMismatch,
                CertificateErrors.MismatchedCert,
                CertificateErrors.WrongEKU };

            _someOtherFlags = new CertificateErrors[] { 
                CertificateErrors.UntrustedRoot,
                CertificateErrors.Revoked,
                CertificateErrors.RevocationUnknown,
                CertificateErrors.CertOrChainInvalid };
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifySimpleErrors()
        {
            foreach(CertificateErrors error in _allFlags)
            {
                Assert.IsTrue(CertificateErrorHelper.ErrorContainsFlag(error, error));
            }
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifyDoubleErrors()
        {
            
            foreach (CertificateErrors error1 in _someFlags)
            {
                foreach (CertificateErrors error2 in _someOtherFlags)
                {
                    CertificateErrors testError = error1 | error2;
                    Assert.IsTrue(CertificateErrorHelper.ErrorContainsFlag(testError, error1));
                    Assert.IsTrue(CertificateErrorHelper.ErrorContainsFlag(testError, error2));

                    if (error1 != error2)
                    {
                        Assert.IsFalse(CertificateErrorHelper.ErrorContainsFlag(error1, error2));
                    }
                }
            }
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifyMultipleErrors()
        {
            CertificateErrors testError = 0;
            foreach (CertificateErrors error1 in _someFlags)
            {
                testError = testError | error1;
            }

            // verify flags exist
            foreach (CertificateErrors error1 in _someFlags)
            {
                Assert.IsTrue(CertificateErrorHelper.ErrorContainsFlag(testError, error1));
            }

            // verify flags don't exist
            foreach (CertificateErrors error2 in _someOtherFlags)
            {
                Assert.IsFalse(CertificateErrorHelper.ErrorContainsFlag(testError, error2));
            }            
        }


        [TestMethod]
        public void CertificateErrorHelper_ErrorNotHitIsNotFatal()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();
            CertificateErrors testError = 0;
            foreach (CertificateErrors error1 in _someFlags)
            {
                testError = testError | error1;
            }
            certError.ErrorFlags = testError;

            foreach (CertificateErrors error2 in _someOtherFlags)
            {
                Assert.IsFalse(CertificateErrorHelper.WasCertErrorHitAndFatal(certError, error2));
            }            
        }

        [TestMethod]
        public void CertificateErrorHelper_ErrorHitAndFatal()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();
            CertificateErrors testError = 0;
            foreach (CertificateErrors error1 in _someFlags)
            {
                testError = testError | error1;
            }
            testError = testError | CertificateErrors.Critical;
            certError.ErrorFlags = testError;

            foreach (CertificateErrors error2 in _someFlags)
            {
                Assert.IsTrue(CertificateErrorHelper.WasCertErrorHitAndFatal(certError, error2));
            }

            // Revoked and MismatchedCert are always fatal
            certError.ErrorFlags = CertificateErrors.Revoked | CertificateErrors.MismatchedCert;
            Assert.IsTrue(CertificateErrorHelper.WasCertErrorHitAndFatal(certError, CertificateErrors.Revoked));
            Assert.IsTrue(CertificateErrorHelper.WasCertErrorHitAndFatal(certError, CertificateErrors.MismatchedCert));
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifyNonFatalErrorsForCertificateError()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();

            certError.ErrorFlags = 0;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));

            certError.ErrorFlags = CertificateErrors.Expired | CertificateErrors.NameMismatch | CertificateErrors.UntrustedRoot;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));

            // If a certificate's revocation status cannot be determined, don't treat revocation as fatal
            certError.ErrorFlags = CertificateErrors.RevocationUnknown | CertificateErrors.Revoked;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifyFatalErrorsCertificateError()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();

            certError.ErrorFlags = CertificateErrors.Critical | CertificateErrors.Expired;
            Assert.IsTrue(CertificateErrorHelper.IsFatalCertificateError(certError));

            certError.ErrorFlags = CertificateErrors.Critical | CertificateErrors.UntrustedRoot;
            Assert.IsTrue(CertificateErrorHelper.IsFatalCertificateError(certError));

            // If a certificate's revocation status CAN be determined, revocation IS fatal
            certError.ErrorFlags = CertificateErrors.Revoked;
            Assert.IsTrue(CertificateErrorHelper.IsFatalCertificateError(certError));
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifyCertificateWithWrongEKU()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();

            // Wrong EKU errors are only fatal if they come from CredSSP
            certError.ErrorFlags = CertificateErrors.WrongEKU;

            certError.ErrorSource = ServerCertificateErrorSource.None;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));

            certError.ErrorSource = ServerCertificateErrorSource.Ssl;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));

            certError.ErrorSource = ServerCertificateErrorSource.CredSSP;
            Assert.IsTrue(CertificateErrorHelper.IsFatalCertificateError(certError));
        }
    }
}
