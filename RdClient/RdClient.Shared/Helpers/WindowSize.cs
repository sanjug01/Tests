using System;
using RdClient.Shared;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace RdClient.Helpers
{
    public class WindowSize : IWindowSize
    {
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