namespace RdClient.Controls
{
    using RdClient.Navigation;
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml.Controls;

    public abstract class ModalUserControl : UserControl, IPresentableView
    {
        private ModalFocusTracker _focusTracker;

        protected abstract Control GetFirstTabControl();

        public IViewModel ViewModel
        {
            get { return this.DataContext as IViewModel; }
        }

        public void Activating(object activationParameter)
        {
        }

        public void Dismissing()
        {
            _focusTracker.Uninstall();
            _focusTracker = null;
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Assert(null == _focusTracker);
            _focusTracker = ModalFocusTracker.Install(this, GetFirstTabControl());
        }
    }
}
