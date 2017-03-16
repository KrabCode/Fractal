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
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

public enum KnownImageFormat { bmp, png, jpeg, gif };
public enum LineStyle { Normal, Bezier, Leaf, ClosedCurve, Polygon, FilledPolygon, FilledClosedCurve, Eighth, Ninth, Tenth };

namespace Fractal
{
   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Global variables

        private Logic _logic;
        private Bitmap displayedBitmap;
        private Random _random = new Random();
       

        private double _childDeviation = 0;
        private double _piOffset = 0;
        private int _generations = 6;
        private int _childCount = 4;
        private int _penOpacity = 50;
        private int _penWidth = 1;
        private int _zoomLevel = 20;
        private int _resolutionX = 1920;
        private int _resolutionY = 1080;
        private double _childHueChange = 0;
        private Pen _penForeground = new Pen(new SolidBrush(Color.FromArgb(50, Color.Black)),1);
        private Brush _brushBackground = new SolidBrush(Color.White);
        private int _rootCount = 1;
        private bool _animate = false;
        private bool _animatingForwards = false;
        private bool _animateAndSave = false;
        private int _autosavedPicsAlready = 0;
        private string _autosaveDirectory = "";
        private double _deviationChangeBetweenFrames = 1;
        private LineStyle _lineStyle = LineStyle.Normal;
        private double _childLengthRelativeToParent = 1;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            _logic = new Logic();
            _logic.RedrawImage += _logic_Redraw;

            TryDrawTree();
        }

        /// <summary>
        /// Fires when _logic finishes DrawTree(), the resulting image is passed using RedrawEventArgs.
        /// </summary>
        /// <param name="sender">Only a Logic instance sends this</param>
        /// <param name="e">e.image should contain the resulting image to draw onto the main drawing surface</param>
        /// <returns></returns>
        private EventHandler _logic_Redraw(object sender, RedrawEventArgs e)
        {
                 
            displayedBitmap = new Bitmap(e.imageToDraw);

            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    
                    if (_animateAndSave)
                    {
                        string filepath = _autosaveDirectory + "img_" + _autosavedPicsAlready++ + ".png";
                        SaveImageToFile(filepath, (Bitmap)e.imageToDraw.Clone(), KnownImageFormat.png);      
                        
                        if(sliderDeviation.Value < sliderDeviation.Maximum)
                        {
                            sliderDeviation.Value += _deviationChangeBetweenFrames;
                            
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("All deviations were saved to " + _autosaveDirectory);
                            _animateAndSave = false;
                            checkboxAutosave.IsChecked = false;
                        }
                    }
                    else if (_animate)
                    {
                        if(_animatingForwards)
                        {
                            if (sliderDeviation.Value < sliderDeviation.Maximum)
                            {
                                sliderDeviation.Value += _deviationChangeBetweenFrames;
                            }
                            else
                            {
                                _animatingForwards = false;
                                TryDrawTree();
                            }
                        }
                        else
                        {
                            if (sliderDeviation.Value != 0 )
                            {
                                sliderDeviation.Value -= _deviationChangeBetweenFrames;
                            }
                            else
                            {
                                _animatingForwards = true;
                                TryDrawTree();
                            }
                        }
                    }
                    imageMainView.Source = BitmapConverter.Bitmap2BitmapSource(e.imageToDraw);
                }));
            
            return null;
        }

        private void TryDrawTree()
        {
            Task t = Task.Run(delegate {
                _logic.DrawTree(_resolutionX,
                    _resolutionY,
                    _generations,
                    _rootCount,
                    _childCount,
                    _zoomLevel,
                    _piOffset,
                    _childDeviation,
                    _childLengthRelativeToParent,
                    _childHueChange,
                    _penForeground,
                    _brushBackground,
                    _lineStyle);
            });
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
                Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
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
            _childDeviation = Math.Round(sliderDeviation.Value,0);
            TryDrawTree();
        }

        private void sliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _zoomLevel = (int)sliderSize.Value;
            TryDrawTree();
        }

        private void sliderDetail_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _generations = (int)sliderDetail.Value;
            TryDrawTree();
        }
        
        private void sliderRootCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _rootCount = (int)sliderRootCount.Value;
            TryDrawTree();
        }

        private void sliderChildCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _childCount = (int)sliderChildCount.Value;
            TryDrawTree();
        }
        
        private void sliderPiOffset_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _piOffset = sliderPiOffset.Value;
            TryDrawTree();
        }

        #endregion

        #region Color selection wiring

        private void btForeground_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //recolor the button
                Color _oppositeColor = GetOppositeColor(cd.Color);
                System.Windows.Media.Color _mediaColor = System.Windows.Media.Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B);
                System.Windows.Media.Color _mediaColorOpposite = System.Windows.Media.Color.FromArgb(_oppositeColor.A, _oppositeColor.R, _oppositeColor.G, _oppositeColor.B);
                btForeground.Background = new System.Windows.Media.SolidColorBrush(_mediaColor);
                btForeground.Foreground = new System.Windows.Media.SolidColorBrush(_mediaColorOpposite);

                //adjust global settings as per the user's wishes and redraw main surface with new settings
                _penForeground = new Pen(Color.FromArgb(_penOpacity, cd.Color), _penWidth);
                TryDrawTree();
            }

        }

        private void btBackground_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //recolor the button
                Color _oppositeColor = GetOppositeColor(cd.Color);
                System.Windows.Media.Color _mediaColor = System.Windows.Media.Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B);
                System.Windows.Media.Color _mediaColorOpposite = System.Windows.Media.Color.FromArgb(_oppositeColor.A, _oppositeColor.R, _oppositeColor.G, _oppositeColor.B);
                btBackground.Background = new System.Windows.Media.SolidColorBrush(_mediaColor);
                btBackground.Foreground = new System.Windows.Media.SolidColorBrush(_mediaColorOpposite);

                //adjust global settings as per the user's wishes and redraw main surface with new settings
                _brushBackground = new SolidBrush(cd.Color);
                TryDrawTree();
            }
        }

        #endregion

        #region Animation and autosave checkbox wiring
        private void checkboxAutosave_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select or create a folder to save all the deviations of the selected settings to:";
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _autosaveDirectory = fbd.SelectedPath + "\\";
                    sliderDeviation.Value = 0;
                    _animateAndSave = (bool)checkboxAutosave.IsChecked;
                    
                    TryDrawTree();
                }
                else
                {
                    checkboxAutosave.IsChecked = false;
                }
            }
        }

        private void checkboxAnimate_Click(object sender, RoutedEventArgs e)
        {
            _animate = (bool)checkboxAnimate.IsChecked;
            checkboxAutosave.IsChecked = false;
            TryDrawTree();
        }

        private void sliderPenWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _penWidth = (int)sliderPenWidth.Value;
            _penForeground = new Pen(Color.FromArgb(_penOpacity, _penForeground.Color), _penWidth);
            TryDrawTree();
        }

        private void sliderPenOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _penOpacity = (int)sliderPenOpacity.Value;
            _penForeground = new Pen(Color.FromArgb( _penOpacity,_penForeground.Color), _penWidth);
            TryDrawTree();
        }

        private Color GetOppositeColor(Color original)
        {
            int r = 255 - original.R;
            int g = 255 - original.G;
            int b = 255 - original.B;

            return Color.FromArgb(r, g, b);
        }
        #endregion

        #region Resolution wiring
        private void tbResolutionX_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbResolutionX.Text != "")
            {
                try
                {
                    int resolutionX = Convert.ToInt32(tbResolutionX.Text);
                    if(resolutionX > 50)
                    {
                        _resolutionX = resolutionX;
                        TryDrawTree();
                    }
                   
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        private void tbResolutionY_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbResolutionY.Text != "")
            {
                try
                {
                    int resolutionY = Convert.ToInt32(tbResolutionY.Text);
                    if(resolutionY > 50)
                    {
                        _resolutionY = resolutionY;
                        TryDrawTree();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }


        #endregion

        private void comboLineStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int itemIndex = comboLineStyle.SelectedIndex;
            switch (itemIndex)
            {
                case 0:
                    {
                        _lineStyle = LineStyle.Normal;
                        break;
                    }
                case 1:
                    {
                        _lineStyle = LineStyle.Polygon;                        
                        break;
                    }
                case 2:
                    {
                        _lineStyle = LineStyle.Bezier;                        
                        break;
                    }
                case 3:
                    {
                        _lineStyle = LineStyle.Leaf;                        
                        break;
                    }
                case 4:
                    {
                        _lineStyle = LineStyle.ClosedCurve;
                        break;
                    }
                case 5:
                    {
                        _lineStyle = LineStyle.FilledPolygon;
                        break;
                    }
                case 6:
                    {
                        _lineStyle = LineStyle.FilledClosedCurve;
                        break;
                    }
                case 7:
                    {
                        _lineStyle = LineStyle.Eighth;
                        break;
                    }
                case 8:
                    {
                        _lineStyle = LineStyle.Ninth;
                        break;
                    }
                case 9:
                    {
                        _lineStyle = LineStyle.Tenth;
                        break;
                    }
                default:
                    {
                        _lineStyle = LineStyle.Normal;
                        break;
                    }                
            }
            TryDrawTree();
        }

        private void sliderRelativeChildLength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _childLengthRelativeToParent = sliderRelativeChildLength.Value;
            TryDrawTree();
        }

        private void sliderHueChange_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _childHueChange = (int)sliderHueChange.Value;
            TryDrawTree();
        }
    }
}
