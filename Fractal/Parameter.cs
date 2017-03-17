using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public class Parameter
    {
        
        public string Name { get; set; }
        public double Value { get; set; }
        public double MinimumValue { get; set; }
        public double MaximumValue { get; set; }

        public bool Animated { get; set; }
        public double AnimatedFrom { get; set; }
        public double AnimatedTo { get; set; }
        public double AnimationChangePerFrame { get; set; }
        public bool AnimatingForwards { get; set; }
    }
}
