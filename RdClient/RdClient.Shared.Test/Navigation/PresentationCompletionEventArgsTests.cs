namespace RdClient.Shared.Test.Navigation
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Navigation;

    [TestClass]
    public sealed class PresentationCompletionEventArgsTests
    {
        private sealed class TestView : IPresentableView
        {
            IViewModel IPresentableView.ViewModel { get { return null; } }
            void IPresentableView.Activating(object activationParameter) { }
            void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
            void IPresentableView.Dismissing() { }
        }

        [TestMethod]
        public void NewPresentationCompletionEventArgs_CorrectProperties()
        {
            IPresentableView view = new TestView();
            object result = new object();

            PresentationCompletionEventArgs args = new PresentationCompletionEventArgs(view, result);
            Assert.AreSame(view, args.View);
            Assert.AreSame(result, args.Result);
        }
    }
}
