using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class CertificateValidationViewModelTests
    {
        class TestCertificateValidationViewModel : CertificateValidationViewModel
        {
            public TestCertificateValidationViewModel()
            {
            }
        }

        private TestCertificateValidationViewModel _vm;

        [TestInitialize]
        public void TestSetUp()
        {
            _vm = new TestCertificateValidationViewModel();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _vm = null;
        }


        [TestMethod]
        public void CertificateValidationViewModel_VerifyCreation()
        {
            // TBD
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Assert.IsTrue(false);
            }
        }

        public void CertificateValidation_ShouldNotSaveOnCancel()
        {
            // TBD
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Assert.IsTrue(false);
            }
        }

        public void CertificateValidation_ShouldValidateWithSaving()
        {
            // TBD
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Assert.IsTrue(false);
            }
        }

        public void CertificateValidation_ShouldValidateWithoutSaving()
        {
            // TBD
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Assert.IsTrue(false);
            }
        }

        public void CertificateValidation_ShouldUseDelegateOnCancel()
        {
            // TBD
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Assert.IsTrue(false);
            }
        }

        public void CertificateValidation_ShouldUseDelegateOnConnect()
        {
            // TBD
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                Assert.IsTrue(false);
            }
        }

    }
}
