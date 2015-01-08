using RdClient.Shared.LifeTimeManagement;
using Windows.UI.Xaml.Controls;

namespace RdClient.Shared.LifeTimeManagement
{
    public interface IRootFrameManager
    {
        Frame RootFrame { get; }
        void LoadRoot(IActivationArgs e);
        void Activate();
    }
}
