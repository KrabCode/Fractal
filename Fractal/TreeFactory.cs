using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Fractal
{
    
    public class TreeFactory
    {
        public delegate EventHandler RedrawEvent(object sender, RedrawEventArgs e);
        public event RedrawEvent RedrawImage;
        private bool busy;
        /// <summary>
        /// Draws a psychedelic tree and updates the MainWindow by invoking RedrawImage and passing the resulting bitmap in RedrawEventArgs e.
        /// </summary>
        /// <param name="imageWidth">Bitmap X resolution</param>
        /// <param name="imageHeight">Bitmap Y resolution</param>
        /// <param name="childDeviation">angle difference between first and last child?</param>
        /// <param name="maxGenerations">Desired number of branch generations</param>
        /// <param name="childCount">Desired number of children per generation</param>
        /// <param name="penForeground">Pen to draw the tree with</param>
        /// <param name="brushBackground">Brush to draw the background with</param>
        /// <param name="zoomLevel">Length of the root line - inherited by every branch</param>
        /// <param name="piOffset">AngleMath uses π + piOffset in place of π for finding the endpoint of a branch</param>
        /// <param name="rootCount">Number of stems - effectively multiplying the number of branches</param>
        public void CreateNewTree(Size imageDimensions,
            Point centerOffset,
            int maxGenerations,
            int rootCount,
            int childCount,
            int zoomLevel,            
            double piOffset,
            double childDeviation,
            double childLengthRelativeToParent,
            double childHueChange,
            Pen penForeground,
            Brush brushBackground,
            LineStyle lineStyle            
            )
        {
            if (!busy)
            {
                busy = true;
                // Queen - Don't Stop Me Now
                // https://www.youtube.com/watch?v=HgzGwKwLmgM

                PointF center = new PointF(imageDimensions.Width / 2 + centerOffset.X, imageDimensions.Height / 2 + centerOffset.Y);
                List<Branch> tree = BuildBranches(center, maxGenerations, childCount, childDeviation, piOffset, childLengthRelativeToParent, rootCount, zoomLevel, penForeground, childHueChange);
                Bitmap _offscreen = new Bitmap(imageDimensions.Width, imageDimensions.Height);
                using (Graphics g = Graphics.FromImage(_offscreen))
                {
                    g.FillRectangle(brushBackground, 0, 0, imageDimensions.Width, imageDimensions.Height); //Fill background with background color                    
                    DrawBranches(g, tree, center, lineStyle );          //Draw the tree that we built
                    g.Flush();
                }
                busy = false;
                RedrawImage(this, new RedrawEventArgs(_offscreen));     //Render tree on the GUI thread
            }            
        }

        List<Branch> BuildBranches(PointF center,
            int maxGenerations,
            int childCount,
            double childDeviation,
            double piOffset,
            double relativeChildLength,
            int rootCount, 
            double zoomLevel, 
            Pen rootPen, 
            double childHueChange)
        {
            //Initialize tree
            List<Branch> tree = new List<Branch>();

            //Build the roots
            int angleStep = 360 / rootCount;
            for (int i = 0; i < rootCount; i++)
            {
                Branch root = new Branch(center,
                    AngleMath.GetPointOnEdgeOfCircle(center.X,
                    center.Y, 
                    zoomLevel,
                    (double)i * angleStep - 90,
                    piOffset),
                    rootPen,
                    center);
                tree.Add(root);
            }
            //Build the tree
            int generation = 0;
            while (generation < maxGenerations)
            {
                //remember the branch count now, because you'll be adding new elements during the cycle 
                //their new index will however be higher than the current population
                int branchCount = tree.Count;

                for (int i = 0; i < branchCount; i++) //do not exceed the original population in this generation
                {
                    //if this branch has no kids yet (happens only once per branch)
                    if (tree[i].Children.Count == 0)
                    {
                        Pen parentPen = tree[i].Pen;
                        //ajdust the pen for the children if needed
                        int a = parentPen.Color.A;
                        float h = parentPen.Color.GetHue() + (float)childHueChange;
                        float s = parentPen.Color.GetSaturation();
                        float b = parentPen.Color.GetBrightness();
                        Color childColor = ColorConverter.FromAhsb(a, h, s, b);
                        Pen childPen = new Pen(childColor, parentPen.Width );
                        
                        //make some kids
                        tree[i].Populate(childCount - 1, //no idea why I have to use a magic number here, TODO: Investigate
                            childDeviation,
                            piOffset,
                            relativeChildLength,
                            childPen);
                        foreach (Branch babyBranch in tree[i].Children)
                        {
                            //add them to the main list
                            tree.Add(babyBranch);
                        }
                    }
                }
                generation++;
            }
            return tree;
        }

        void DrawBranches(Graphics g, List<Branch> branches, PointF center, LineStyle lineStyle)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            
            //Draw the tree according to selected line style - probably could be done more efficiently
            switch (lineStyle)
            {
                case LineStyle.Normal:
                    {
                        foreach (Branch b in branches)
                        {
                            g.DrawLine(b.Pen, b.Origin, b.End);
                        }
                        break;
                    }
                case LineStyle.Polygon:
                    {
                        foreach (Branch b in branches)
                        {
                            g.DrawPolygon(b.Pen,
                                new PointF[] { b.ParentOrigin, b.Origin, b.End
                                });
                        }
                        break;
                    }
                case LineStyle.Bezier:
                    {
                        foreach (Branch b in branches)
                        {
                            g.DrawBezier(b.Pen,
                                center,
                                b.ParentOrigin,
                                b.Origin,
                                b.End
                                );
                        }
                        break;
                    }
                case LineStyle.Leaf:
                    {
                        foreach (Branch b in branches)
                        {
                            g.DrawCurve(b.Pen,
                                new PointF[] { b.End,  b.Origin, b.ParentOrigin
                                });
                        }
                        break;
                    }
                case LineStyle.ClosedCurve:
                    {
                        foreach (Branch b in branches)
                        {
                            g.DrawClosedCurve(b.Pen,
                                new PointF[] { b.ParentOrigin, b.Origin, b.End
                                });
                        }
                        break;
                    }
                case LineStyle.FilledPolygon:
                    {
                        foreach (Branch b in branches)
                        {
                            g.FillPolygon(b.Pen.Brush,
                                new PointF[] { b.ParentOrigin, b.Origin, b.End
                                });
                        }
                        break;
                    }
                case LineStyle.FilledClosedCurve:
                    {
                        foreach (Branch b in branches)
                        {
                            g.FillClosedCurve(b.Pen.Brush,
                                new PointF[] { b.ParentOrigin, b.Origin, b.End
                                });
                        }
                        break;
                    }
                case LineStyle.Eighth:
                    {

                        break;
                    }
                case LineStyle.Ninth:
                    {
                        break;
                    }
                case LineStyle.Tenth:
                    {
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

    }
}
