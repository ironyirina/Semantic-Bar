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

namespace KnowledgeAcquisitionComponent
{
    /// <summary>
    /// Interaction logic for UnknownWordsWindow.xaml
    /// </summary>
    public partial class UnknownWordsWindow : Window
    {
        public List<string> UnknownWords { get; set; }

        public SemanticWeb SW { get; set; }

        public List<string> SkippedWords { get; set; }

        public UnknownWordsWindow()
        {
            InitializeComponent();
            
        }

        private void BtnAddClick1(object sender, RoutedEventArgs e)
        {
            var w = new AddConceptWindow
                        {
                            SW = SW,
                            ConceptName =
                                lbWords.SelectedItems.Count > 0 ? lbWords.SelectedItems[0].ToString() : string.Empty
                        };
            w.ShowDialog();
            Close();
        }

        private void WindowLoaded1(object sender, RoutedEventArgs e)
        {
            lbWords.ItemsSource = UnknownWords;
        }

        private void BtnSkipClick1(object sender, RoutedEventArgs e)
        {
            if (lbWords.SelectedItems.Count > 0)
                SkippedWords.Add(lbWords.SelectedItems[0].ToString());
            else
            {
                foreach (var item in lbWords.Items)
                {
                    SkippedWords.Add(item.ToString());
                }
            }
        }
    }
}
