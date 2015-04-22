namespace RdClient.Input
{
    using System;
    using RdClient.Shared.Input.Keyboard;
    using Windows.UI.ViewManagement;
    using Windows.Foundation;


    /// <summary>
    /// Implementation of IInputPanelFactory that creates IInputPanel objects based
    /// on the InputPane class.
    /// </summary>
    sealed class InputPaneInputPanelFactory : IInputPanelFactory
    {
        private sealed class InputPanel : IInputPanel
        {
            private readonly InputPane _inputPane;
            private EventHandler _isVisibleChanged;
            private bool _isVisible;

            public InputPanel()
            {
                _inputPane = InputPane.GetForCurrentView();
                _inputPane.Showing += this.OnShowing;
                _inputPane.Hiding += this.OnHiding;

                Rect orc = _inputPane.OccludedRect;
                _isVisible = 0 != orc.Width || 0 != orc.Height || 0 != orc.Left || 0 != orc.Top;
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
                _inputPane.TryHide();
            }

            void IInputPanel.Show()
            {
                _inputPane.TryShow();
            }

            private void OnShowing(InputPane source, InputPaneVisibilityEventArgs e)
            {
                if (!_isVisible)
                {
                    _isVisible = true;
                    EmitIsVisibleChanged();
                }
            }

            private void OnHiding(InputPane source, InputPaneVisibilityEventArgs e)
            {
                if(_isVisible)
                {
                    _isVisible = false;
                    EmitIsVisibleChanged();
                }
            }

            private void EmitIsVisibleChanged()
            {
                if (null != _isVisibleChanged)
                    _isVisibleChanged(this, EventArgs.Empty);
            }
        }

        IInputPanel IInputPanelFactory.GetInputPanel()
        {
            return new InputPanel();
        }
    }
}
