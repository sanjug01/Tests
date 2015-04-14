namespace RdClient.Shared.ViewModels
{
    using System.Windows.Input;

    public sealed class SelectNewResourceTypeViewModel : ViewModelBase
    {
        private readonly RelayCommand _addDesktop;

        public ICommand AddDesktop
        {
            get { return _addDesktop; }
        }

        public SelectNewResourceTypeViewModel()
        {
            _addDesktop = new RelayCommand(this.ExecuteAddDesktop);
        }

        private void ExecuteAddDesktop(object parameter)
        {
            //
            // Dismiss self as a modal view; this will also dismisses accessory views.
            //
            this.DismissModal(null);
        }
    }
}
