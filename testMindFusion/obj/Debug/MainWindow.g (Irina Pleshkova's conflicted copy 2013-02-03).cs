﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B590B84F3FFBAD193F28644FE2158D15"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MindFusion.Diagramming.Wpf;
using MindFusion.UI.Wpf;
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


namespace textMindFusion {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 53 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.StatusBar sbState;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander expanderErrList;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MindFusion.Diagramming.Wpf.Diagram DD;
        
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
            System.Uri resourceLocater = new System.Uri("/textMindFusion;component/mainwindow.xaml", System.UriKind.Relative);
            
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
            
            #line 5 "..\..\MainWindow.xaml"
            ((textMindFusion.MainWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.WindowLoaded1);
            
            #line default
            #line hidden
            return;
            case 2:
            this.sbState = ((System.Windows.Controls.Primitives.StatusBar)(target));
            return;
            case 3:
            this.expanderErrList = ((System.Windows.Controls.Expander)(target));
            return;
            case 4:
            
            #line 75 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Slider)(target)).ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.Slider_ValueChanged_1);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DD = ((MindFusion.Diagramming.Wpf.Diagram)(target));
            
            #line 79 "..\..\MainWindow.xaml"
            this.DD.LinkTextEditing += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkValidationEventArgs>(this.DD_LinkTextEditing_1);
            
            #line default
            #line hidden
            
            #line 82 "..\..\MainWindow.xaml"
            this.DD.NodeTextEdited += new System.EventHandler<MindFusion.Diagramming.Wpf.EditNodeTextEventArgs>(this.DD_NodeTextEdited);
            
            #line default
            #line hidden
            
            #line 83 "..\..\MainWindow.xaml"
            this.DD.NodeCreated += new System.EventHandler<MindFusion.Diagramming.Wpf.NodeEventArgs>(this.DD_NodeCreated);
            
            #line default
            #line hidden
            
            #line 84 "..\..\MainWindow.xaml"
            this.DD.NodeDeleted += new System.EventHandler<MindFusion.Diagramming.Wpf.NodeEventArgs>(this.DD_NodeDeleted);
            
            #line default
            #line hidden
            
            #line 85 "..\..\MainWindow.xaml"
            this.DD.NodeClicked += new System.EventHandler<MindFusion.Diagramming.Wpf.NodeEventArgs>(this.DD_NodeClicked);
            
            #line default
            #line hidden
            
            #line 88 "..\..\MainWindow.xaml"
            this.DD.LinkDeleted += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkEventArgs>(this.DD_LinkDeleted);
            
            #line default
            #line hidden
            
            #line 89 "..\..\MainWindow.xaml"
            this.DD.LinkCreated += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkEventArgs>(this.DD_LinkCreated);
            
            #line default
            #line hidden
            
            #line 90 "..\..\MainWindow.xaml"
            this.DD.LinkCreating += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkValidationEventArgs>(this.DD_LinkCreating);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

