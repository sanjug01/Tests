﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class CertificateValidationViewModelTests
    {
        private CertificateValidationViewModel _vm;
        private Mock.RdpCertificate _testCertificate;
        private string _testHost;
        private CertificateValidationViewModelArgs _testArgs;
        private Mock.ModalPresentationContext _context;

        [TestInitialize]
        public void TestSetUp()
        {
            _vm = new CertificateValidationViewModel();

            _testHost = "MyPC";

            _testCertificate = new Mock.RdpCertificate();
            _testCertificate.FriendlyName = "Test FriendlyName";
            _testCertificate.Issuer = "TestIssuer";
            _testCertificate.HasPrivateKey = true;
            _testCertificate.IsStronglyProtected = true;
            _testCertificate.SerialNumber = new byte[4] { 4, 5, 6, 7 };
            _testCertificate.Error = new Mock.RdpCertificateError();
            _testCertificate.ValidFrom = DateTimeOffset.Now.AddMonths(-1);
            _testCertificate.ValidTo = DateTimeOffset.Now.AddMonths(1);

            _testArgs = new CertificateValidationViewModelArgs(_testHost, _testCertificate);

            _context = new Mock.ModalPresentationContext();
        }
        
        [TestMethod]
        public void CertificateValidationViewModel_VerifyPresenting()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, null);

                Assert.IsFalse(_vm.IsExpandedView);
                Assert.AreEqual(_testHost, _vm.Host);
            }
        }

        [TestMethod]
        public void CertificateValidationViewModel_ShowAndHideDetails()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, null);
                Assert.IsFalse(_vm.IsExpandedView);

                // show extra details
                _vm.ShowDetails.Execute(null);
                Assert.IsTrue(_vm.IsExpandedView);

                // hide it back
                _vm.HideDetails.Execute(null);
                Assert.IsFalse(_vm.IsExpandedView);
            }
        }


        [TestMethod]
        public void CertificateValidation_ShouldUseDelegateOnConnectAlways()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);

                _context.Expect("Dismiss", parameters =>
                {
                    CertificateValidationResult result = parameters[0] as CertificateValidationResult;
                    Assert.AreEqual(CertificateValidationResult.CertificateTrustLevel.AcceptedAlways, (result.Result));
                    return null;
                });
                _vm.RememberChoice = true;
                _vm.AcceptCertificate.Execute(null);
            }
        }

        [TestMethod]
        public void CertificateValidation_ShouldUseDelegateOnConnectOnce()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);

                _context.Expect("Dismiss", parameters =>
                {
                    CertificateValidationResult result = parameters[0] as CertificateValidationResult;
                    Assert.AreEqual(CertificateValidationResult.CertificateTrustLevel.AcceptedOnce, result.Result);
                    return null;
                });
                _vm.RememberChoice = false;
                _vm.AcceptCertificate.Execute(null);
            }
        }

        [TestMethod]
        public void CertificateValidation_ShouldUseDelegateOnCancel()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);

                _context.Expect("Dismiss", parameters =>
                {
                    CertificateValidationResult result = parameters[0] as CertificateValidationResult;
                    Assert.AreEqual(CertificateValidationResult.CertificateTrustLevel.Denied, result.Result);
                    return null;
                });
                _vm.Cancel.Execute(null);
            }
        }

    }
}
