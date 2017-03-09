using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
public enum KnownImageFormat { bmp, png, jpeg, gif };

namespace Fractal
{
   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logic _logic;
        private Bitmap displayedBitmap;
        private Random _random = new Random();
        private double _deviation = 0;
        private double _piOffset = 0;
        private int _detail = 6;
        private int _childCount = 4;
        private int _penOpacity = 50;
        private int _size = 20;
        private int _resolutionX = 1920;
        private int _resolutionY = 1080;


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
            displayedBitmap = new Bitmap(e.imageToDraw);
            Application.Current.Dispatcher.Invoke(new Action(() => {
                imageMainView.Source = BitmapConverter.Bitmap2BitmapSource(e.imageToDraw);
            }));
           
            return null;
        }

        private void TryRedraw()
        {
            Task t = Task.Run(delegate { _logic.Start(_resolutionX, _resolutionY, _deviation, _detail, _childCount, _penOpacity, _size, _piOffset); });
        }

        #region Save button wiring
        public void SaveImageToFile(string filePath, Bitmap image, KnownImageFormat format)
        {

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                ImageFormat finalFormat = ImageFormat.Png;
                switch (format)
                {
                    case KnownImageFormat.png:
                        {
                            finalFormat = ImageFormat.Png;
                            break;
                        }
                    case KnownImageFormat.bmp:
                        {
                            finalFormat = ImageFormat.Bmp;
                            break;
                        }
                    case KnownImageFormat.gif:
                        {
                            finalFormat = ImageFormat.Gif;
                            break;
                        }
                    case KnownImageFormat.jpeg:
                        {
                            finalFormat = ImageFormat.Jpeg;
                            break;
                        }
                    default:
                        {
                            finalFormat = ImageFormat.Png;
                            break;
                        }
                }
                image.Save(fileStream, finalFormat);
            }
        }

        private string GetVacantSuffix()
        {
            string name = "";

            for (int i = 0; i < 8; i++)
            {
                name += GetLetter();
            }

            return name;
        }
        
        public char GetLetter()
        {
            // This method returns a random lowercase letter.
            // ... Between 'a' and 'z'
            int num = _random.Next(0, 26); // Zero to 25
            char let = (char)('a' + num);
            return let;
        }
                
        private void btSave_Click_1(object sender, RoutedEventArgs e)
        {
            if (displayedBitmap != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save image";
                sfd.Filter = ".png|*.png|.bmp|*.bmp|.jpg|*.jpg|.gif(stationary)|*.gif";
                //find the best possible name for the new file,
                //so that the user doesn't have to type anything or overwrite when lazy
                string initialDirectory = sfd.InitialDirectory;
                sfd.FileName += "Image_" + GetVacantSuffix();
                KnownImageFormat format;
                if ((bool)sfd.ShowDialog())
                {
                    string ext = System.IO.Path.GetExtension(sfd.FileName);
                    switch (ext)
                    {
                        case ".jpg":
                            format = KnownImageFormat.jpeg;
                            break;
                        case ".bmp":
                            format = KnownImageFormat.bmp;
                            break;
                        case ".gif":
                            format = KnownImageFormat.gif;
                            break;
                        default:
                            format = KnownImageFormat.png;
                            break;

                    }

                    SaveImageToFile(sfd.FileName,
                        displayedBitmap,
                        format);
                }
            }
        }
        #endregion

        #region Slider wiring
        private void sliderDeviation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _deviation = Math.Round(sliderDeviation.Value,0);
            TryRedraw();
        }

        private void sliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _size = (int)sliderSize.Value;
            TryRedraw();
        }

        private void sliderDetail_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _detail = (int)sliderDetail.Value;
            TryRedraw();
        }

        private void sliderPenOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _penOpacity = (int)sliderPenOpacity.Value;
            TryRedraw();
        }

        private void sliderChildCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _childCount = (int)sliderChildCount.Value;
            TryRedraw();
        }
        

        private void sliderPiOffset_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _piOffset = sliderPiOffset.Value;
            TryRedraw();
        }
    }
    #endregion
}
