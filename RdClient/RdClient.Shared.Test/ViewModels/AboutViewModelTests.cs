namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.Test.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class AboutViewModelTests
    {
        private Mock.NavigationService _navService;
        private Mock.ModalPresentationContext _context;
        private AboutViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _navService = new Mock.NavigationService();
            _vm = new AboutViewModel();
            ((ITelemetryClientSite)_vm).SetTelemetryClient(new Mock.TestTelemetryClient());
            _context = new Mock.ModalPresentationContext();
            ((IViewModel)_vm).Presenting(_navService, null, _context); 
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _context.Dispose();
        }

        [TestMethod]
        public void AboutView_CloseCommandDismisses()
        {
            _context.Expect("Dismiss", p => { return null; });
            _vm.Close.Execute(null);
        }
        
        [TestMethod]
        public void AboutView_BackNavigationDismisses()
        {
            _context.Expect("Dismiss", p => { return null; });
            IBackCommandArgs backArgs = new BackCommandArgs();
            Assert.IsFalse(backArgs.Handled);
            (_vm as IViewModel).NavigatingBack(backArgs);
            Assert.IsTrue(backArgs.Handled);
        }

        [TestMethod]
        public void AboutView_ShowEulaCmdShowsDocument()
        {
            _navService.Expect("PushAccessoryView", new List<object> { "RichTextView", null, null }, null);
            _vm.ShowEulaCommand.Execute(null);
        }


        [TestMethod]
        public void AboutView_ShowThirdPartyCmdShowsDocument()
        {
            _navService.Expect("PushAccessoryView", new List<object> { "RichTextView", null, null }, null);
            _vm.ShowThirdPartyNoticesCommand.Execute(null);
        }


    }
}
