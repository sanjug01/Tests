namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation;
    using System.Windows.Input;

    public sealed class AboutViewModel : AccessoryViewModelBase
    {
        private readonly ICommand _navigateBack;

        public AboutViewModel()
        {
            _navigateBack = new RelayCommand(o => this.DismissModal(null));
        }

        public ICommand NavigateBack
        {
            get { return _navigateBack; }
        }

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            base.OnNavigatingBack(backArgs);
            DismissModal(null);
        }
    }
}
