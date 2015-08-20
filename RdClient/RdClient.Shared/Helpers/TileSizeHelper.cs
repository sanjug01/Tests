using RdClient.Shared.CxWrappers;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Helpers
{
    public class TileSizeHelper
    {
        private const double MediumViewSizeLimit = 1366d;

        // constants to calculate tile size
        private const double DesiredAspectRatio = 9d / 16d;
        private const double MinViewSizeOffset = 32d;
        private const double MinViewSizeScaleFactor = 2d;
        private const double MediumViewSizeOffset = 72d;
        private const double MediumViewSizeScaleFactor = 4d;
        private const double LargeViewSizeOffset = 80d;
        private const double LargeViewSizeScaleFactor = 5f;
        private const double RoundingBase = 4d;
        private const double MinWidthLimit = 144d;

        /// <summary>
        /// apply proprietary algorithm to calculate the tile size based on the actual window size
        /// generic formula is :
        ///    tileWidth = (mainViewDimension - sizeOffset) / ScaleFactor 
        ///    tileHeight = tileWidth * AspectRatio
        /// the final values are rounded down
        /// </summary>
        /// <param name="windowSize">actual window size</param>
        /// <returns>tile size</returns>
        public static Size GetTileSize(Size windowSize)
        {
            double mainDimension = 0d;

            double width, height;
            if (windowSize.Width < GlobalConstants.NarrowLayoutWidthLimit)
            {
                // phone or narrow layout, use min dimension
                mainDimension = Math.Min(windowSize.Width, windowSize.Height); 
                width = Math.Max(RoundSize((mainDimension - MinViewSizeOffset) / MinViewSizeScaleFactor), MinWidthLimit);
                height = RoundSize(width * DesiredAspectRatio);
            }
            else if (windowSize.Width < MediumViewSizeLimit && windowSize.Height < MediumViewSizeLimit)
            {
                // medium screens, both dimensions < 1366
                //        use max dimension
                mainDimension = Math.Max(windowSize.Width, windowSize.Height);
                width = Math.Max(RoundSize((mainDimension - MediumViewSizeOffset) / MediumViewSizeScaleFactor), MinWidthLimit);
                height = RoundSize(width * DesiredAspectRatio);
            }
            else
            {
                // large screens, at least one dimension > 1365
                //       use max dimension
                mainDimension = Math.Max(windowSize.Width, windowSize.Height);
                width = Math.Max(RoundSize((mainDimension - LargeViewSizeOffset) / LargeViewSizeScaleFactor), MinWidthLimit);
                height = RoundSize(width * DesiredAspectRatio);
            }

            return new Size(width, height);
        }

        /// <summary>
        /// apply rounding down to the nearest multiple of 4
        /// This is commonly required for all final sizes
        /// </summary>
        /// <param name="value">calculated size</param>
        /// <returns>rounded value of the calculated size</returns>
        private static double RoundSize(double value)
        {
            return (RoundingBase * Math.Floor(value / RoundingBase));
        }
    }
}
