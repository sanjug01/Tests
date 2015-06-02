namespace RdClient.Controls
{
    using RdClient.Shared.Input.Keyboard;
    using System;
    using System.Diagnostics;
    using Windows.Foundation;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class TouchKeyboardActivator : UserControl, IInputPanel
    {
        private readonly InputPane _inputPane;
        private EventHandler _isVisibleChanged;

        public TouchKeyboardActivator()
        {
            this.InitializeComponent();
            this.HiddenTextBox.TextChanged += this.OnTextChanged;
            _inputPane = InputPane.GetForCurrentView();
            _inputPane.Showing += this.OnInputPaneShowing;
            _inputPane.Showing += this.OnInputPaneHiding;
        }

        bool IInputPanel.IsVisible
        {
            get
            {
                Rect rect = InputPane.GetForCurrentView().OccludedRect;
                Debug.WriteLine("InputPane={0},{1}-{2},{3}", rect.Left, rect.Top, rect.Width, rect.Height);
                return 0 != rect.Height && 0 != rect.Width;
            }
        }

        event EventHandler IInputPanel.IsVisibleChanged
        {
            add { _isVisibleChanged += value; }
            remove { _isVisibleChanged -= value; }
        }

        void IInputPanel.Hide()
        {
            _inputPane.TryHide();
        }

        void IInputPanel.Show()
        {
            this.HiddenTextBox.Focus(FocusState.Keyboard);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //
            // Clear the text box immediately
            //
            ((TextBox)sender).Text = string.Empty;
        }

        private void EmitIsVisibleChanged()
        {
            if (null != _isVisibleChanged)
                _isVisibleChanged(this, EventArgs.Empty);
        }

        private void OnInputPaneShowing(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            EmitIsVisibleChanged();
        }

        private void OnInputPaneHiding(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            EmitIsVisibleChanged();
        }
    }
}
