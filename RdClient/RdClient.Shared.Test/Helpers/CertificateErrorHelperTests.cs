namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.CxWrappers;
    using System;

    [TestClass]
    public sealed class CertificateErrorHelperTests
    {

        private CertificateError[] _allFlags;
        private CertificateError[] _someFlags;
        private CertificateError[] _someOtherFlags;


        [TestInitialize]
        public void TestSetUp()
        {
            _allFlags = new CertificateError[] { 
                CertificateError.Expired, 
                CertificateError.NameMismatch,
                CertificateError.UntrustedRoot,
                CertificateError.Revoked,
                CertificateError.RevocationUnknown,
                CertificateError.CertOrChainInvalid,
                CertificateError.MismatchedCert,
                CertificateError.WrongEKU,
                CertificateError.Critical };

            // exclusive flags, avoid Critical
            _someFlags = new CertificateError[] { 
                CertificateError.Expired, 
                CertificateError.NameMismatch,
                CertificateError.MismatchedCert,
                CertificateError.WrongEKU };

            _someOtherFlags = new CertificateError[] { 
                CertificateError.UntrustedRoot,
                CertificateError.Revoked,
                CertificateError.RevocationUnknown,
                CertificateError.CertOrChainInvalid };
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifySimpleErrors()
        {
            foreach(CertificateError error in _allFlags)
            {
                Assert.IsTrue(CertificateErrorHelper.ErrorContainsFlag(error, error));
            }
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifyDoubleErrors()
        {
            
            foreach (CertificateError error1 in _someFlags)
            {
                foreach (CertificateError error2 in _someOtherFlags)
                {
                    CertificateError testError = error1 | error2;
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
            CertificateError testError = 0;
            foreach (CertificateError error1 in _someFlags)
            {
                testError = testError | error1;
            }

            // verify flags exist
            foreach (CertificateError error1 in _someFlags)
            {
                Assert.IsTrue(CertificateErrorHelper.ErrorContainsFlag(testError, error1));
            }

            // verify flags don't exist
            foreach (CertificateError error2 in _someOtherFlags)
            {
                Assert.IsFalse(CertificateErrorHelper.ErrorContainsFlag(testError, error2));
            }            
        }


        [TestMethod]
        public void CertificateErrorHelper_ErrorNotHitIsNotFatal()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();
            CertificateError testError = 0;
            foreach (CertificateError error1 in _someFlags)
            {
                testError = testError | error1;
            }
            certError.ErrorFlags = testError;

            foreach (CertificateError error2 in _someOtherFlags)
            {
                Assert.IsFalse(CertificateErrorHelper.WasCertErrorHitAndFatal(certError, error2));
            }            
        }

        [TestMethod]
        public void CertificateErrorHelper_ErrorHitAndFatal()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();
            CertificateError testError = 0;
            foreach (CertificateError error1 in _someFlags)
            {
                testError = testError | error1;
            }
            testError = testError | CertificateError.Critical;
            certError.ErrorFlags = testError;

            foreach (CertificateError error2 in _someFlags)
            {
                Assert.IsTrue(CertificateErrorHelper.WasCertErrorHitAndFatal(certError, error2));
            }

            // Revoked and MismatchedCert are always fatal
            certError.ErrorFlags = CertificateError.Revoked | CertificateError.MismatchedCert;
            Assert.IsTrue(CertificateErrorHelper.WasCertErrorHitAndFatal(certError, CertificateError.Revoked));
            Assert.IsTrue(CertificateErrorHelper.WasCertErrorHitAndFatal(certError, CertificateError.MismatchedCert));
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifyNonFatalErrorsForCertificateError()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();

            certError.ErrorFlags = 0;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));

            certError.ErrorFlags = CertificateError.Expired | CertificateError.NameMismatch | CertificateError.UntrustedRoot;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));

            // If a certificate's revocation status cannot be determined, don't treat revocation as fatal
            certError.ErrorFlags = CertificateError.RevocationUnknown | CertificateError.Revoked;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifyFatalErrorsCertificateError()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();

            certError.ErrorFlags = CertificateError.Critical | CertificateError.Expired;
            Assert.IsTrue(CertificateErrorHelper.IsFatalCertificateError(certError));

            certError.ErrorFlags = CertificateError.Critical | CertificateError.UntrustedRoot;
            Assert.IsTrue(CertificateErrorHelper.IsFatalCertificateError(certError));

            // If a certificate's revocation status CAN be determined, revocation IS fatal
            certError.ErrorFlags = CertificateError.Revoked;
            Assert.IsTrue(CertificateErrorHelper.IsFatalCertificateError(certError));
        }

        [TestMethod]
        public void CertificateErrorHelper_VerifyCertificateWithWrongEKU()
        {
            Mock.RdpCertificateError certError = new Mock.RdpCertificateError();

            // Wrong EKU errors are only fatal if they come from CredSSP
            certError.ErrorFlags = CertificateError.WrongEKU;

            certError.ErrorSource = ServerCertificateErrorSource.None;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));

            certError.ErrorSource = ServerCertificateErrorSource.Ssl;
            Assert.IsFalse(CertificateErrorHelper.IsFatalCertificateError(certError));

            certError.ErrorSource = ServerCertificateErrorSource.CredSSP;
            Assert.IsTrue(CertificateErrorHelper.IsFatalCertificateError(certError));
        }
    }
}
