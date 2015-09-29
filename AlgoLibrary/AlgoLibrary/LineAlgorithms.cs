using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoLibrary
{

    public class Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X;
        public double Y;
    }

    public class Line
    {
        public Line(double x0, double y0, double x1, double y1)
        {
            if (x0 < x1)
            {
                P0 = new Point(x0, y0);
                P1 = new Point(x1, y1);
            }
            else
            {
                P1 = new Point(x0, y0);
                P0 = new Point(x1, y1);
            }
        }
        public Point P0 { get; set; }
        public Point P1 { get; set; }

        public double YIntersect
        {
            get
            {
                if (P0.X != P1.X)
                    return (P1.Y* P0.X - P0.Y * P1.X) / (P1.X - P0.X);
                else
                    return Double.MaxValue;
            }
        }

        public double Slope
        { 
            get
            {
                if (P0.X != P1.X)
                    return (P1.Y - P0.Y) / (P1.X - P0.X);
                else
                    return Double.MaxValue; 
            }
        }

        public Point Intersect(Line secondLine)
        {
            if(this.Slope == secondLine.Slope)
            {
                if (this.YIntersect != secondLine.YIntersect)
                    return null;
                else
                    return new Point(0, this.YIntersect);
            }

            else
            {
                return new Point(
                    (this.YIntersect - secondLine.YIntersect) / (secondLine.Slope - this.Slope),
                    (secondLine.Slope * this.YIntersect - this.Slope * secondLine.YIntersect) 
                    / (secondLine.Slope - this.Slope)
                    );
            }
        }
    }
    

    public class LineAlgorithms
    {
        public LineAlgorithms() { }
        public int BaseMethod()
        {
            return 1;
        }
    }
}
