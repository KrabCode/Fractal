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
        public PointF ParentOrigin { get; set; }

        public Branch(PointF origin, PointF end, Pen pen, PointF parentOrigin)
        {
            Origin = origin;
            End = end;
            Pen = pen;
            ParentOrigin = parentOrigin;
            Children = new List<Branch>();
        }

        public void Populate(int childCount, double childDeviation, double piOffset)
        {
            double minAngle = AngleMath.GetAngleInDegrees(Origin, End) - childDeviation;
            double childAngleStep = childDeviation * 2 / childCount;

            while (Children.Count <= childCount)
            {
                double childEndAngle = minAngle + (childAngleStep * Children.Count);
                PointF childEndPoint = AngleMath.GetPointOnEdgeOfCircle(End.X,
                    End.Y,
                    AngleMath.GetDistance(Origin, End),
                    childEndAngle,
                    piOffset
                    );
                
                Branch child = new Branch(End,
                    childEndPoint,
                    Pen, 
                    this.Origin);

                Children.Add(child);
            }
        }
    }
}
