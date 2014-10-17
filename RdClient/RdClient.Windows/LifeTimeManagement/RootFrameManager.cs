using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.LifeTimeManagement
{
    class RootFrameManager : IRootFrameManager
    {
        public Frame RootFrame
        {
            get {
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
                if (!RootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
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
