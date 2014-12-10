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

    }
}
