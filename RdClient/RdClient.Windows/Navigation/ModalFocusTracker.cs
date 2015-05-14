namespace RdClient.Navigation
{
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using Windows.System;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media;

    sealed class ModalFocusTracker
    {
        private readonly IDialogViewModel _dialogViewModel;
        private readonly UIElement _view;
        private Control _focused;

        public static ModalFocusTracker Install(IPresentableView view)
        {
            Contract.Assert(null != view);
            return new ModalFocusTracker(view);
        }

        public void Uninstall()
        {
            _view.Dispatcher.AcceleratorKeyActivated -= this.OnAcceleratorKeyActivated;
            _view.GotFocus -= this.OnGotFocus;
            _view.LostFocus -= this.OnLostFocus;
            _focused = null;
        }

        private ModalFocusTracker(IPresentableView view)
        {
            Contract.Assert(view is UIElement);

            _view = (UIElement)view;
            _dialogViewModel = view.ViewModel as IDialogViewModel;
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
                            e.Handled = FocusFirstTabControl();
                        }
                        break;

                    case VirtualKey.Escape:
                        //
                        // Execute the Cancel command in the dialog view model.
                        //
                        if (null != _dialogViewModel && null != _dialogViewModel.Cancel && _dialogViewModel.Cancel.CanExecute(null))
                        {
                            _dialogViewModel.Cancel.Execute(null);
                            e.Handled = true;
                        }
                        break;

                    case VirtualKey.Enter:
                        //
                        // Execute the DefaultAction command in the dialog view model.
                        // The default action is executed only if the control that has input focus is not a button (any kind of a button),
                        // and there is a default action command, and the command can be executed.
                        //
                        if (null != _dialogViewModel
                            && !(_focused is ButtonBase)
                            && null != _dialogViewModel.DefaultAction
                            && _dialogViewModel.DefaultAction.CanExecute(null))
                        {
                            _dialogViewModel.DefaultAction.Execute(null);
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

        private bool FocusFirstTabControl()
        {
            bool focused = false;
            Control control = FindFirstTabControl(_view);

            if (null != control)
            {
                control.Focus(FocusState.Keyboard);
                focused = true;
            }

            return focused;
        }

        private Control FindFirstTabControl(DependencyObject parent)
        {
            Contract.Assert(null != parent);

            Control firstTabControl = null;
            int childrenNumber = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; null == firstTabControl && i < childrenNumber; ++i)
            {
                UIElement element = GetActiveElement(VisualTreeHelper.GetChild(parent, i));

                if (null != element)
                {
                    //
                    // Look in the element's children first.
                    //
                    firstTabControl = FindFirstTabControl(element) ?? GetTabControl(element);
                }
            }

            if (null == firstTabControl)
            {
                firstTabControl = GetTabControl(GetActiveElement(parent));
            }

            return firstTabControl;
        }

        private UIElement GetActiveElement(DependencyObject obj)
        {
            UIElement element = obj as UIElement;

            return (null != element && Visibility.Visible == element.Visibility && element.IsHitTestVisible) ? element : null;
        }

        private Control GetTabControl(UIElement element)
        {
            Control control = element as Control;

            return (null != control && control.IsEnabled && control.IsTabStop) ? control : null;
        }
    }
}
