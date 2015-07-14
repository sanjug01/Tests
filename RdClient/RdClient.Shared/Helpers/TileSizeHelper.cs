using RdClient.Shared.CxWrappers;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Helpers
{
    public class TileSizeHelper
    {
        private const double DesiredAspectRatio = 0.5625;

        public static Size GetTileSize(Size windowSize)
        {
            double screenDimension = 0;
            double tileWidth, tileHeight;
            if (windowSize.Width < GlobalConstants.NarrowLayoutMaxWidth)
            {
                // phone or narrow layout
                screenDimension = Math.Min(windowSize.Width, windowSize.Height); // min dimension

                tileWidth = RoundSize((screenDimension - 32) / 2.0);
                tileHeight = RoundSize(tileWidth * DesiredAspectRatio);
            }
            else if (windowSize.Width <= 1365.0 && windowSize.Height <= 1365.0)
            {
                // medium screens, max dimension <= 1365
                screenDimension = Math.Max(windowSize.Width, windowSize.Height); // max dimension
                tileWidth = RoundSize((screenDimension - 72) / 4.0);
                tileHeight = RoundSize(tileWidth * DesiredAspectRatio);
            }
            else
            {
                // large screens, max dimensions > 1365
                screenDimension = Math.Max(windowSize.Width, windowSize.Height); // max dimension
                tileWidth = RoundSize((screenDimension - 80) / 5.0);
                tileHeight = RoundSize(tileWidth * DesiredAspectRatio);
            }

            return new Size(tileWidth, tileHeight);
        }


        private static double RoundSize(double value)
        {
            return (4 * Math.Floor(value / 4.0));
        }
    }
}
