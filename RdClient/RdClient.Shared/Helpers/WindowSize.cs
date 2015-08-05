using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace RdClient.Shared.Helpers
{
    public class WindowSizeChangedEventArgs
    {
        private readonly Size _size;
        public Size Size
        {
            get
            {
                return _size;
            }
        }

        public WindowSizeChangedEventArgs(Size size)
        {
            _size = size;
        }
    }

    public enum WindowActivation
    {
        Activated,
        Deactivated
    }

    public class WindowActivatedEventArgs
    {
        private readonly WindowActivation _windowActivation;

        public WindowActivation WindowActivation
        {
            get
            {
                return _windowActivation;
            }
        }

        public WindowActivatedEventArgs(WindowActivation windowActivation)
        {
            _windowActivation = windowActivation;
        }
    }

    public class WindowSize : IWindowSize
    {
        public event EventHandler<WindowSizeChangedEventArgs> SizeChanged;
        public event EventHandler<WindowActivatedEventArgs> Activated;

        public WindowSize()
        {
            Window.Current.CoreWindow.SizeChanged += (s, o) => EmitSizeChanged(new WindowSizeChangedEventArgs(o.Size));
            Window.Current.CoreWindow.Activated += (s, o) =>
            {
                if(o.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
                {
                    EmitActivated(new WindowActivatedEventArgs(WindowActivation.Deactivated));
                }
                else
                {
                    EmitActivated(new WindowActivatedEventArgs(WindowActivation.Activated));
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