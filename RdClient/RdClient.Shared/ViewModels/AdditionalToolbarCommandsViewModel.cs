namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation;
    using System.Windows.Input;

    public sealed class AdditionalToolbarCommandsViewModel : AccessoryViewModelBase
    {
        private ICommand _about;
        private ICommand _help;

        public AdditionalToolbarCommandsViewModel()
        {
            _about = new RelayCommand(o => ShowAboutViewCommandExecute());
            _help = new RelayCommand(o => ShowHelpCommandExecute());
        }

        private void ShowAboutViewCommandExecute()
        {
            DismissSelfAndPushAccessoryView("AboutView");
        }
        private void ShowHelpCommandExecute()
        {
            // TODO : not decided if link or internal document
            RichTextViewModelArgs args = new RichTextViewModelArgs(InternalDocType.EulaDoc);
            DismissSelfAndPushAccessoryView("RichTextView", args);
        }

        public ICommand ShowAboutDialog
        {
            get { return _about; }
        }

        public ICommand ShowHelpDialog
        {
            get { return _help; }
        }
    }
}
