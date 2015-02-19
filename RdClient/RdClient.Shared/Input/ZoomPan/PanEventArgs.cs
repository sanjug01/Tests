namespace RdClient.Shared.Input.ZoomPan
{
    using System;

    public sealed class PanEventArgs : EventArgs
    {
        private readonly double _deltaX;
        private readonly double _deltaY;

        public double DeltaX { get { return _deltaX; } }
        public double DeltaY { get { return _deltaY; } }

        public PanEventArgs(double deltaX, double deltaY)
        {
            _deltaX = deltaX;
            _deltaY = deltaY;
        }
    }
}
