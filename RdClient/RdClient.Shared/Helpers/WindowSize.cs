using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace RdClient.Shared.Helpers
{
    public class WindowSizeChangedEventArgs
    {
        public Size Size { get; set; }
    }

    public enum WindowActivation
    {
        Activated,
        Deactivated
    }

    public class WindowActivatedEventArgs
    {
        public WindowActivation WindowActivation { get; set; }
    }

    public class WindowSize : IWindowSize
    {
        public event EventHandler<WindowSizeChangedEventArgs> SizeChanged;
        public event EventHandler<WindowActivatedEventArgs> Activated;

        public WindowSize()
        {
            Window.Current.CoreWindow.SizeChanged += (s, o) => { EmitSizeChanged(new WindowSizeChangedEventArgs() { Size = o.Size }); };
            Window.Current.CoreWindow.Activated += (s, o) =>
            {
                if(o.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
                {
                    EmitActivated(new WindowActivatedEventArgs() { WindowActivation = WindowActivation.Deactivated });
                }
                else
                {
                    EmitActivated(new WindowActivatedEventArgs() { WindowActivation = WindowActivation.Activated });
                }
            };
        }

        private void EmitActivated(WindowActivatedEventArgs e)
        {
            if(Activated != null)
            {
                Activated(this, e);
            }
        }

        private void EmitSizeChanged(WindowSizeChangedEventArgs e)
        {
            if(SizeChanged != null)
            {
                SizeChanged(this, e);
            }
        }

        Size IWindowSize.Size
        {
            get
            {
                Rect bounds = Window.Current.CoreWindow.Bounds;
                return new Size(bounds.Width, bounds.Height);
            }
        }
    }
}