using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient
{
    class LifeTimeManager
    {
        private Window _currentWindow;

        public void Initialize(Window currentWindow)
        {
            _currentWindow = currentWindow;
        }

        public void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (_currentWindow == null)
            {
                throw new Exception("The LifeTimeManager needs to be Initialized()");
            }

            Frame rootFrame = _currentWindow.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                _currentWindow.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            _currentWindow.Activate();
        }

        public void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity

            deferral.Complete();
        }

    }
}
