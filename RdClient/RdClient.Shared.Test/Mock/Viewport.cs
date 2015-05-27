using RdClient.Shared.Models.Viewport;
using RdMock;
using System;
using System.ComponentModel;
using Windows.Foundation;

namespace RdClient.Shared.Test.Mock
{
    public class Viewport : MockBase, IViewport
    {
        public IViewportPanel SessionPanel { get; set; }

        public Point Offset { get; set; }

        public Size Size { get; set; }

        double IViewport.ZoomFactor
        {
            get { return 2.0; }
        }

        public event EventHandler Changed;
        public void EmitChangedEvent()
        {
            if(Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void EmitPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Set(double zoomFactor, Point anchorPoint)
        {
            Invoke(new object[] { zoomFactor, anchorPoint });
        }

        public void PanAndZoom(Point anchorPoint, double dx, double dy, double scaleFactor)
        {
            Invoke(new object[] { anchorPoint, dx, dy, scaleFactor });
        }

        public void SetZoom(double zoomFactor, Point anchorPoint)
        {
            Invoke(new object[] { zoomFactor, anchorPoint });
        }

        public void SetPan(double x, double y)
        {
            Invoke(new object[] { x, y });
        }

        public void Reset()
        {
            Invoke(new object[] { });
        }
    }

}
