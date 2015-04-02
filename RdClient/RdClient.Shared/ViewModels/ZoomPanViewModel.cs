namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Input.ZoomPan;

    public class ZoomTransform : IZoomPanTransform
    {
        public TransformType TransformType { get; private set; }

        public ZoomTransform(TransformType type)
        {
            TransformType = type;
        }
    }

    public class CustomZoomTransform : ICustomZoomTransform
    {
        public CustomZoomTransform(double centerX, double centerY, double scaleX, double scaleY)
        {
            TransformType = TransformType .ZoomCustom;
            CenterX = centerX;
            CenterY = centerY;
            ScaleX = scaleX;
            ScaleY = scaleY;
        }

        public TransformType TransformType { get; private set; }
        public double CenterX { get; private set; }
        public double CenterY { get; private set; }
        public double ScaleX { get; private set; }
        public double ScaleY { get; private set; }
    }

    public class PanTransform : IPanTransform
    {
        public TransformType TransformType { get; private set; }
        public const double DefaultPan = 50.0;
        public double X { get; private set; }
        public double Y { get; private set; }
        public PanTransform(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.TransformType = TransformType.Pan;
        }
    }

    // specific zoom/pan instances to be used as parameters
    public sealed class ZoomInTransform : ZoomTransform
    {
        public ZoomInTransform() : base (TransformType.ZoomIn) { }
    }

    public sealed class ZoomOutTransform : ZoomTransform
    {
        public ZoomOutTransform() : base (TransformType.ZoomOut) { }
    }
}
