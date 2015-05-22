namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation;
    using System.Windows.Input;

    public sealed class AdditionalToolbarCommandsViewModel : AccessoryViewModelBase
    {
        private readonly ICommand _about;

        public AdditionalToolbarCommandsViewModel()
        {
            _about = new RelayCommand(o => ShowAboutViewCommandExecute());
        }

        private void ShowAboutViewCommandExecute()
        {
            DismissSelfAndPushAccessoryView("AboutView");
        }

        public ICommand ShowAboutDialog
        {
            get { return _about; }
        }
    }
}
