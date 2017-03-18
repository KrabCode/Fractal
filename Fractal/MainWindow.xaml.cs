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
using System.Text.RegularExpressions;

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

        private TreeFactory _treeFactory;
        private Bitmap displayedBitmap;
        private Random _random = new Random();
       
        private int _resolutionX = 1920;
        private int _resolutionY = 1080;
        private Pen _penForeground = new Pen(new SolidBrush(Color.FromArgb(50, Color.Black)),1);
        private int _lastPenWidth = 2;
        private int _lastPenOpacity = 80;
        private Brush _brushBackground = new SolidBrush(Color.White);
        private bool _atLeastOneParameterIsAnimated = false;
        private LineStyle _lineStyle = LineStyle.Normal;

        public List<Parameter> Settings;
        public Dictionary<string, Parameter> SettingsMap;

        //for dragging the image
        private System.Windows.Point _mousePositionWhenUserStartedDraggingImage;
        private System.Windows.Point _offsetWhenUserStartedDraggingImage;
        private bool _isUserDraggingImage;

        bool fullyLoaded = false;
        #endregion

        #region Initialization

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            
            _treeFactory = new TreeFactory();            
            _treeFactory.RedrawImage += _logic_Redraw;

            Settings = new List<Parameter>();
            SettingsMap = new Dictionary<string, Parameter>();

            Settings.Add(new Parameter() { Name = "Deviation",      Value = 15, MinimumValue = 0,       MaximumValue = 360,     TooltipPrecision = 3    });            
            Settings.Add(new Parameter() { Name = "Generations",    Value = 3,  MinimumValue = 1,       MaximumValue = 15,      TooltipPrecision = 0    });
            Settings.Add(new Parameter() { Name = "Child count",    Value = 3,  MinimumValue = 2,       MaximumValue = 15,      TooltipPrecision = 0    });
            Settings.Add(new Parameter() { Name = "Child length",   Value = 1,  MinimumValue = 0,       MaximumValue = 2,       TooltipPrecision = 3    });
            Settings.Add(new Parameter() { Name = "Root count",     Value = 4,  MinimumValue = 1,       MaximumValue = 10,      TooltipPrecision = 0    });
            Settings.Add(new Parameter() { Name = "Pen width",      Value = 3,  MinimumValue = 1,       MaximumValue = 20,      TooltipPrecision = 0    });
            Settings.Add(new Parameter() { Name = "Pen opacity",    Value = 80, MinimumValue = 1,       MaximumValue = 254,     TooltipPrecision = 0    });
            Settings.Add(new Parameter() { Name = "Hue change",     Value = 0,  MinimumValue = 0,       MaximumValue = 360,     TooltipPrecision = 0    });
            Settings.Add(new Parameter() { Name = "Zoom level",     Value = 80, MinimumValue = 0,       MaximumValue = 3000,    TooltipPrecision = 1    });
            Settings.Add(new Parameter() { Name = "Pi offset",      Value = 0,  MinimumValue = -10,     MaximumValue = 10,      TooltipPrecision = 3    });
            Settings.Add(new Parameter() { Name = "X offset",       Value = 0,  MinimumValue = -1000,   MaximumValue = +1000,   TooltipPrecision = 0    });
            Settings.Add(new Parameter() { Name = "Y offset",       Value = 0,  MinimumValue = -1000,   MaximumValue = +1000,   TooltipPrecision = 0    });

            foreach (Parameter p in Settings)
            {
                p.Animated = false;
                p.AnimatedFrom = p.MinimumValue;
                p.AnimatedTo = p.MaximumValue;
                p.AnimationChangePerFrame = 1;
                p.AnimatingForwards = true;

                SettingsMap.Add(p.Name, p);
            }

            lvSettings.ItemsSource = Settings;

            fullyLoaded = true;
            TryDrawTree();
        }
        #endregion

        #region Redraw event and subsequent logic
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
                    imageMainView.Source = BitmapConverter.Bitmap2BitmapSource(e.imageToDraw);
                    TryDrawNextAnimationFrame();
                }));
            
            return null;
        }

        private void TryDrawNextAnimationFrame()
        {
            _atLeastOneParameterIsAnimated = false;
            foreach (Parameter p in Settings)
            {
                if(p.Animated)
                {
                    _atLeastOneParameterIsAnimated = true;
                    break;
                }
            }

            if (_atLeastOneParameterIsAnimated)
            {
                foreach (Parameter p in Settings)
                {
                    if(p.Animated)
                    {
                        if(p.AnimatingForwards)
                        {
                            p.Value += p.AnimationChangePerFrame;
                        }
                        else
                        {
                            p.Value -= p.AnimationChangePerFrame;
                        }

                        if(p.Value <= p.AnimatedFrom)
                        {
                            p.AnimatingForwards = true;
                        }

                        if (p.Value >= p.AnimatedTo)
                        {
                            p.AnimatingForwards = false;
                        }

                    }
                }
                TryDrawTree();   
            }
        }

        private void TryDrawTree()
        {
            if(fullyLoaded)
            {
                try //object may be used by user dragging
                {
                    
                    //ajdust pen if settings changed
                    if (_lastPenOpacity != (int)SettingsMap["Pen opacity"].Value || _lastPenWidth != (int)SettingsMap["Pen width"].Value)
                    {
                        _lastPenOpacity = (int)SettingsMap["Pen opacity"].Value;
                        _lastPenWidth = (int)SettingsMap["Pen width"].Value;

                        _penForeground = new Pen(Color.FromArgb((int)SettingsMap["Pen opacity"].Value, _penForeground.Color), (int)SettingsMap["Pen width"].Value);                       
                    }
                } catch (Exception e)
                {
                    
                    Console.WriteLine(e.Message + ":\n"+ e.StackTrace );
                }

                Task t = Task.Run(delegate {
                    _treeFactory.CreateNewTree(new System.Drawing.Size(new System.Drawing.Point(_resolutionX, _resolutionY)),
                        new System.Drawing.Point((int)SettingsMap["X offset"].Value, (int)SettingsMap["Y offset"].Value),
                        (int)SettingsMap["Generations"].Value,
                        (int)SettingsMap["Root count"].Value,
                        (int)SettingsMap["Child count"].Value,
                        (int)SettingsMap["Zoom level"].Value,
                        SettingsMap["Pi offset"].Value,
                        SettingsMap["Deviation"].Value,
                        SettingsMap["Child length"].Value,
                        SettingsMap["Hue change"].Value,
                        _penForeground,
                        _brushBackground,
                        _lineStyle);
                });
            }
            
        }
        #endregion

        #region Save button wiring




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

        #endregion

        #region Slider wiring

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(!_atLeastOneParameterIsAnimated)
            {
                TryDrawTree();
            }
            
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
                _penForeground = new Pen(Color.FromArgb((int)SettingsMap["Pen opacity"].Value, cd.Color), (int)SettingsMap["Pen width"].Value);
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
        private Color GetOppositeColor(Color original)
        {
            int r = 255 - original.R;
            int g = 255 - original.G;
            int b = 255 - original.B;

            return Color.FromArgb(r, g, b);
        }
        #endregion

        #region Line Style Selection
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

        #region Mouse-Image interaction

        private void imageMainView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if(_isUserDraggingImage)
                {
                    System.Windows.Point _mousePositionCurrent = Mouse.GetPosition(appMainWindow);
                    System.Windows.Point _mousePosDifference = new System.Windows.Point(
                       _mousePositionCurrent.X - _mousePositionWhenUserStartedDraggingImage.X ,
                       _mousePositionCurrent.Y - _mousePositionWhenUserStartedDraggingImage.Y );

                    bool previousXoffsetAnimatedValue = SettingsMap["X offset"].Animated;
                    bool previousYoffsetAnimatedValue = SettingsMap["Y offset"].Animated;

                    SettingsMap["X offset"].Animated = true;
                    SettingsMap["X offset"].Animated = true;

                    SettingsMap["X offset"].Value = _offsetWhenUserStartedDraggingImage.X + _mousePosDifference.X ;
                    SettingsMap["Y offset"].Value = _offsetWhenUserStartedDraggingImage.Y + _mousePosDifference.Y ;


                    SettingsMap["X offset"].Animated = previousXoffsetAnimatedValue;
                    SettingsMap["X offset"].Animated = previousYoffsetAnimatedValue;


                }
                else
                {
                    _isUserDraggingImage = true;
                    _mousePositionWhenUserStartedDraggingImage = Mouse.GetPosition(appMainWindow);
                    _offsetWhenUserStartedDraggingImage = new System.Windows.Point(SettingsMap["X offset"].Value, SettingsMap["Y offset"].Value);
                }
               
            }
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                _isUserDraggingImage = false;
            }
        }

        private void imageMainView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta != 0 && SettingsMap["Zoom level"].Value + e.Delta / 5 > 0)
            {
                SettingsMap["Zoom level"].Value += e.Delta / 5;
            }
        }
        #endregion

        #region Regex
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string text = e.Text;
            if (!IsTextNumerical(text))
            {
                e.Handled = true;
            }
        }

        private static bool IsTextNumerical(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
        #endregion

        //called by loop checkboxes
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TryDrawTree();
        }

        /*
        private void checkboxAutosave_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select or create a folder to save all the deviations of the selected settings to:";
                DialogResult result = fbd.ShowDialog();

                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    _autosaveDirectory = fbd.SelectedPath + "\\";
                    // sliderDeviation.Value = 0;
                    _animateAndSave = (bool)checkboxAutosave.IsChecked;
                    
                    TryDrawTree();
                }
                else
                {
                    checkboxAutosave.IsChecked = false;
                }
            }
        }

        */
    }
}
