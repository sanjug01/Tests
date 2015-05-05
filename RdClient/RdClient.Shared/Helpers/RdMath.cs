using System;
using Windows.Foundation;

namespace RdClient.Shared.Helpers
{
    public class RdMath
    {
        public static double Distance(Point p)
        {
            return Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }

        public static double Angle(Point p)
        {
            return Math.Asin(p.Y / Math.Sqrt(p.X * p.X + p.Y * p.Y)) * 180 / Math.PI;
        }        
    }
}
