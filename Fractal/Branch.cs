using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    class Branch
    {
        public PointF Origin { get; set; }
        public PointF End { get; set; }
        public Pen Pen { get; set; }
        public List<Branch> Children { get; set; }
        public double Angle { get; set; }



        private double _piOffset { get; set; }
        private double _childDeviation { get; set; }
        private int _childCount { get; set; }
        private double _length { get; set; }

        public Branch(PointF origin, PointF end, Pen pen, double childDeviation, int childCount, double piOffset)
        {
            Origin = origin;
            End = end;
            Pen = pen;
            Angle = AngleMath.GetAngleInDegrees(Origin, End);
            Children = new List<Branch>();

            _piOffset = piOffset;
            _childDeviation = childDeviation;            
            _length = AngleMath.GetDistance(Origin, End);
            _childCount = childCount;
        }

        public void Populate()
        {
            double minAngle = Angle - _childDeviation;
            double childAngleStep = _childDeviation * 2 / _childCount;

            while (Children.Count <= _childCount)
            {
                double childEndAngle = minAngle + (childAngleStep * Children.Count);
                PointF childEndPoint = AngleMath.GetPointOnEdgeOfCircle(End.X, End.Y, _length, childEndAngle, _piOffset);
                
                Branch child = new Branch(End, childEndPoint, Pen, _childDeviation, _childCount, _piOffset);
                Children.Add(child);
            }
        }
    }
}
