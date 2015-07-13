using System;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace RdClient.Shared.Helpers
{
    public class WindowSizeChangedEventArgs
    {
        public Size Size { get; set; }
    }

    public class WindowSize : IWindowSize
    {
        public event EventHandler<WindowSizeChangedEventArgs> SizeChanged;

        public WindowSize()
        {
            Window.Current.CoreWindow.SizeChanged += (s, o) => { EmitSizeChanged(new WindowSizeChangedEventArgs() { Size = o.Size }); };
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