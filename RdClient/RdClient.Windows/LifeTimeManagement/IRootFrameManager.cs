using Windows.UI.Xaml.Controls;

namespace RdClient.LifeTimeManagement
{
    public interface IRootFrameManager
    {
        Frame RootFrame { get; }
        void LoadRoot(IActivationArgs e);
        void Activate();
    }
}
