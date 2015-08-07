
using System;
using Windows.Foundation;

namespace RdClient.Shared.Helpers
{
    public interface IWindowSize
    {
        event EventHandler<WindowSizeChangedEventArgs> SizeChanged;
        event EventHandler<WindowActivatedEventArgs> Activated;
        Size Size { get; }
    }
}
