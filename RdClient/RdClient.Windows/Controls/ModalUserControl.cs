namespace RdClient.Controls
{
    using RdClient.Navigation;
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml.Controls;

    public abstract class ModalUserControl : UserControl, IPresentableView, IStackedView
    {
        private ModalFocusTracker _focusTracker;

        /// <summary>
        /// indicates if the user control is activated.
        /// this is an internal state, currently depending on _focusTracker
        /// </summary>
        private bool IsActivated
        {
            get { return null != _focusTracker; }
        }

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
            // Bug 2537012: avoid double deactivation which may occur on both DismissStackedView and self Dismiss
            if (this.IsActivated)
            {
                _focusTracker.Uninstall();
                _focusTracker = null;
            }
        }
    }
}
