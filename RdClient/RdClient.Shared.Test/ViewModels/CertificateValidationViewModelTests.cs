﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class CertificateValidationViewModelTests
    {

        class TestModalPresentationContext : IModalPresentationContext
        {
            public object Result { get; private set; }
            void IModalPresentationContext.Dismiss(object result)
            {
                this.Result = result;
            }            
        }

        private CertificateValidationViewModel _vm;
        private Mock.RdpCertificate _testCertificate;
        private string _testHost;
        private CertificateValidationViewModelArgs _testArgs;
        private TestModalPresentationContext _context;

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

            _context = new TestModalPresentationContext();
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
        public void CertificateValidationViewModel_VerifyListOfErrorsUpdated()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                
                Mock.RdpCertificateError testError = new Mock.RdpCertificateError();

                // 6 error flags
                testError.ErrorFlags = CertificateErrors.Revoked 
                    | CertificateErrors.NameMismatch 
                    | CertificateErrors.UntrustedRoot 
                    | CertificateErrors.Expired
                    | CertificateErrors.CertOrChainInvalid
                    | CertificateErrors.MismatchedCert;
                _testCertificate.Error = testError;
                ((IViewModel)_vm).Presenting(navigation, _testArgs, null);

                // error list should have 6 items
                Assert.AreEqual(6, _vm.CertificateErrorsList.Count);
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
                _vm.ShowDetailsCommand.Execute(null);
                Assert.IsTrue(_vm.IsExpandedView);

                // hide it back
                _vm.HideDetailsCommand.Execute(null);
                Assert.IsFalse(_vm.IsExpandedView);
            }
        }


        [TestMethod]
        public void CertificateValidation_ShouldUseDelegateOnConnectAlways()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);

                _vm.AcceptCertificateCommand.Execute(null);

                Assert.IsTrue(_context.Result is CertificateValidationResult);
                Assert.AreEqual(CertificateValidationResult.CertificateTrustLevel.AcceptedAlways, (_context.Result as CertificateValidationResult).Result);
            }
        }

        [TestMethod]
        public void CertificateValidation_ShouldUseDelegateOnConnectOnce()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);

                _vm.AcceptOnceCommand.Execute(null);
                Assert.IsTrue(_context.Result is CertificateValidationResult);
                Assert.AreEqual(CertificateValidationResult.CertificateTrustLevel.AcceptedOnce, (_context.Result as CertificateValidationResult).Result);
            }
        }

        [TestMethod]
        public void CertificateValidation_ShouldUseDelegateOnCancel()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);

                _vm.CancelCommand.Execute(null);
                Assert.IsTrue(_context.Result is CertificateValidationResult);
                Assert.AreEqual(CertificateValidationResult.CertificateTrustLevel.Denied, (_context.Result as CertificateValidationResult).Result);
            }
        }

    }
}