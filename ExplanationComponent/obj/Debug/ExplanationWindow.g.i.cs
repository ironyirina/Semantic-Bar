﻿#pragma checksum "..\..\ExplanationWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AFADBB27E7FCDA2C22536C77C2242675"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DiagramControls;
using ExplanationComponent;
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


namespace ExplanationComponent {
    
    
    /// <summary>
    /// ExplanationWindow
    /// </summary>
    public partial class ExplanationWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 33 "..\..\ExplanationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button zoomInButton;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\ExplanationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button zoomOutButton;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\ExplanationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button fitButton;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\ExplanationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button noZoomButton;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\ExplanationWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DiagramControls.DiagramControl DD;
        
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
            System.Uri resourceLocater = new System.Uri("/ExplanationComponent;component/explanationwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ExplanationWindow.xaml"
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
            
            #line 17 "..\..\ExplanationWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ZoomInExecuted);
            
            #line default
            #line hidden
            
            #line 17 "..\..\ExplanationWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.ZoomInCanExecute);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 19 "..\..\ExplanationWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ZoomOutExecuted);
            
            #line default
            #line hidden
            
            #line 19 "..\..\ExplanationWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.ZoomOutCanExecute);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 21 "..\..\ExplanationWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.FitSizeExecuted);
            
            #line default
            #line hidden
            
            #line 21 "..\..\ExplanationWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.FitSizeCanExecute);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 23 "..\..\ExplanationWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.NoZoomExecuted);
            
            #line default
            #line hidden
            
            #line 23 "..\..\ExplanationWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.NoZoomCanExecute);
            
            #line default
            #line hidden
            return;
            case 5:
            this.zoomInButton = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.zoomOutButton = ((System.Windows.Controls.Button)(target));
            return;
            case 7:
            this.fitButton = ((System.Windows.Controls.Button)(target));
            return;
            case 8:
            this.noZoomButton = ((System.Windows.Controls.Button)(target));
            return;
            case 9:
            this.DD = ((DiagramControls.DiagramControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

