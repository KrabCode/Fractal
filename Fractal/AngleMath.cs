using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public static class AngleMath
    {
        public static PointF GetPointOnEdgeOfCircle(double x0, double y0, double radius, double angleInDegrees, double piOffset)
        {
            double x = x0 + radius * Math.Cos(angleInDegrees * (Math.PI + piOffset )/ 180);
            double y = y0 + radius * Math.Sin(angleInDegrees * (Math.PI + piOffset )/ 180);
            return new PointF((float)x, (float)y);
        }

        public static double GetAngleInDegrees(PointF origin, PointF end)
        {
            PointF delta = new PointF(end.X - origin.X, end.Y - origin.Y);
            return RadianToDegree(Math.Atan2(delta.Y, delta.X));
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static double GetDistance(PointF a, PointF b)
        {
            //Right angled triangle ABC, return the hypotenuse
            PointF c = new PointF(a.X, b.Y);
            double CB = b.X - c.X;
            double AC = a.Y - c.Y;
            return Math.Sqrt((AC * AC) + (CB * CB));
        }


    } 
}
