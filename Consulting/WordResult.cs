using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Kernel;

namespace Consulting
{
    /// <summary>
    /// Результат поиска для слова
    /// </summary>
    public class WordResult
    {
        /// <summary>
        /// Слово, для которого выполняется поиск
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Неименованная вершина в сем. сети, соответствующая искомому слову
        /// </summary>
        public Node UnnamedMeNode { get; set; }

        /// <summary>
        /// Связанные запросы
        /// </summary>
        public List<string> SimilarQueries { get; set; }

        public WordResult()
        {
            UnnamedMeNode = new Node();
            SimilarQueries = new List<string>();
        }

        public virtual GroupBox Print()
        {
            var stackPanel = new StackPanel();
            var groupBox = new GroupBox {Header = "Результат поиска для " + Name, Content = stackPanel};
            Expander exp = null;
            if (SimilarQueries.Count > 0)
            {
                exp = new Expander {Header = "Похожие запросы", Content = new ListBox {ItemsSource = SimilarQueries}};
            }
            if (exp != null)
            {
                stackPanel.Children.Add(exp);
                groupBox.Content = stackPanel;
                return groupBox;
            }
            return null;
        }
    }
}
