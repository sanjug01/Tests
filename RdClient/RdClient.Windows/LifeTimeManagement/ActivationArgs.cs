namespace RdClient.LifeTimeManagement
{
    using RdClient.Shared.LifeTimeManagement;
    using Windows.ApplicationModel.Activation;

    public class ActivationArgs : IActivationArgs, IApplicationViewActivatedEventArgs, IPrelaunchActivatedEventArgs
    {
        private string _arguments;
        public string Arguments { get { return _arguments; } }

        private string _titleId;
        public string TileId { get { return _titleId; } }

        private ActivationKind _kind;
        public ActivationKind Kind { get { return _kind; } }

        private ApplicationExecutionState _previousExecutionState;
        public ApplicationExecutionState PreviousExecutionState { get { return _previousExecutionState;  } }

        private SplashScreen _splashScreen;
        public SplashScreen SplashScreen { get { return _splashScreen; } }

        private int _currentlyShownApplicationViewId;
        public int CurrentlyShownApplicationViewId { get { return _currentlyShownApplicationViewId; } }

        private bool _prelaunchActivated;
        public bool PrelaunchActivated { get { return _prelaunchActivated; } }

        private object _parameter;
        public object Parameter { get { return _parameter; } set { _parameter = value; } }

        public ActivationArgs(
            string arguments,
            string titleId,
            ActivationKind kind,
            ApplicationExecutionState previousExecutionState,
            SplashScreen splashscreen,
            int currentlyShownApplicationViewId,
            bool prelaunchActivated,
            object parameter)
        {
            _arguments = arguments;
            _titleId = titleId;
            _kind = kind;
            _previousExecutionState = previousExecutionState;
            _splashScreen = splashscreen;
            _currentlyShownApplicationViewId = currentlyShownApplicationViewId;
            _prelaunchActivated = prelaunchActivated;
            _parameter = parameter;
        }
    }
}
