using RdClient.LifeTimeManagement;
using Windows.UI.Xaml.Controls;

namespace RdClient.Windows.Test.Mock
{
    class RootFrameManager : IRootFrameManager
    {
        private IActivationArgs _launchActivatedArgs;
        private int _loadedCount = 0;
        private int _activatedCount = 0;

        public IActivationArgs LaunchActivatedArgs
        { 
            get { 
                return _launchActivatedArgs; 
            } 
        }

        public int LoadedCount
        {
            get
            {
                return _loadedCount;
            }
        }

        public int ActivatedCount
        {
            get {
                return _activatedCount;
            }
        }

        public void LoadRoot(IActivationArgs e)
        {
            _loadedCount += 1;
            _launchActivatedArgs = e;
        }

        public void Activate()
        {
            _activatedCount += 1;
        }

        public Frame RootFrame
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
