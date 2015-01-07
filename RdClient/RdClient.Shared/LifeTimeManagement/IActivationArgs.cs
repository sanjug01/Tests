using Windows.ApplicationModel.Activation;

namespace RdClient.Shared.LifeTimeManagement
{
    public interface IActivationArgs : ILaunchActivatedEventArgs, IActivatedEventArgs
    {
        object Parameter { get; set; }
    }
}
