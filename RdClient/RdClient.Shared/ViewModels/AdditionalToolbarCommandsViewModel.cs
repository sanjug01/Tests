﻿namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation;
    using System.Windows.Input;

    public sealed class AdditionalToolbarCommandsViewModel : ViewModelBase
    {
        private ICommand _about;

        public AdditionalToolbarCommandsViewModel()
        {
            _about = new RelayCommand(o => ShowAboutViewCommandExecute());
        }

        private void ShowAboutViewCommandExecute()
        {
            var nav = NavigationService;
            //Dismiss ourselves first, no need to wait on the AboutView
            this.DismissModal(null);
            nav.PushAccessoryView("AboutView", null);
        }

        public ICommand ShowAboutDialog
        {
            get { return _about; }
        }
    }
}
