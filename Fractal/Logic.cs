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

        
        public void Start(int width, int height, double childDeviation, int detail, int childCount, int penOpacity, int size)
        {
            if(!busy)
            {

                int levelOfDetail = 0;
                int maxLevelOfDetail = detail;

                busy = true;
                _offscreen = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(_offscreen);
                List<Branch> branchesToPopulate = new List<Branch>();


                Pen blackPen = new Pen(new SolidBrush(Color.FromArgb(penOpacity,Color.Black)));


                Branch root = new Branch(new Point(width / 2, height / 2 + size), new Point(width / 2, height / 2 - size), blackPen, childDeviation, childCount);
                root.Populate();
                
                g.DrawLine(blackPen, root.Origin, root.End);
                foreach (Branch child in root.Children)
                {
                    branchesToPopulate.Add(child);
                }
                while (levelOfDetail < maxLevelOfDetail)
                {
                    int branchesToPopulateCount = branchesToPopulate.Count;
                    for (int i = 0; i < branchesToPopulateCount; i++)
                    {
                        if (branchesToPopulate[i].Children.Count == 0)
                        {
                            branchesToPopulate[i].Populate();
                            foreach (Branch babyBranch in branchesToPopulate[i].Children)
                            {
                                branchesToPopulate.Add(babyBranch);
                            }
                        }
                    }

                    levelOfDetail++;
                }

                foreach (Branch b in branchesToPopulate)
                {
                    g.DrawLine(blackPen, b.Origin, b.End);
                }

                RedrawImage(this, new RedrawEventArgs(_offscreen));
                levelOfDetail = 0;
                busy = false;
            }            
        }
    }
}
