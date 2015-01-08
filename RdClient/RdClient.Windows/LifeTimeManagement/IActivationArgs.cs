using Windows.ApplicationModel.Activation;

namespace RdClient.LifeTimeManagement
{
    public interface IActivationArgs : ILaunchActivatedEventArgs, IActivatedEventArgs, IApplicationViewActivatedEventArgs, IPrelaunchActivatedEventArgs
    {
        object Parameter { get; set; }
    }
}
