namespace RdClient.Shared.Telemetry.Events
{
    using System;

    /// <summary>
    /// Session input telemetry data point that captures user's actions in the session view.
    /// The datra point is reported after final termination of the session.
    /// </summary>
    public sealed class SessionInput
    {
        private int _enterFullScreen;
        private int _exitFullScreen;
        private int _menusShown;
        private double _firstExit, _firstEntry;

        /// <summary>
        /// Number of times the session had entered the full screen mode by user's request (the initial entry is not included in this counter).
        /// </summary>
        public int enterFullScreen
        {
            get { return _enterFullScreen; }
        }

        /// <summary>
        /// Number of times the session has exited the full screen mode upon a user's request.
        /// </summary>
        public int exitFullScreen
        {
            get { return _exitFullScreen; }
        }

        /// <summary>
        /// Number of times the in-session menus had been shown.
        /// </summary>
        public int menusShown
        {
            get { return _menusShown; }
        }

        /// <summary>
        /// Time before connecting the session and first exit out of the full screen mode upon user's request.
        /// If user hadn't requested to exit out of the full screen mode, value of the metric is 0
        /// </summary>
        public double firstFullScreenExit
        {
            get { return _firstExit; }
        }

        /// <summary>
        /// Time before connecting the session and first entering the full screen mode upon user's request.
        /// </summary>
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
