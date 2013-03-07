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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Consulting;
using Kernel;
using System.Reflection;

namespace ConsultingWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SemanticWeb Sw { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        static MainWindow()
        {
            Search = new RoutedUICommand("Search", "Search", typeof(MainWindow));
        }

        public static RoutedUICommand Search { get; private set; }

        private void SearchExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            panelRes.Children.Clear();
            var searcher = new Searcher(tbQuery.Text, Sw);
            var res = searcher.Search();
            panelRes.Children.Add(new Label {Content = res.Message});
            List<WordResult> resWords = res.EveryWordResult;
            if (resWords == null) return;
            foreach (WordResult word in resWords)
            {
                panelRes.Children.Add(word.Print());
            }
        }
    }
}
