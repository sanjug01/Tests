﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class DesktopIdentityValidationViewModelTests
    {
        private DesktopIdentityValidationViewModel _vm;
        private Mock.RdpCertificate _testCertificate;
        private string _testHost;
        private DesktopIdentityValidationViewModelArgs _testArgs;
        private Mock.ModalPresentationContext _context;

        [TestInitialize]
        public void TestSetUp()
        {
            _vm = new DesktopIdentityValidationViewModel();

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

            _testArgs = new DesktopIdentityValidationViewModelArgs(_testHost);

            _context = new Mock.ModalPresentationContext();
        }
        
        [TestMethod]
        public void DesktopIdentityValidationViewModel_VerifyPresenting()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, null);

                Assert.AreEqual(_testHost, _vm.Host);
            }
        }

        [TestMethod]
        public void DesktopIdentityValidation_ShouldUseDelegateOnConnectAlways()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);

                _vm.AcceptIdentityCommand.Execute(null);

                Assert.IsTrue(_context.Result is DesktopIdentityValidationResult);
                Assert.AreEqual(DesktopIdentityValidationResult.IdentityTrustLevel.AcceptedAlways, (_context.Result as DesktopIdentityValidationResult).Result);
            }
        }

        [TestMethod]
        public void DesktopIdentityValidation_ShouldUseDelegateOnConnectOnce()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);

                _vm.AcceptOnceCommand.Execute(null);
                Assert.IsTrue(_context.Result is DesktopIdentityValidationResult);
                Assert.AreEqual(DesktopIdentityValidationResult.IdentityTrustLevel.AcceptedOnce, (_context.Result as DesktopIdentityValidationResult).Result);
            }
        }

        [TestMethod]
        public void DesktopIdentityValidation_ShouldUseDelegateOnCancel()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);

                _vm.CancelCommand.Execute(null);
                Assert.IsTrue(_context.Result is DesktopIdentityValidationResult);
                Assert.AreEqual(DesktopIdentityValidationResult.IdentityTrustLevel.Denied, (_context.Result as DesktopIdentityValidationResult).Result);
            }
        }

    }
}