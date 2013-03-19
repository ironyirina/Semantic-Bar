using System.Collections.Generic;
using System.Windows;

namespace KnowledgeAcquisitionComponent
{
    /// <summary>
    /// Interaction logic for UnknownWordsWindow.xaml
    /// </summary>
    public partial class UnknownWordsWindow
    {
        public List<string> UnknownWords { get; set; }

        public List<string> SkippedWords { get; set; }

        public UnknownWordsWindow()
        {
            InitializeComponent();
            UnknownWords = new List<string>();
            SkippedWords = new List<string>();
        }

        private void BtnAddClick1(object sender, RoutedEventArgs e)
        {
            var w = new AddConceptWindow
                        {
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
                SkippedWords.Add((string)lbWords.SelectedItem);
            else
            {
                foreach (var item in lbWords.Items)
                {
                    SkippedWords.Add(item.ToString());
                }
            }
            Close();
        }
    }
}
