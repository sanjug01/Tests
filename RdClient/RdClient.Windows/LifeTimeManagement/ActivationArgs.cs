using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace RdClient.LifeTimeManagement
{
    public class ActivationArgs : IActivationArgs
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

        public ActivationArgs(
            string arguments,
            string titleId,
            ActivationKind kind,
            ApplicationExecutionState previousExecutionState,
            SplashScreen splashscreen,
            int currentlyShownApplicationViewId,
            bool prelaunchActivated)
        {
            _arguments = arguments;
            _titleId = titleId;
            _kind = kind;
            _previousExecutionState = previousExecutionState;
            _splashScreen = splashscreen;
            _currentlyShownApplicationViewId = currentlyShownApplicationViewId;
            _prelaunchActivated = prelaunchActivated;
        }
    }
}
