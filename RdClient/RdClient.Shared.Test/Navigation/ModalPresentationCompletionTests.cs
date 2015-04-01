namespace RdClient.Shared.Test.Navigation
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Navigation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [TestClass]
    public sealed class ModalPresentationCompletionTests
    {
        [DebuggerNonUserCode] // exclude from code coverage
        private sealed class TestView : IPresentableView
        {
            IViewModel IPresentableView.ViewModel { get { return null; } }
            void IPresentableView.Activating(object activationParameter) { }
            void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
            void IPresentableView.Dismissing() { }
        }

        [TestMethod]
        public void RegisterPresentationCompletionHandler_Complete_Reported()
        {
            IList<PresentationCompletionEventArgs> reported = new List<PresentationCompletionEventArgs>();
            ModalPresentationCompletion completion = new ModalPresentationCompletion();
            IPresentationCompletion iCompletion = completion;
            IPresentableView view = new TestView();
            object result = new object();

            completion.Completed += (s, e) => reported.Add(e);
            iCompletion.Completed(view, result);
            Assert.AreEqual(1, reported.Count);
            Assert.AreSame(view, reported[0].View);
            Assert.AreSame(result, reported[0].Result);
        }

        [TestMethod]
        public void RegisterAndUnregisterPresentationCompletionHandler_Complete_NotReported()
        {
            IList<PresentationCompletionEventArgs> reported = new List<PresentationCompletionEventArgs>();
            ModalPresentationCompletion completion = new ModalPresentationCompletion();
            IPresentationCompletion iCompletion = completion;
            IPresentableView view = new TestView();
            object result = new object();

            EventHandler<PresentationCompletionEventArgs> handler = (s, e) => reported.Add(e);
            completion.Completed += handler;
            completion.Completed -= handler;
            iCompletion.Completed(view, result);
            Assert.AreEqual(0, reported.Count);
        }
    }
}
