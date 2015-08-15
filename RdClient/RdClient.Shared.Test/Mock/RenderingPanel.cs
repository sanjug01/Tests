using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using RdClient.Shared.Models.Viewport;
using RdMock;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using RdClient.Shared.Helpers;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace RdClient.Shared.Test.Mock
{
    public class RenderingPanel : MockBase, IRenderingPanel
    {
        public event EventHandler Ready;

        public IViewport Viewport
        {
            get;
            set;
        }

        public IScaleFactor ScaleFactor
        {
            get;
            set;            
        }

        public void OnMouseVisibilityChanged(object sender, PropertyChangedEventArgs e)
        {
            Invoke(new object[] { sender, e });
        }

        public void ChangeMouseCursorShape(ImageSource shape, Point hotspot)
        {
            Invoke(new object[] { shape, hotspot });
        }

        public void MoveMouseCursor(Windows.Foundation.Point point)
        {
            Invoke(new object[] { point });
        }

        public void EmitRead()
        {
            Ready(this, EventArgs.Empty);
        }

        public void ChangeMouseVisibility(Visibility visibility)
        {
            Invoke(new object[] { visibility });
        }

        public void ScaleMouseCursor(double scale)
        {
            Invoke(new object[] { scale });
        }
    }
}
