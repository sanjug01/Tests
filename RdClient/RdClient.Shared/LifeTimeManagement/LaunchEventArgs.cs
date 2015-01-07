namespace RdClient.Shared.LifeTimeManagement
{
    using System;
    using Windows.ApplicationModel.Activation;

    public sealed class LaunchEventArgs : EventArgs
    {
        private readonly ApplicationExecutionState _previousState;
        private readonly IActivationArgs _activationArgs;

        public ApplicationExecutionState PreviousState { get { return _previousState; } }
        public IActivationArgs ActivationArgs { get { return _activationArgs; } }

        public LaunchEventArgs(ApplicationExecutionState previousState, IActivationArgs activationArgs)
        {
            _previousState = previousState;
            _activationArgs = activationArgs;
        }
    }
}
