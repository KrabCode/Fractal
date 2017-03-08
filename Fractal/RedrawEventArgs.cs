using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public class RedrawEventArgs : EventArgs
    {
        public Bitmap imageToDraw { get; set; }
        public RedrawEventArgs(Bitmap toDraw)
        {
            imageToDraw = toDraw;
        }

    }
}
