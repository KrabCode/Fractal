using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Fractal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logic _logic;
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.WindowState = WindowState.Maximized;

            _logic = new Logic();
            _logic.RedrawImage += _logic_Redraw;

            TryRedraw();
        }

        private EventHandler _logic_Redraw(object sender, RedrawEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                imageMainView.Source = BitmapConverter.Bitmap2BitmapSource(e.imageToDraw);
            }));            
            return null;
        }


        double deviation = 0;
        int detail = 5;
        int childCount = 2;
        int penOpacity = 50;

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            deviation = sliderDeviation.Value;
            TryRedraw();
        }

        private void sliderDetail_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            detail = (int)sliderDetail.Value;
            TryRedraw();
        }

        private void sliderChildCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            childCount = (int)sliderChildCount.Value;
            TryRedraw();
        }

        private void sliderPenOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            penOpacity = (int)sliderPenOpacity.Value;
            TryRedraw();
        }

        private void TryRedraw()
        {
            Task t = Task.Run(delegate { _logic.Start(1920, 1080, deviation, detail, childCount, penOpacity); });
        }
    }
}
