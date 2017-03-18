using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractal
{
    public class Parameter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool _IsFirstChange = true;
        private double _value;
        public double Value
        {
            get
            {                
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if (!_IsFirstChange)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                    }
                    else
                    {
                        _IsFirstChange = false;
                    }
                }                
            }
        }

        public string Name { get; set; }
        public double MinimumValue { get; set; }
        public double MaximumValue { get; set; }
        public bool Animated { get; set; }
        public int TooltipPrecision { get; set; }
        public double AnimatedFrom { get; set; }
        public double AnimatedTo { get; set; }
        public double AnimationChangePerFrame { get; set; }
        public bool AnimatingForwards { get; set; }        
    }
}
