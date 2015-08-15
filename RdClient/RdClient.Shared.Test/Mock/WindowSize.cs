using RdClient.Shared.Helpers;
using RdMock;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Test.Mock
{
    public class WindowSize : MockBase, IWindowSize
    {
        public Size Size
        {
            get; set;
        }

        public event EventHandler<WindowActivatedEventArgs> Activated;

        public void EmitActivated(WindowActivation activation)
        {
            if(Activated != null)
            {
                Activated(this, new WindowActivatedEventArgs(activation));
            }
        }

        public event EventHandler<WindowSizeChangedEventArgs> SizeChanged;

        public void EmitSizeChanged(Size size)
        {
            if(SizeChanged != null)
            {
                SizeChanged(this, new WindowSizeChangedEventArgs(size));
            }
        }
    }
}
