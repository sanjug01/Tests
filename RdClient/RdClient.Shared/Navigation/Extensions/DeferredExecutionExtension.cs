namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Helpers;

    /// <summary>
    /// Navigation service extension that attaches an IDeferredExecution object to view models.
    /// Having the deferred execution object, a view model can use it to defer its update until
    /// an appropriate moment (most practical deferrals are posted to the UI thread).
    /// </summary>
    public sealed class DeferredExecutionExtension : MutableObject, INavigationExtension
    {
        private IDeferredExecution _deferredExecution;

        public IDeferredExecution DeferredExecution
        {
            get { return _deferredExecution; }
            set { this.SetProperty<IDeferredExecution>(ref _deferredExecution, value); }
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<IDeferredExecutionSite>(site => site.SetDeferredExecution(_deferredExecution));
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<IDeferredExecutionSite>(site => site.SetDeferredExecution(null));
        }
    }
}
