namespace RdClient.Controls
{
    using RdClient.Shared.Input.Keyboard;
    using System;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class TouchKeyboardActivator : UserControl, IInputPanel
    {
        private readonly InputPane _inputPane;
        private EventHandler _isVisibleChanged;
        private bool _isVisible;

        public TouchKeyboardActivator()
        {
            this.InitializeComponent();
            this.HiddenTextBox.TextChanged += this.OnTextChanged;
            _inputPane = InputPane.GetForCurrentView();
            _inputPane.Showing += this.OnInputPaneShowing;
            _inputPane.Hiding += this.OnInputPaneHiding;
        }

        bool IInputPanel.IsVisible
        {
            get { return _isVisible; }
        }

        event EventHandler IInputPanel.IsVisibleChanged
        {
            add { _isVisibleChanged += value; }
            remove { _isVisibleChanged -= value; }
        }

        void IInputPanel.Hide()
        {
            this.DummyButton.Focus(FocusState.Programmatic);
            _inputPane.TryHide();
        }

        void IInputPanel.Show()
        {
            //
            // Set focus to the hidden text box in the pointer mode, which may under the right circumstances
            // invoke the touch keyboard.
            //
            this.HiddenTextBox.Focus(FocusState.Pointer);
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
            _isVisible = true;
            EmitIsVisibleChanged();
        }

        private void OnInputPaneHiding(InputPane sender, InputPaneVisibilityEventArgs e)
        {
            _isVisible = false;
            //
            // User may hide the keyboard by tapping its "close" button, in which case input focus must go
            // away from the edit box.
            //
            this.DummyButton.Focus(FocusState.Programmatic);
            EmitIsVisibleChanged();
        }
    }
}
