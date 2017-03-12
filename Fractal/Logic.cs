using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public class Logic
    {
        public delegate EventHandler RedrawEvent(object sender, RedrawEventArgs e);
        public event RedrawEvent RedrawImage;
        private Bitmap _offscreen;
        private bool busy;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageWidth">Bitmap X resolution</param>
        /// <param name="imageHeight">Bitmap Y resolution</param>
        /// <param name="childDeviation">angle difference between first and last child?</param>
        /// <param name="maxGenerations">Desired number of branch generations</param>
        /// <param name="childCount">Desired number of children per generation</param>
        /// <param name="penForeground">Pen to draw the tree with</param>
        /// <param name="brushBackground">Brush to draw the background with</param>
        /// <param name="zoomLevel"></param>
        /// <param name="piOffset"></param>
        /// <param name="rootCount"></param>
        public void DrawTree(int imageWidth, int imageHeight, double childDeviation, int maxGenerations, int childCount, Pen penForeground, Brush brushBackground, int zoomLevel, double piOffset, int rootCount)
        {
            if (!busy)
            {
                busy = true;
                // Queen - Don't Stop Me Now
                // https://www.youtube.com/watch?v=HgzGwKwLmgM
                               
                //Initialize image, graphics to draw with, draw background                                
                _offscreen = new Bitmap(imageWidth, imageHeight);
                Graphics g = Graphics.FromImage(_offscreen);
                g.FillRectangle(brushBackground, 0, 0, imageWidth, imageHeight);

                //Initialize main list
                List<Branch> branches = new List<Branch>();                
                
                //Build the roots
                int angleStep = 360 / rootCount;
                Point center = new Point(imageWidth / 2, imageHeight / 2);
                for (int i = 0; i < rootCount; i++ )
                {
                    Branch root = new Branch(center, AngleMath.GetPointOnEdgeOfCircle(center.X, center.Y, zoomLevel, (double)i*angleStep - 90, piOffset), penForeground);                    
                    branches.Add(root);                    
                }

                //Build the tree
                int generation = 0;                
                while (generation < maxGenerations)
                {
                    //remember the branch count now, because you'll be adding new elements during the cycle 
                    //their new index will however be higher than this number
                    int branchCount = branches.Count;

                    for (int i = 0; i < branchCount; i++)
                    {
                        //if tree branch has no kids yet
                        if (branches[i].Children.Count == 0)
                        {
                            //make some kids
                            branches[i].Populate(childCount, childDeviation, piOffset);
                            foreach (Branch babyBranch in branches[i].Children)
                            {
                                branches.Add(babyBranch);
                            }
                        }
                    }
                    generation++;
                }

                //Draw the tree - probably could be done more efficiently
                foreach (Branch b in branches)
                {
                    g.DrawLine(b.Pen, b.Origin, b.End);
                }
                
                g.Dispose();
                busy = false;               
                RedrawImage(this, new RedrawEventArgs(_offscreen));
                
            }            
        }
    }
}
