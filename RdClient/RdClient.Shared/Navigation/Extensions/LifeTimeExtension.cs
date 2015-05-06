namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.LifeTimeManagement;

    /// <summary>
    /// Navigation service extension that attaches an ILifeTimeManager object to view models.
    /// Having the LifeTime manager object, a view model can use it to manage app state
    /// </summary>
    public sealed class LifeTimeExtension : MutableObject, INavigationExtension
    {
        private ILifeTimeManager _lifeTimeManager;

        public ILifeTimeManager LifeTimeManager
        {
            get { return _lifeTimeManager; }
            set { this.SetProperty<ILifeTimeManager>(ref _lifeTimeManager, value); }
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<ILifeTimeSite>(site => site.SetLifeTimeManager(_lifeTimeManager));
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<ILifeTimeSite>(site => site.SetLifeTimeManager(null));
        }
    }
}
