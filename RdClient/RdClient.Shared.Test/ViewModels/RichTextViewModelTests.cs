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
    public class RichTextViewModelTests
    {
        private Mock.NavigationService _navService;
        private Mock.ModalPresentationContext _context;
        private RichTextViewModel _vm;

        [TestInitialize]
        public void TestSetup()
        {
            _navService = new Mock.NavigationService();

            _vm = new RichTextViewModel();
            _context = new Mock.ModalPresentationContext();
            RichTextViewModelArgs args = new RichTextViewModelArgs(InternalDocType.PrivacyDoc);
            ((IViewModel)_vm).Presenting(_navService, args, _context); 
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _context.Dispose();
        }

        [TestMethod]
        public void RichTextView_CloseCommandDismisses()
        {
            _context.Expect("Dismiss", null);
            _vm.Close.Execute(null);
        }
        
        [TestMethod]
        public void RichTextView_BackNavigationDismisses()
        {
            _context.Expect("Dismiss", p => { return null; });
            IBackCommandArgs backArgs = new BackCommandArgs();
            Assert.IsFalse(backArgs.Handled);
            (_vm as IViewModel).NavigatingBack(backArgs);
            Assert.IsTrue(backArgs.Handled);
        }
    }
}
