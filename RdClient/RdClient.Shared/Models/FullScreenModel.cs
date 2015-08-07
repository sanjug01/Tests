﻿using RdClient.Shared.Helpers;
using System;
using Windows.UI.ViewManagement;


namespace RdClient.Shared.Models
{
    enum FullScreenChanging
    {
        Entering,
        Exiting,
        Idle
    }

    public class FullScreenModel : IFullScreenModel
    {
        private FullScreenChanging _changing;
        private IFullScreen _fullScreen;
        private Debouncer _enterFullScreenDebouncer;
        private Debouncer _exitFullScreenDebouncer;

        private EventHandler _enteringFullScreen, _exitingFullScreen, _enteredFullScreen, _exitedFullScreen;
        public event EventHandler FullScreenChange;
        public event EventHandler UserInteractionModeChange;

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
        public ITimerFactory TimerFactory { get; set; }

        public IWindowSize WindowSize
        {
            set
            {
                value.SizeChanged += (s, o) => FullScreenDebouncer();
                value.Activated += OnWindowActivationChanged;
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

        public IFullScreen FullScreen
        {
            set
            {
                _fullScreen = value;
                _fullScreen.UserInteractionModeChanged += (s, o) => EmitUserInteractionModeChange();
                _fullScreen.IsFullScreenModeChanged += (s, o) => EmitFullScreenChange();
            }
        }

        public FullScreenModel()
        {
            _changing = FullScreenChanging.Idle;
        }

        public void EnterFullScreen()
        {
            if (null != _enteringFullScreen)
                _enteringFullScreen(this, EventArgs.Empty);
            _changing = FullScreenChanging.Entering;
            _fullScreen.EnterFullScreen();
            FullScreenDebouncer();
        }

        public void ExitFullScreen()
        {
            if (null != _exitingFullScreen)
                _exitingFullScreen(this, EventArgs.Empty);
            _changing = FullScreenChanging.Exiting;
            _fullScreen.ExitFullScreen();
            FullScreenDebouncer();
        }

        private void EmitFullScreenChange()
        {
            if(FullScreenChange != null)
            {
                FullScreenChange(this, EventArgs.Empty);
            }
        }

        private void EmitUserInteractionModeChange()
        {
            if(UserInteractionModeChange != null)
            {
                UserInteractionModeChange(this, EventArgs.Empty);
            }
        }

        private void OnWindowActivationChanged(object sender, WindowActivatedEventArgs e)
        {
            switch(_changing)
            {
                case FullScreenChanging.Entering:
                    if(e.WindowActivation == WindowActivation.Deactivated)
                    {
                        if (_enterFullScreenDebouncer != null)
                        {
                            _enterFullScreenDebouncer.Cancel();
                        }
                    }
                    else if(e.WindowActivation == WindowActivation.Activated)
                    {
                        EnterFullScreen();
                    }
                    break;
                case FullScreenChanging.Exiting:
                    if (e.WindowActivation == WindowActivation.Deactivated)
                    {
                        if (_exitFullScreenDebouncer != null)
                        {
                            _exitFullScreenDebouncer.Cancel();
                        }
                    }
                    else if (e.WindowActivation == WindowActivation.Activated)
                    {
                        ExitFullScreen();
                    }
                    break;
            }
        }

        private void FullScreenDebouncer()
        {
            switch(_changing)
            {
                case FullScreenChanging.Entering:
                    EmitEnteredFullScreen();
                    break;
                case FullScreenChanging.Exiting:
                    EmitExitedFullScreen();
                    break;
            }
        }

        private void EmitEnteredFullScreen()
        {
            if (this.TimerFactory != null)
            {
                if (_enterFullScreenDebouncer == null)
                {
                    _enterFullScreenDebouncer = new Debouncer(_EmitEnteredFullScreen, this.TimerFactory.CreateTimer(), TimeSpan.FromMilliseconds(100));
                }

                _enterFullScreenDebouncer.Call();
            }
        }

        private void _EmitEnteredFullScreen()
        {
            if (_enteredFullScreen != null)
            {
                _enteredFullScreen(this, EventArgs.Empty);
            }

            _enterFullScreenDebouncer = null;
            _changing = FullScreenChanging.Idle;
        }

        private void EmitExitedFullScreen()
        {
            if (this.TimerFactory != null)
            {
                if (_exitFullScreenDebouncer == null)
                {
                    _exitFullScreenDebouncer = new Debouncer(_EmitExitedFullScreen, this.TimerFactory.CreateTimer(), TimeSpan.FromMilliseconds(100));
                }

                _exitFullScreenDebouncer.Call();
            }
        }

        private void _EmitExitedFullScreen()
        {
            if (_exitedFullScreen != null)
            {
                _exitedFullScreen(this, EventArgs.Empty);
            }

            _exitFullScreenDebouncer = null;
            _changing = FullScreenChanging.Idle;
        }
    }
}
