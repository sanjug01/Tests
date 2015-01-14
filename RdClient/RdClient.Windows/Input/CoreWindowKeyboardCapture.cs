namespace RdClient.Input
{
    using RdClient.Shared.Input;
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
        private CoreWindowKeyboardCore _core;

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

                _core.Dispose();
                _core = null;
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
            Contract.Assert(null == _core);
            Contract.Ensures(null != _core);
            _core = new CoreWindowKeyboardCore(this);
        }

        void IKeyboardCapture.Stop()
        {
            if (null != _core)
            {
                _core.Dispose();
                _core = null;
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
            if(null != _core)
            {
                _core.ProcessAcceleratorKeyEvent(e.EventType, e.VirtualKey, e.KeyStatus);
            }
        }

        private void OnWindowActivated(CoreWindow sender, WindowActivatedEventArgs e)
        {
            if (null != _core)
            {
                switch (e.WindowActivationState)
                {
                    case CoreWindowActivationState.CodeActivated:
                    case CoreWindowActivationState.PointerActivated:
                        break;

                    case CoreWindowActivationState.Deactivated:
                        _core.ClearState();
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
        }
    }
}
