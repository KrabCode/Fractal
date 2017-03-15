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

        public void Populate(int targetPopulation, double childDeviation, double piOffset, double relativeChildLength)
        {
            //There's n children, growing from the End of this branch, each deviating from the parent in an angle between -childDeviation and childDeviation
            double angleThisBranch = AngleMath.GetAngleInDegrees(Origin, End);      //What's the absolute orientation of this branch?            
            double angleOfFirstChild = angleThisBranch - childDeviation;                //What's the absolute orientation of the first child at -childDeviation?
            double angleNeighbourDifference = childDeviation * 2 / targetPopulation;        //What's the angle difference between the first child and the next?

            while (Children.Count <= targetPopulation) //Until we hit the target population
            {
                //Find absolute angle for this child
                double childAngle = angleOfFirstChild + (angleNeighbourDifference * Children.Count);
                //Find the End of the child using the newfound childAngle
                PointF childEnd = AngleMath.GetPointOnEdgeOfCircle(End.X,
                    End.Y,
                    AngleMath.GetDistance(Origin, End) * relativeChildLength,
                    childAngle,
                    piOffset
                    );
                //Instantiate the child
                Branch child = new Branch(End, //The origin of the child is the End of this branch
                    childEnd, //The End of the child
                    Pen, 
                    this.Origin);

                //Add it to the children collection so that this method may end someday
                Children.Add(child);
            }
        }
    }
}
