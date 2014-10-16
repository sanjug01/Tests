using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace RdClient
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {

        private LifeTimeManager _lifeTimeManager;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            _lifeTimeManager = new LifeTimeManager();
            _lifeTimeManager.Initialize(Window.Current);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            _lifeTimeManager.OnLaunched(e);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            _lifeTimeManager.OnSuspending(sender, e);
        }
    }
}