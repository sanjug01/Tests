using RdClient.Shared.Helpers;
using System;
using Windows.UI.ViewManagement;


namespace RdClient.Shared.Models
{
    public class FullScreenModel : IFullScreenModel
    {
        private bool _changing;
        private IFullScreen _fullScreen;
        private EventHandler _enteringFullScreen, _exitingFullScreen, _enteredFullScreen, _exitedFullScreen;

        public FullScreenModel()
        {
            _changing = false;
        }

        public ITimerFactory TimerFactory { get; set; }
        public IWindowSize WindowSize
        {
            set
            {
                value.SizeChanged += (s, o) => FullScreenDebouncer();
            }
        }

        event EventHandler IFullScreenModel.EnteringFullScreen
        {
            add { _enteringFullScreen += value; }
            remove { _enteringFullScreen -= value; }
        }

        event EventHandler IFullScreenModel.EnteredFullScreen
        {
            add { _enteredFullScreen += value; }
            remove { _enteredFullScreen -= value; }
        }

        event EventHandler IFullScreenModel.ExitingFullScreen
        {
            add { _exitingFullScreen += value; }
            remove { _exitingFullScreen -= value; }
        }

        event EventHandler IFullScreenModel.ExitedFullScreen
        {
            add { _exitedFullScreen += value; }
            remove { _exitedFullScreen -= value; }
        }


        private void FullScreenDebouncer()
        {
            if(_changing)
            {
                if(_fullScreen.IsFullScreenMode)
                {
                    EmitEnteredFullScreen();
                }
                else
                {
                    EmitExitedFullScreen();
                }
            }
        }

        private Debouncer _enterFullScreenDebouncer;
        private void EmitEnteredFullScreen()
        {
            if(this.TimerFactory != null)
            {
                if(_enterFullScreenDebouncer == null)
                {
                    _enterFullScreenDebouncer = new Debouncer(_EmitEnteredFullScreen, this.TimerFactory.CreateTimer(), TimeSpan.FromMilliseconds(100));
                }

                _enterFullScreenDebouncer.Call();
            }
        }

        private void _EmitEnteredFullScreen()
        {
            if(_enteredFullScreen != null)
            {
                _enteredFullScreen(this, EventArgs.Empty);
            }

            _enterFullScreenDebouncer = null;
            _changing = false;
        }

        private Debouncer _exitFullScreenDebouncer;
        private void EmitExitedFullScreen()
        {
            if(this.TimerFactory != null)
            {
                if(_exitFullScreenDebouncer == null)
                {
                    _exitFullScreenDebouncer = new Debouncer(_EmitExitedFullScreen, this.TimerFactory.CreateTimer(), TimeSpan.FromMilliseconds(100));
                }

                _exitFullScreenDebouncer.Call();
            }
        }

        private void _EmitExitedFullScreen()
        {
            if(_exitedFullScreen != null)
            {
                _exitedFullScreen(this, EventArgs.Empty);
            }

            _exitFullScreenDebouncer = null;
            _changing = false;
        }

        public IFullScreen FullScreen
        {
            set
            {
                _fullScreen = value;
                _fullScreen.UserInteractionModeChanged += (s, o) => EmitUserInteractionModeChange();
                _fullScreen.IsFullScreenModeChanged += (s, o) => EmitFullScreenChange();
            }
        }

        public void EnterFullScreen()
        {
            if (null != _enteringFullScreen)
                _enteringFullScreen(this, EventArgs.Empty);
            _changing = true;
            _fullScreen.EnterFullScreen();
        }

        public void ExitFullScreen()
        {
            if (null != _exitingFullScreen)
                _exitingFullScreen(this, EventArgs.Empty);
            _changing = true;
            _fullScreen.ExitFullScreen();
        }

        public event EventHandler FullScreenChange;
        private void EmitFullScreenChange()
        {
            if(FullScreenChange != null)
            {
                FullScreenChange(this, EventArgs.Empty);
            }
        }

        public event EventHandler UserInteractionModeChange;
        private void EmitUserInteractionModeChange()
        {
            if(UserInteractionModeChange != null)
            {
                UserInteractionModeChange(this, EventArgs.Empty);
            }
        }

        public UserInteractionMode UserInteractionMode
        {
            get
            {
                return _fullScreen.UserInteractionMode;
            }
        }

        public bool IsFullScreenMode
        {
            get
            {
                return _fullScreen.IsFullScreenMode;
            }
        }
    }
}
