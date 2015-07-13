using RdClient.Shared.CxWrappers;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Helpers
{
    public class TileSizeHelper
    {
        private double _tileWidth, _tileHeight;
        private const double DesiredAspectRatio = 0.5625;


        public TileSizeHelper()
        {
            _tileWidth = 296.0;
            _tileHeight = 164.0;
        }

        public Size TileSize
        {
            get
            {
                this.UpdateValues();
                return new Size(_tileWidth, _tileHeight);
            }
        }

        public IWindowSize WindowSize { private get; set; }

        private static double RoundSize(double value)
        {
            return (4 * Math.Floor(value / 4.0));
        }

        private void UpdateValues()
        {
            double screenDimension;

            if(null == this.WindowSize)
            {
                // pending updates, use default value
                return;
            }

            if (this.WindowSize.Size.Width < GlobalConstants.NarrowLayoutMaxWidth)
            {
                // phone or narrow layout
                screenDimension = Math.Min(WindowSize.Size.Width, WindowSize.Size.Height); // min dimension

                _tileWidth = RoundSize((screenDimension - 32) / 2.0);
                _tileHeight = RoundSize(_tileWidth * DesiredAspectRatio);
            }
            else if (this.WindowSize.Size.Width <= 1365.0 && this.WindowSize.Size.Height <= 1365.0)
            {
                // medium screens, max dimension <= 1365
                screenDimension = Math.Max(WindowSize.Size.Width, WindowSize.Size.Height); // max dimension
                _tileWidth = RoundSize( (screenDimension -72 ) / 4.0 );
                _tileHeight = RoundSize(_tileWidth * DesiredAspectRatio);
            }
            else
            {
                // large screens, max dimensions > 1365
                screenDimension = Math.Max(WindowSize.Size.Width, WindowSize.Size.Height); // max dimension
                _tileWidth = RoundSize((screenDimension - 80) / 5.0);
                _tileHeight = RoundSize(_tileWidth * DesiredAspectRatio);
            }
        }
    }
}
