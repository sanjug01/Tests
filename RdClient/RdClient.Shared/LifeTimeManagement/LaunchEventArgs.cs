namespace RdClient.Shared.LifeTimeManagement
{
    using System;
    using Windows.ApplicationModel.Activation;

    public sealed class LaunchEventArgs : EventArgs
    {
        private readonly IActivationArgs _activationArgs;

        public IActivationArgs ActivationArgs { get { return _activationArgs; } }

        public LaunchEventArgs(IActivationArgs activationArgs)
        {
            _activationArgs = activationArgs;
        }
    }
}
