﻿#pragma checksum "..\..\DiagramControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AD0F97E157767BEDC295F03FAB05E0E7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MindFusion.Diagramming.Wpf;
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


namespace DiagramControls {
    
    
    /// <summary>
    /// DiagramControl
    /// </summary>
    public partial class DiagramControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\DiagramControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer scrollViewer1;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\DiagramControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MindFusion.Diagramming.Wpf.Diagram DD;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\DiagramControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ContextMenu myContextMenu;
        
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
            System.Uri resourceLocater = new System.Uri("/DiagramControls;component/diagramcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\DiagramControl.xaml"
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
            this.scrollViewer1 = ((System.Windows.Controls.ScrollViewer)(target));
            return;
            case 2:
            this.DD = ((MindFusion.Diagramming.Wpf.Diagram)(target));
            
            #line 11 "..\..\DiagramControl.xaml"
            this.DD.NodeDeleted += new System.EventHandler<MindFusion.Diagramming.Wpf.NodeEventArgs>(this.DdNodeDeleted);
            
            #line default
            #line hidden
            
            #line 12 "..\..\DiagramControl.xaml"
            this.DD.NodeTextEditing += new System.EventHandler<MindFusion.Diagramming.Wpf.NodeValidationEventArgs>(this.DdNodeTextEditing);
            
            #line default
            #line hidden
            
            #line 12 "..\..\DiagramControl.xaml"
            this.DD.NodeCreated += new System.EventHandler<MindFusion.Diagramming.Wpf.NodeEventArgs>(this.DdNodeCreated);
            
            #line default
            #line hidden
            
            #line 12 "..\..\DiagramControl.xaml"
            this.DD.LinkTextEditing += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkValidationEventArgs>(this.DdLinkTextEditing);
            
            #line default
            #line hidden
            
            #line 13 "..\..\DiagramControl.xaml"
            this.DD.LinkCreated += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkEventArgs>(this.DdLinkCreated);
            
            #line default
            #line hidden
            
            #line 13 "..\..\DiagramControl.xaml"
            this.DD.LinkDeleted += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkEventArgs>(this.DdLinkDeleted);
            
            #line default
            #line hidden
            
            #line 13 "..\..\DiagramControl.xaml"
            this.DD.LinkModified += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkEventArgs>(this.DdLinkModified);
            
            #line default
            #line hidden
            
            #line 13 "..\..\DiagramControl.xaml"
            this.DD.LinkModifying += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkValidationEventArgs>(this.DdLinkModifying);
            
            #line default
            #line hidden
            
            #line 14 "..\..\DiagramControl.xaml"
            this.DD.LinkDeleting += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkValidationEventArgs>(this.DdLinkDeleting);
            
            #line default
            #line hidden
            
            #line 14 "..\..\DiagramControl.xaml"
            this.DD.NodeClicked += new System.EventHandler<MindFusion.Diagramming.Wpf.NodeEventArgs>(this.DdNodeClicked);
            
            #line default
            #line hidden
            
            #line 14 "..\..\DiagramControl.xaml"
            this.DD.LinkClicked += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkEventArgs>(this.DdLinkClicked);
            
            #line default
            #line hidden
            
            #line 14 "..\..\DiagramControl.xaml"
            this.DD.Clicked += new System.EventHandler<MindFusion.Diagramming.Wpf.DiagramEventArgs>(this.DdClicked);
            
            #line default
            #line hidden
            
            #line 15 "..\..\DiagramControl.xaml"
            this.DD.NodeSelected += new System.EventHandler<MindFusion.Diagramming.Wpf.NodeEventArgs>(this.DdNodeSelected);
            
            #line default
            #line hidden
            
            #line 15 "..\..\DiagramControl.xaml"
            this.DD.NodeDeselected += new System.EventHandler<MindFusion.Diagramming.Wpf.NodeEventArgs>(this.DdNodeDeselected);
            
            #line default
            #line hidden
            
            #line 15 "..\..\DiagramControl.xaml"
            this.DD.LinkSelected += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkEventArgs>(this.DdLinkSelected);
            
            #line default
            #line hidden
            
            #line 16 "..\..\DiagramControl.xaml"
            this.DD.LinkDeselected += new System.EventHandler<MindFusion.Diagramming.Wpf.LinkEventArgs>(this.DdLinkDeselected);
            
            #line default
            #line hidden
            
            #line 17 "..\..\DiagramControl.xaml"
            this.DD.DoubleClicked += new System.EventHandler<MindFusion.Diagramming.Wpf.DiagramEventArgs>(this.DdDoubleClicked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.myContextMenu = ((System.Windows.Controls.ContextMenu)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

