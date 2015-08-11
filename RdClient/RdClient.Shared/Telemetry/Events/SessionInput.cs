namespace RdClient.Shared.Telemetry.Events
{
    using System;

    /// <summary>
    /// Session input telemetry data point that captures user's actions in the session view.
    /// </summary>
    public sealed class SessionInput
    {
        private int _enterFullScreen;
        private int _exitFullScreen;
        private int _menusShown;
        private double _firstExit, _firstEntry;

        public int enterFullScreen
        {
            get { return _enterFullScreen; }
        }

        public int exitFullScreen
        {
            get { return _exitFullScreen; }
        }

        public int menusShown
        {
            get { return _menusShown; }
        }

        public double firstFullScreenExit
        {
            get { return _firstExit; }
        }

        public double firstFullScreenEntry
        {
            get { return _firstEntry; }
        }

        public void EnterFullScreen(IStopwatch sessionViewStopwatch)
        {
            if( 0 == _enterFullScreen++)
            {
                _firstEntry = Math.Ceiling(sessionViewStopwatch.Elapsed.TotalSeconds);
            }
        }

        public void ExitFullScreen(IStopwatch sessionViewStopwatch)
        {
            if (0 == _exitFullScreen++)
            {
                _firstExit = Math.Ceiling(sessionViewStopwatch.Elapsed.TotalSeconds);
            }
        }

        public void ShowSessionMenus()
        {
            ++_menusShown;
        }
    }
}
