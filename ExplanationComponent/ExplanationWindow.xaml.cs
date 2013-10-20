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
using System.Windows.Shapes;
using Kernel;

namespace ExplanationComponent
{
    /// <summary>
    /// Interaction logic for ExplanationWindow.xaml
    /// </summary>
    public partial class ExplanationWindow
    {
        #region Private Fields

        private readonly string _fileName;
        private readonly List<Node> _nodes;
        private readonly List<Arc> _arcs;

        #endregion

        #region Ctor
        public ExplanationWindow(string fileName, List<Node> nodes, List<Arc> arcs)
        {
            _fileName = fileName;
            _nodes = nodes;
            _arcs = arcs;
            InitializeComponent();

            DD.CanEdit = false;
            DD.FileName = _fileName;
            DD.Load = true;
            DD.PrintGraph(SemanticWeb.Web());
            DD.AddStrokeThickness(_nodes, _arcs);
        }

        static ExplanationWindow()
        {
            ZoomInCommand = new RoutedUICommand("ZoomIn", "ZoomIn", typeof(ExplanationWindow));
            ZoomInCommand.InputGestures.Add(new KeyGesture(Key.Add, ModifierKeys.Control));
            ZoomOutCommand = new RoutedUICommand("ZoomOut", "ZoomOut", typeof(ExplanationWindow));
            ZoomOutCommand.InputGestures.Add(new KeyGesture(Key.Subtract, ModifierKeys.Control));
            FitSizeCommand = new RoutedUICommand("FitSize", "FitSize", typeof(ExplanationWindow));
            NoZoomCommand = new RoutedUICommand("NoZoom", "NoZoom", typeof(ExplanationWindow));
        }
        #endregion

        #region Zoom-Panel
        #region ZoomIn
        public static RoutedUICommand ZoomInCommand { get; private set; }

        private void ZoomInExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DD.ZoomFactor = Math.Min(1000, DD.ZoomFactor + 10);
        }

        private void ZoomInCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        #region ZoomOut
        public static RoutedUICommand ZoomOutCommand { get; private set; }

        private void ZoomOutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DD.ZoomFactor = Math.Max(20, DD.ZoomFactor - 10);
        }

        private void ZoomOutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region FitSize
        public static RoutedUICommand FitSizeCommand { get; private set; }

        private void FitSizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DD.FitSize();
        }

        private void FitSizeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region NoZoom
        public static RoutedUICommand NoZoomCommand { get; private set; }

        private void NoZoomExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DD.ZoomFactor = 100;
        }

        private void NoZoomCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion
        #endregion 
    }
}
