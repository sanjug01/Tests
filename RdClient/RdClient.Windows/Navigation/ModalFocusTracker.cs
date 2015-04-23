namespace RdClient.Navigation
{
    using System.Diagnostics.Contracts;
    using Windows.System;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    sealed class ModalFocusTracker
    {
        private readonly UIElement _view;
        private readonly Control _firstControl;
        private Control _focused;

        public static ModalFocusTracker Install(UIElement view, Control firstControl)
        {
            Contract.Assert(null != view);
            Contract.Assert(null != firstControl);
            return new ModalFocusTracker(view, firstControl);
        }

        public void Uninstall()
        {
            _view.Dispatcher.AcceleratorKeyActivated -= this.OnAcceleratorKeyActivated;
            _view.GotFocus -= this.OnGotFocus;
            _view.LostFocus -= this.OnLostFocus;
            _focused = null;
        }

        private ModalFocusTracker(UIElement view, Control firstControl)
        {
            _view = view;
            _firstControl = firstControl;
            _focused = null;
            _view.Dispatcher.AcceleratorKeyActivated += this.OnAcceleratorKeyActivated;
            _view.GotFocus += this.OnGotFocus;
            _view.LostFocus += this.OnLostFocus;
        }

        private void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            if (CoreAcceleratorKeyEventType.KeyDown == e.EventType)
            {
                switch (e.VirtualKey)
                {
                    case VirtualKey.Tab:
                        if (null == _focused)
                        {
                            _firstControl.Focus(FocusState.Keyboard);
                            e.Handled = true;
                        }
                        break;
                }
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            _focused = e.OriginalSource as Control;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            _focused = null;
        }
    }
}
