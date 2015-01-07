namespace RdClient.LifeTimeManagement
{
    using RdClient.Shared.Helpers;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed class RootFrameManager : IRootFrameManager
    {
        public Frame RootFrame
        {
            get
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame == null)
                {
                    rootFrame = new Frame();

                    rootFrame.CacheSize = 1;

                    Window.Current.Content = rootFrame;
                }

                return rootFrame;
            }
        }

        public void LoadRoot(IActivationArgs e)
        {
            if (RootFrame.Content == null)
            {
                if (!RootFrame.Navigate(typeof(MainPage), e.Parameter))
                {
                    throw new RdClientException("Failed to create initial page");
                }
            }

            Activate();
        }

        public void Activate()
        {
            Window.Current.Activate();
        }
    }
}
