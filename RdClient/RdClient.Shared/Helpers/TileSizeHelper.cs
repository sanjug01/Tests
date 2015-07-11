using RdClient.Shared.CxWrappers;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Helpers
{
    public class TileSizeHelper
    {
        private IScreenProperties _screenProp;
        private IWindowSize _windowSize;
        private Size _tileSize;

        private const double DesiredAspectRatio = 0.5625;


        public TileSizeHelper()
        {
            _tileSize = new Size(296, 164);
            _windowSize = null;
            _screenProp = null;
        }

        public Size TileSize
        {
            get
            {
                return _tileSize;
            }
        }

        public IWindowSize WindowSize
        {
            private get { return _windowSize; }
            set
            {
                _windowSize = value;
                UpdateValues();
            }
        }

        public IScreenProperties ScreenProperties
        {
            private get { return _screenProp; }
            set
            {
                _screenProp = value;
                UpdateValues();
            }
        }

        private static double RoundSize(double value)
        {
            return (4 * Math.Floor(value / 4.0));
        }

        private void UpdateValues()
        {
            double screenDimension;

            if(null == _screenProp || null == _windowSize)
            {
                // pending updates, use default value

                return;
            }

            if (this.WindowSize.Size.Width < GlobalConstants.NarrowLayoutMaxWidth)
            {
                // phone or narrow layout
                screenDimension = Math.Min(ScreenProperties.Resolution.Width, ScreenProperties.Resolution.Height); // min dimension
                _tileSize.Width = RoundSize((screenDimension - 32) / 2.0);
                _tileSize.Height = RoundSize(_tileSize.Width * DesiredAspectRatio);
            }
            else if (this.ScreenProperties.Resolution.Width <= 1365.0 && this.ScreenProperties.Resolution.Height <= 1365.0)
            {
                // medium screens, max dimension <= 1365
                screenDimension = Math.Max(ScreenProperties.Resolution.Width, ScreenProperties.Resolution.Height); // max dimension
                _tileSize.Width = RoundSize( (screenDimension -72 ) / 4.0 );
                _tileSize.Height = RoundSize(_tileSize.Width * DesiredAspectRatio);
            }
            else
            {
                // large screens, max dimensions > 1365
                screenDimension = Math.Max(ScreenProperties.Resolution.Width, ScreenProperties.Resolution.Height); // max dimension
                _tileSize.Width = RoundSize((screenDimension - 80) / 5.0);
                _tileSize.Height = RoundSize(_tileSize.Width * DesiredAspectRatio);
            }
        }
    }
}
