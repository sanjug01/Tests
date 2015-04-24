namespace RdClient.Controls
{
    using RdClient.Navigation;
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml.Controls;

    public abstract class ModalUserControl : UserControl, IPresentableView, IStackedView
    {
        private ModalFocusTracker _focusTracker;

        public IViewModel ViewModel
        {
            get { return this.DataContext as IViewModel; }
        }

        public void Activating(object activationParameter) { }

        public void Dismissing()
        {
            Contract.Assert(null == _focusTracker);
        }

        public void Presenting(INavigationService navigationService, object activationParameter) { }

        /// <summary>
        /// IStackedView.Activate - install the focus tracker so the view will respond
        /// to Tab, Escape and Enter keys.
        /// </summary>
        public void Activate()
        {
            Contract.Assert(null == _focusTracker);
            _focusTracker = ModalFocusTracker.Install(this);
        }

        /// <summary>
        /// IStackedView.Deactivate - remove the focus tracker so the view will not respond
        /// to Tab, Escape and Enter keys.
        /// </summary>
        public void Deactivate()
        {
            _focusTracker.Uninstall();
            _focusTracker = null;
        }
    }
}
