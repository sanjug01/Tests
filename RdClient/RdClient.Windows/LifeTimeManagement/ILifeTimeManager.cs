using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

namespace RdClient.LifeTimeManagement
{
    public interface ILifeTimeManager
    {
        void OnLaunched(IActivationArgs e);
        void OnSuspending(object sender, ISuspensionArgs e);
    }
}
