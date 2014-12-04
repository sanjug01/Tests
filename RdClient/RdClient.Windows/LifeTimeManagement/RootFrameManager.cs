using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using System.Runtime.Serialization;
using Windows.Storage;
using RdClient.Models;
using System.Threading.Tasks;

namespace RdClient.LifeTimeManagement
{
    class RootFrameManager : IRootFrameManager
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
