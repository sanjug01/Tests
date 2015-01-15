namespace RdClient.Input
{
    using RdClient.Shared.Input.Keyboard;
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using Windows.UI.Core;
    using Windows.UI.Xaml;

    public sealed class CoreWindowKeyboardCapture : DependencyObject, IDisposable, IKeyboardCapture, IKeyboardCaptureSink
    {
        private int _disposed;
        private CoreWindow _coreWindow;
        private EventHandler<KeystrokeEventArgs> _keystroke;
        private KeyboardState _keyboardState;

        public CoreWindowKeyboardCapture()
        {
            _disposed = 0;
        }

        ~CoreWindowKeyboardCapture()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (0 == Interlocked.CompareExchange(ref _disposed, 1, 0))
            {
                if (disposing)
                {
                    if( null != _coreWindow )
                    {
                        this.Dispatcher.AcceleratorKeyActivated -= this.OnAcceleratorKeyActivated;
                        _coreWindow.Activated -= this.OnWindowActivated;
                        _coreWindow.Closed -= this.OnWindowClosed;
                        _coreWindow = null;
                    }
                }

                _keyboardState.Clear();
                _keyboardState = null;
            }
            else
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        event EventHandler<KeystrokeEventArgs> IKeyboardCapture.Keystroke
        {
            add
            {
                if(null == _keystroke)
                {
                    Contract.Assert(null == _coreWindow);
                    _coreWindow = CoreWindow.GetForCurrentThread();
                    Contract.Assert(null != _coreWindow);
                    this.Dispatcher.AcceleratorKeyActivated += this.OnAcceleratorKeyActivated;
                    _coreWindow.Activated += this.OnWindowActivated;
                    _coreWindow.Closed += this.OnWindowClosed;
                }

                _keystroke += value;
            }

            remove
            {
                _keystroke -= value;

                if(null == _keystroke)
                {
                    Contract.Assert(null != _coreWindow);
                    this.Dispatcher.AcceleratorKeyActivated -= this.OnAcceleratorKeyActivated;
                    _coreWindow.Activated -= this.OnWindowActivated;
                    _coreWindow.Closed -= this.OnWindowClosed;
                    _coreWindow = null;
                }
            }
        }

        void IKeyboardCapture.Start()
        {
            Contract.Assert(null == _keyboardState);
            Contract.Ensures(null != _keyboardState);
            _keyboardState = new KeyboardState(this);
        }

        void IKeyboardCapture.Stop()
        {
            if (null != _keyboardState)
            {
                _keyboardState.Clear();
                _keyboardState = null;
            }
        }

        void IKeyboardCaptureSink.ReportKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
        {
            EmitKeystroke(new KeystrokeEventArgs(keyCode, isScanCode, isExtendedKey, isKeyReleased));
        }

        private void EmitKeystroke(KeystrokeEventArgs e)
        {
            Contract.Requires(null != e);

            if (null != _keystroke)
                _keystroke(this, e);
        }

        private void OnAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            if (null != _keyboardState)
            {
                e.Handled = _keyboardState.UpdateState(e.EventType, e.VirtualKey, e.KeyStatus);
            }
        }

        private void OnWindowActivated(CoreWindow sender, WindowActivatedEventArgs e)
        {
            if (null != _keyboardState)
            {
                switch (e.WindowActivationState)
                {
                    case CoreWindowActivationState.CodeActivated:
                    case CoreWindowActivationState.PointerActivated:
                        break;

                    case CoreWindowActivationState.Deactivated:
                        _keyboardState.Clear();
                        break;

                    default:
                        Contract.Assert(false);
                        break;
                }
            }
        }

        private void OnWindowClosed(CoreWindow sender, CoreWindowEventArgs e)
        {
            sender.Activated -= this.OnWindowActivated;
            sender.Closed -= this.OnWindowClosed;
            if (null != _keyboardState)
                _keyboardState.Clear();
        }
    }
}
