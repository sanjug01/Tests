namespace RdClient.Shared.ViewModels
{
    using System.Windows.Input;

    public sealed class AdditionalToolbarCommandsViewModel : AccessoryViewModelBase
    {
        private ICommand _about;

        public AdditionalToolbarCommandsViewModel()
        {
            _about = new RelayCommand(parameter => DismissSelfAndPushAccessoryView("AboutView"));
        }

        public ICommand ShowAboutDialog
        {
            get { return _about; }
        }
    }
}
