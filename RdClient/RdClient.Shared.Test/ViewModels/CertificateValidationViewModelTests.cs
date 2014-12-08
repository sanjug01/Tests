using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestCleanup]
        public void TestTearDown()
        {
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
        public void CertificateValidation_ShouldNotSaveOnCancel()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.DataModel dataModel = new Mock.DataModel())
            {
                _vm.DataModel = dataModel;
                ((IViewModel)_vm).Presenting(navigation, _testArgs, null);

                navigation.Expect("DismissModalView", new List<object> { null, false }, null);
                _vm.CancelCommand.Execute(null);

                // Fail this test until data model supports certificates persistance
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void CertificateValidation_ShouldValidateWithSaving()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.DataModel dataModel = new Mock.DataModel())
            {
                _vm.DataModel = dataModel;
                ((IViewModel)_vm).Presenting(navigation, _testArgs, null);
                navigation.Expect("DismissModalView", new List<object> { null, false }, null);
                _vm.AcceptCertificateCommand.Execute(null);

                // Fail this test until data model supports certificates persistance
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void CertificateValidation_ShouldValidateWithoutSaving()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.DataModel dataModel = new Mock.DataModel())
            {
                _vm.DataModel = dataModel;
                ((IViewModel)_vm).Presenting(navigation, _testArgs, null);
                navigation.Expect("DismissModalView", new List<object> { null, false }, null);
                _vm.AcceptOnceCommand.Execute(null);

                // Fail this test until data model supports certificates persistance
                Assert.IsTrue(false);
            }
        }

        [TestMethod]
        public void CertificateValidation_ShouldUseDelegateOnCancel()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context );
                Assert.AreNotEqual(false, _context.Result);

                _vm.CancelCommand.Execute(null);
                Assert.AreEqual(false, _context.Result);
            }
        }

        [TestMethod]
        public void CertificateValidation_ShouldUseDelegateOnConnectAlways()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);
                Assert.AreNotEqual(false, _context.Result);

                _vm.AcceptCertificateCommand.Execute(null);
                Assert.AreEqual(true, _context.Result);
            }
        }

        [TestMethod]
        public void CertificateValidation_ShouldUseDelegateOnConnectOnce()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                ((IViewModel)_vm).Presenting(navigation, _testArgs, _context);
                Assert.AreNotEqual(false, _context.Result);

                _vm.AcceptOnceCommand.Execute(null);
                Assert.AreEqual(true, _context.Result);
            }
        }

    }
}
