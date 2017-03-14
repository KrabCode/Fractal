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
    
    public class Logic
    {
        public delegate EventHandler RedrawEvent(object sender, RedrawEventArgs e);
        public event RedrawEvent RedrawImage;
        private Bitmap _offscreen;
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
        public void DrawTree(int imageWidth,
            int imageHeight,
            double childDeviation,
            int maxGenerations,
            int childCount,
            Pen penForeground,
            Brush brushBackground,
            int zoomLevel,
            double piOffset,
            int rootCount,
            LineStyle lineStyle,
            System.Drawing.Drawing2D.CompositingQuality graphicsQuality)
        {
            if (!busy)
            {
                busy = true;
                // Queen - Don't Stop Me Now
                // https://www.youtube.com/watch?v=HgzGwKwLmgM
                               
                //Initialize image, graphics to draw with, draw background      
                
                _offscreen = new Bitmap(imageWidth, imageHeight);
                using (Graphics g = Graphics.FromImage(_offscreen))
                {
                    
                    g.CompositingQuality = graphicsQuality;
                    g.FillRectangle(brushBackground, 0, 0, imageWidth, imageHeight);

                    //Initialize main list
                    List<Branch> branches = new List<Branch>();

                    //Build the roots
                    int angleStep = 360 / rootCount;
                    PointF center = new PointF(imageWidth / 2, imageHeight / 2);
                    for (int i = 0; i < rootCount; i++)
                    {
                        Branch root = new Branch(center,
                            AngleMath.GetPointOnEdgeOfCircle(center.X, center.Y, zoomLevel, (double)i * angleStep - 90, piOffset),
                            penForeground, center);
                        branches.Add(root);
                    }

                    //Build the tree
                    int generation = 0;
                    while (generation < maxGenerations)
                    {
                        //remember the branch count now, because you'll be adding new elements during the cycle 
                        //their new index will however be higher than the current population
                        int branchCount = branches.Count;

                        for (int i = 0; i < branchCount; i++) //do not exceed the original population in this generation
                        {
                            //if this branch has no kids yet (happens only once per branch)
                            if (branches[i].Children.Count == 0)
                            {
                                //make some kids
                                branches[i].Populate(childCount, childDeviation, piOffset);
                                foreach (Branch babyBranch in branches[i].Children)
                                {
                                    //add them to the main list
                                    branches.Add(babyBranch);
                                }
                            }
                        }
                        generation++;
                    }

                    //Draw the tree - probably could be done more efficiently
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
                                        b.End);
                                }
                                break;
                            }
                        case LineStyle.Leaf:
                            {
                                foreach (Branch b in branches)
                                {
                                    g.DrawCurve(b.Pen,
                                        new PointF[] { b.ParentOrigin, b.Origin, b.End
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

                    g.Flush();
                }

                busy = false;
                RedrawImage(this, new RedrawEventArgs(_offscreen));
                
            }
        }
    }
}
