namespace RdClient.Shared.Input.ZoomPan
{
    using System;

    public sealed class ZoomEventArgs : EventArgs
    {
        private readonly double _centerX;
        private readonly double _centerY;

        private readonly double _fromLength;
        private readonly double _toLength;

        public double CenterX { get { return _centerX; } }
        public double CenterY { get { return _centerY; } }
        public double FromLength { get { return _fromLength; } }
        public double ToLength { get { return _toLength; } }


        public ZoomEventArgs(double centerX, double centerY, double fromLength, double toLength)
        {
            _centerX = centerX;
            _centerY = centerY;
            _fromLength = fromLength;
            _toLength = toLength;
        }
    }
}
