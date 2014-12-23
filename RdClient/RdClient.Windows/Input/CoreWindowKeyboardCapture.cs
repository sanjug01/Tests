namespace RdClient.Input
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input;
    using System;
    using System.Diagnostics.Contracts;
    using Windows.UI.Core;

    public sealed class CoreWindowKeyboardCapture : MutableObject, IKeyboardCapture, IKeyboardCaptureSink
    {
        private EventHandler<KeystrokeEventArgs> _keystroke;
        private CoreWindowKeyboardCore _core;

        public CoreWindowKeyboardCapture()
        {
            CoreWindow cw = CoreWindow.GetForCurrentThread();

            _core = new CoreWindowKeyboardCore(this);

            cw.Activated += this.OnWindowActivated;
            cw.Closed += this.OnWindowClosed;
            cw.KeyDown += this.OnWindowKeyDown;
            cw.KeyUp += this.OnWindowKeyUp;
            cw.CharacterReceived += this.OnWindowCharacterReceived;
        }

        protected override void DisposeManagedState()
        {
            base.DisposeManagedState();
            _core.Dispose();
            _core = null;
        }

        event EventHandler<KeystrokeEventArgs> IKeyboardCapture.Keystroke
        {
            add
            {
                using(LockWrite())
                    _keystroke += value;
            }

            remove
            {
                using (LockWrite())
                    _keystroke -= value;
            }
        }

        void IKeyboardCapture.Start()
        {
            Contract.Assert(null == _core);
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

            using(LockUngradeableRead())
            {
                if (null != _keystroke)
                    _keystroke(this, e);
            }
        }

        private void OnWindowActivated(CoreWindow sender, WindowActivatedEventArgs e)
        {
        }

        private void OnWindowClosed(CoreWindow sender, CoreWindowEventArgs e)
        {
            sender.Activated -= this.OnWindowActivated;
            sender.Closed -= this.OnWindowClosed;
            sender.KeyDown -= this.OnWindowKeyDown;
            sender.KeyUp -= this.OnWindowKeyUp;
            sender.CharacterReceived -= this.OnWindowCharacterReceived;
        }

        private void OnWindowKeyDown(CoreWindow sender, KeyEventArgs e)
        {
            if(null != _core)
            {
                _core.ProcessKeyDown(e.VirtualKey, e.KeyStatus);
            }
        }

        private void OnWindowKeyUp(CoreWindow sender, KeyEventArgs e)
        {
            if (null != _core)
            {
                _core.ProcessKeyUp(e.VirtualKey, e.KeyStatus);
            }
        }

        private void OnWindowCharacterReceived(CoreWindow sender, CharacterReceivedEventArgs e)
        {
            if (null != _core)
            {
                _core.ProcessCharacterReceived(e.KeyCode, e.KeyStatus);
            }
        }
    }
}
