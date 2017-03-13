﻿using Microsoft.Win32;
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
        private bool _animate;
        private bool _animatingForwards;
        private bool _animateAndSave = false;        
        private int _autosavedPicsAlready = 0;
        private string _autosaveDirectory = "";
        private double _deviationChangeBetweenFrames = 1;

        private double _deviation = 0;
        private double _piOffset = 0;
        private int _detail = 6;
        private int _childCount = 4;
        private int _penOpacity = 50;
        private int _penWidth = 1;
        private int _size = 20;
        private int _resolutionX = 1920;
        private int _resolutionY = 1080;
        private Pen _penForeground = new Pen(new SolidBrush(Color.FromArgb(50, Color.Black)),1);
        private Brush _brushBackground = new SolidBrush(Color.White);
        private int _rootCount = 1;

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

            DrawTree();
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
                            }
                        }
                        DrawTree();
                    }
                    imageMainView.Source = BitmapConverter.Bitmap2BitmapSource(e.imageToDraw);
                }));
            
            return null;
        }

        private void DrawTree()
        {
            Task t = Task.Run(delegate { _logic.DrawTree(_resolutionX, _resolutionY, _deviation, _detail, _childCount, _penForeground, _brushBackground, _size, _piOffset, _rootCount); });
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
            _deviation = Math.Round(sliderDeviation.Value,0);
            DrawTree();
        }

        private void sliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _size = (int)sliderSize.Value;
            DrawTree();
        }

        private void sliderDetail_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _detail = (int)sliderDetail.Value;
            DrawTree();
        }
        
        private void sliderRootCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _rootCount = (int)sliderRootCount.Value;
            DrawTree();
        }

        private void sliderChildCount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _childCount = (int)sliderChildCount.Value;
            DrawTree();
        }
        
        private void sliderPiOffset_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _piOffset = sliderPiOffset.Value;
            DrawTree();
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
                DrawTree();
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
                DrawTree();
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
                    
                    DrawTree();
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
            DrawTree();
        }

        private void sliderPenWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _penWidth = (int)sliderPenWidth.Value;
            _penForeground = new Pen(Color.FromArgb(_penOpacity, _penForeground.Color), _penWidth);
            DrawTree();
        }

        private void sliderPenOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _penOpacity = (int)sliderPenOpacity.Value;
            _penForeground = new Pen(Color.FromArgb( _penOpacity,_penForeground.Color), _penWidth);
            DrawTree();
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
                    _resolutionX = resolutionX;
                    DrawTree();
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
                    _resolutionY = resolutionY;
                    DrawTree();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        

    }
}
