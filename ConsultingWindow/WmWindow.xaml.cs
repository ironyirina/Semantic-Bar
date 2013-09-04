using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Kernel;

namespace ConsultingWindow
{
    /// <summary>
    /// Interaction logic for WmWindow.xaml
    /// </summary>
    public partial class WmWindow : Window
    {
        public List<Node> Nodes { get; set; }
        public List<Arc> Arcs { get; set; }

        public WmWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded1(object sender, RoutedEventArgs e)
        {
            lbNodes.ItemsSource = Nodes.Distinct();
            lbArcs.ItemsSource = Arcs.Distinct();
        }


    }
}
