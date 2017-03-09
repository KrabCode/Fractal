﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DB9E1F8F64AACBE8CA509AFC8C6EBBA8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Fractal;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Fractal {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imageMainView;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sliderDeviation;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sliderDetail;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sliderChildCount;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sliderPenOpacity;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sliderSize;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sliderPiOffset;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider sliderRootCount;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btSave;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Fractal;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.imageMainView = ((System.Windows.Controls.Image)(target));
            return;
            case 2:
            this.sliderDeviation = ((System.Windows.Controls.Slider)(target));
            
            #line 37 "..\..\MainWindow.xaml"
            this.sliderDeviation.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.sliderDeviation_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.sliderDetail = ((System.Windows.Controls.Slider)(target));
            
            #line 38 "..\..\MainWindow.xaml"
            this.sliderDetail.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.sliderDetail_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.sliderChildCount = ((System.Windows.Controls.Slider)(target));
            
            #line 39 "..\..\MainWindow.xaml"
            this.sliderChildCount.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.sliderChildCount_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.sliderPenOpacity = ((System.Windows.Controls.Slider)(target));
            
            #line 40 "..\..\MainWindow.xaml"
            this.sliderPenOpacity.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.sliderPenOpacity_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.sliderSize = ((System.Windows.Controls.Slider)(target));
            
            #line 41 "..\..\MainWindow.xaml"
            this.sliderSize.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.sliderSize_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.sliderPiOffset = ((System.Windows.Controls.Slider)(target));
            
            #line 42 "..\..\MainWindow.xaml"
            this.sliderPiOffset.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.sliderPiOffset_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.sliderRootCount = ((System.Windows.Controls.Slider)(target));
            
            #line 43 "..\..\MainWindow.xaml"
            this.sliderRootCount.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.sliderRootCount_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btSave = ((System.Windows.Controls.Button)(target));
            
            #line 45 "..\..\MainWindow.xaml"
            this.btSave.Click += new System.Windows.RoutedEventHandler(this.btSave_Click_1);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

