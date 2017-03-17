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

        private TreeFactory _treeFactory;
        private Bitmap displayedBitmap;
        private Random _random = new Random();
       

        
        private int _penOpacity = 50;
        private int _penWidth = 1;
        private int _resolutionX = 1920;
        private int _resolutionY = 1080;
        private Pen _penForeground = new Pen(new SolidBrush(Color.FromArgb(50, Color.Black)),1);
        private Brush _brushBackground = new SolidBrush(Color.White);        
        private bool _animateAndSave = false;
        private bool _atLeastOneParameterIsAnimated = false;
        private string _autosaveDirectory = "";
        private LineStyle _lineStyle = LineStyle.Normal;
        

        bool fullyLoaded = false;
        #endregion

        public List<Parameter> Settings;
        public Dictionary<string, Parameter> SettingsMap;
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

            Settings.Add(new Parameter() { Name = "Deviation", Value = 15, MinimumValue = 0, MaximumValue = 360 });
            Settings.Add(new Parameter() { Name = "Pi offset", Value = 0, MinimumValue = -10, MaximumValue = 10 });
            Settings.Add(new Parameter() { Name = "Generations", Value = 3, MinimumValue = 1, MaximumValue = 15 });
            Settings.Add(new Parameter() { Name = "Child count", Value = 4, MinimumValue = 2, MaximumValue = 15});
            Settings.Add(new Parameter() { Name = "Pen opacity", Value = 80, MinimumValue = 1, MaximumValue = 254 });
            Settings.Add(new Parameter() { Name = "Pen width", Value = 2, MinimumValue = 1, MaximumValue = 20 });
            Settings.Add(new Parameter() { Name = "Zoom level", Value = 80, MinimumValue = 0, MaximumValue = 3000 });
            Settings.Add(new Parameter() { Name = "Hue change", Value = 0, MinimumValue = 0, MaximumValue = 360 });
            Settings.Add(new Parameter() { Name = "Root count", Value = 4, MinimumValue = 1, MaximumValue = 10 });
            Settings.Add(new Parameter() { Name = "Child length", Value = 1, MinimumValue = 0, MaximumValue = 2 });

           

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

                        if(p.Value == p.AnimatedFrom)
                        {
                            p.AnimatingForwards = true;
                        }

                        if (p.Value == p.AnimatedTo)
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
                _penForeground = new Pen(Color.FromArgb((int)SettingsMap["Pen opacity"].Value, _penForeground.Color), (int)SettingsMap["Pen width"].Value);

                Task t = Task.Run(delegate {
                    _treeFactory.CreateNewTree(_resolutionX,
                        _resolutionY,
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
        private Color GetOppositeColor(Color original)
        {
            int r = 255 - original.R;
            int g = 255 - original.G;
            int b = 255 - original.B;

            return Color.FromArgb(r, g, b);
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TryDrawTree();
        }
    }
}
