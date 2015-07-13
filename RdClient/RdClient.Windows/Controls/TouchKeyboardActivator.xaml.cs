namespace RdClient.Controls
{
    using RdClient.Shared.Input.Keyboard;
    using RdClient.Shared.Telemetry;
    using System;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class TouchKeyboardActivator : UserControl, IInputPanel
    {
        private readonly InputPane _inputPane;
        private ITelemetryClient _telemetryClient;
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

        public ITelemetryClient TelemetryClient
        {
            get { return _telemetryClient; }
            set { _telemetryClient = value; }
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
            ReportTelemetryEvent("action", "hide");
        }

        void IInputPanel.Show()
        {
            //
            // Set focus to the hidden text box in the pointer mode, which may under the right circumstances
            // invoke the touch keyboard.
            //
            this.HiddenTextBox.Focus(FocusState.Pointer);
            _inputPane.TryShow();
            ReportTelemetryEvent("action", "show");
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
            ReportTelemetryEvent("event", "showing");
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
            ReportTelemetryEvent("event", "hiding");
        }

        private void ReportTelemetryEvent( string eventType, string value )
        {

            if (null != _telemetryClient)
            {
                ITelemetryEvent te = _telemetryClient.MakeEvent("TouchKeyboard");
                te.AddTag(eventType, value);
                te.Report();
            }
        }
    }
}
