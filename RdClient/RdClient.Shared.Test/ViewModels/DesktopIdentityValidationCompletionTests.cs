﻿namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;

    [TestClass]
    public sealed class DesktopIdentityValidationCompletionTests
    {
        private const string ViewName = "DesktopIdentityValidationView";

        private ApplicationDataModel _dataModel;
        private INavigationService _nav;
        private DesktopIdentityValidationViewModel _vm;
        private IRdpCertificate _certificate;
        private DesktopIdentityValidationViewModelArgs _args;
        private TestValidation _validation;
        ////private TestCertificateTrust _permanentTrust, _sessionTrust;

        private sealed class TestCertificate : IRdpCertificate
        {
            private readonly byte[] _serialNumber, _hash;
            private readonly IRdpCertificateError _error;

            private sealed class Error : IRdpCertificateError
            {
                int IRdpCertificateError.ErrorCode { get { return 42; } }

                CertificateError IRdpCertificateError.ErrorFlags { get { return CertificateError.UntrustedRoot; } }
                ServerCertificateErrorSource IRdpCertificateError.ErrorSource { get { return ServerCertificateErrorSource.None; } }
            }

            public TestCertificate()
            {
                Random rnd = new Random();

                _serialNumber = new byte[16];
                rnd.NextBytes(_serialNumber);
                _hash = new byte[32];
                rnd.NextBytes(_hash);
                _error = new Error();
            }

            string IRdpCertificate.FriendlyName { get { return "Friendly Name"; } }
            bool IRdpCertificate.HasPrivateKey { get { return false; } }
            bool IRdpCertificate.IsStronglyProtected { get { return true; } }
            string IRdpCertificate.Issuer { get { return "Issuer"; } }
            byte[] IRdpCertificate.SerialNumber { get { return _serialNumber; } }
            string IRdpCertificate.Subject { get { return "Subject"; } }
            DateTimeOffset IRdpCertificate.ValidFrom { get { return DateTimeOffset.MinValue; } }
            DateTimeOffset IRdpCertificate.ValidTo { get { return DateTimeOffset.MaxValue; } }
            byte[] IRdpCertificate.GetHashValue() { return _hash; }
            byte[] IRdpCertificate.GetHashValue(string hashAlgorithmName) { return _hash; }
            IRdpCertificateError IRdpCertificate.Error { get { return _error; } }
        }

        private sealed class TestView : IPresentableView
        {
            private readonly IViewModel _vm;

            public TestView(IViewModel vm) { _vm = vm; }
            IViewModel IPresentableView.ViewModel { get { return _vm; } }
            void IPresentableView.Activating(object activationParameter) { }
            void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
            void IPresentableView.Dismissing() { }
        }

        private sealed class TestViewPresenter : IViewPresenter
        {
            void IViewPresenter.PresentView(IPresentableView view) { }
            void IViewPresenter.PushModalView(IPresentableView view) { }
            void IViewPresenter.DismissModalView(IPresentableView view) { }
            void IViewPresenter.PresentingFirstModalView() { }
            void IViewPresenter.DismissedLastModalView() { }
        }

        private sealed class TestViewFactory : IPresentableViewFactory
        {
            private readonly IViewModel _vm;

            public TestViewFactory(IViewModel vm)
            {
                Assert.IsNotNull(vm);
                _vm = vm;
            }

            IPresentableView IPresentableViewFactory.CreateView(string name, object activationParameter)
            {
                Assert.AreEqual(ViewName, name);
                return new TestView(_vm);
            }

            void IPresentableViewFactory.AddViewClass(string name, System.Type viewClass, bool isSingleton) { }
        }

        private sealed class TestValidation : IServerIdentityValidation
        {
            private readonly IRdpCertificate _certificate;

            public event EventHandler Accepted;
            public event EventHandler Rejected;

            public TestValidation(IRdpCertificate certificate)
            {
                _certificate = certificate;
            }

            void IValidation.Accept()
            {
                if (null != this.Accepted)
                    this.Accepted(this, EventArgs.Empty);
            }

            void IValidation.Reject()
            {
                if (null != this.Rejected)
                    this.Rejected(this, EventArgs.Empty);
            }
        }

        ////private sealed class TestCertificateTrust : ICertificateTrust
        ////{
        ////    public readonly List<IRdpCertificate> Trusted = new List<IRdpCertificate>();

        ////    void ICertificateTrust.TrustCertificate(IRdpCertificate certificate) { this.Trusted.Add(certificate); }
        ////    void ICertificateTrust.RemoveAllTrust() { throw new NotImplementedException(); }
        ////    bool ICertificateTrust.IsCertificateTrusted(IRdpCertificate certificate) { throw new NotImplementedException(); }
        ////}

        [TestInitialize]
        public void SetUpTest()
        {
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            _nav = new NavigationService();
            _vm = new DesktopIdentityValidationViewModel();
            _nav.ViewFactory = new TestViewFactory(_vm);
            _nav.Presenter = new TestViewPresenter();
            _nav.Extensions.Add(new DataModelExtension() { AppDataModel = _dataModel });
            _certificate = new TestCertificate();
            _args = new DesktopIdentityValidationViewModelArgs("host");
            _validation = new TestValidation(_certificate);
            //_permanentTrust = new TestCertificateTrust();
            //_sessionTrust = new TestCertificateTrust();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _vm = null;
            _nav = null;
            _dataModel = null;
            _certificate = null;
            _args = null;
            _validation = null;
            ////_permanentTrust = null;
            ////_sessionTrust = null;
        }

        [TestMethod]
        public void DesktopIdentityValidationCompletion_AcceptOnce_AcceptedAndSessionTrusted()
        {
            DesktopIdentityValidationCompletion completion = 
                new DesktopIdentityValidationCompletion(
                    _validation 
                    ////, _permanentTrust, _sessionTrust
                    );
            int acceptedCount = 0;
            _validation.Accepted += (sender, e) => ++acceptedCount;
            _validation.Rejected += (sender, e) => Assert.Fail();

            _nav.PushModalView(ViewName, _args, completion);
            _vm.AcceptOnceCommand.Execute(null);
            
            ////Assert.AreEqual(0, _permanentTrust.Trusted.Count);
            ////Assert.AreEqual(1, _sessionTrust.Trusted.Count);
            ////Assert.AreSame(_certificate, _sessionTrust.Trusted[0]);
            Assert.AreEqual(1, acceptedCount);
        }

        [TestMethod]
        public void DesktopIdentityValidationCompletion_AcceptAlways_AcceptedAndPermanentTrusted()
        {
            DesktopIdentityValidationCompletion completion =
                new DesktopIdentityValidationCompletion(
                    _validation
                    ////, _permanentTrust, _sessionTrust
                    );
            int acceptedCount = 0;
            _validation.Accepted += (sender, e) => ++acceptedCount;
            _validation.Rejected += (sender, e) => Assert.Fail();

            _nav.PushModalView(ViewName, _args, completion);
            _vm.AcceptIdentityCommand.Execute(null);
            
            ////Assert.AreEqual(1, _permanentTrust.Trusted.Count);
            ////Assert.AreSame(_certificate, _permanentTrust.Trusted[0]);
            ////Assert.AreEqual(0, _sessionTrust.Trusted.Count);
            Assert.AreEqual(1, acceptedCount);
        }

        [TestMethod]
        public void DesktopIdentityValidationCompletion_Reject_Rejected()
        {
            DesktopIdentityValidationCompletion completion =
                new DesktopIdentityValidationCompletion(
                    _validation
                    ////, _permanentTrust, _sessionTrust
                    );
            int rejectedCount = 0;
            _validation.Accepted += (sender, e) => Assert.Fail();
            _validation.Rejected += (sender, e) => ++rejectedCount;

            _nav.PushModalView(ViewName, _args, completion);
            _vm.CancelCommand.Execute(null);
            
            ////Assert.AreEqual(0, _permanentTrust.Trusted.Count);
            ////Assert.AreEqual(0, _sessionTrust.Trusted.Count);
            Assert.AreEqual(1, rejectedCount);
        }

        [TestMethod]
        public void DesktopIdentityValidationCompletion_NavigateBack_Rejected()
        {
            DesktopIdentityValidationCompletion completion =
                new DesktopIdentityValidationCompletion(
                    _validation
                    ////, _permanentTrust, _sessionTrust
                    );
            int rejectedCount = 0;
            _validation.Accepted += (sender, e) => Assert.Fail();
            _validation.Rejected += (sender, e) => ++rejectedCount;

            _nav.PushModalView(ViewName, _args, completion);
            _vm.CastAndCall<IViewModel>(vm => vm.NavigatingBack(new BackCommandArgs()));

            ////Assert.AreEqual(0, _permanentTrust.Trusted.Count);
            ////Assert.AreEqual(0, _sessionTrust.Trusted.Count);
            Assert.AreEqual(1, rejectedCount);
        }
    }
}
