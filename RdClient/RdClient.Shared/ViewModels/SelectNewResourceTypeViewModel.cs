namespace RdClient.Shared.ViewModels
{
    using System.Windows.Input;

    public sealed class SelectNewResourceTypeViewModel : AccessoryViewModelBase
    {
        private readonly RelayCommand _addDesktop;
        private readonly RelayCommand _addOnPremiseWorkspace;
        private readonly RelayCommand _addCloudWorkspace;

        public ICommand AddDesktop
        {
            get { return _addDesktop; }
        }

        public ICommand AddOnPremiseWorkspace
        {
            get { return _addOnPremiseWorkspace; }
        }

        public ICommand AddCloudWorkspace
        {
            get { return _addCloudWorkspace; }
        }

        public SelectNewResourceTypeViewModel()
        {
            _addDesktop = new RelayCommand(this.ExecuteAddDesktop);
            _addOnPremiseWorkspace = new RelayCommand(this.ExecuteAddOnPremiseWorkspace);
            _addCloudWorkspace = new RelayCommand(this.ExecuteAddCloudWorkspace);
        }

        private void ExecuteAddDesktop(object parameter)
        {
            DismissSelfAndPushAccessoryView("DesktopEditorView");
        }

        private void ExecuteAddOnPremiseWorkspace(object parameter)
        {
            DismissSelfAndPushAccessoryView("DesktopEditorView", null);
        }

        private void ExecuteAddCloudWorkspace(object parameter)
        {
            DismissSelfAndPushAccessoryView("DesktopEditorView", null);
        }
    }
}
