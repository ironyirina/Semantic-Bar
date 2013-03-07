using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel;
using System.Windows.Controls;
using System.Reflection;


namespace Consulting
{
    /// <summary>
    /// Результат поиска для MetaObjects
    /// </summary>
    public class MetaResult : WordResult
    {
        /// <summary>
        /// Где используется
        /// </summary>
        public List<string> Usages { get; set; }

        /// <summary>
        /// Атрибуты
        /// </summary>
        public TreeViewItem Attributes { get; set; }

        /// <summary>
        /// Вершины, куда можно попасть по дугам с именем name из #System
        /// </summary>
        public TreeViewItem Instances { get; set; }

        public MetaResult()
        {
            Usages = new List<string>();
            Attributes = new TreeViewItem();
            Instances = new TreeViewItem();
        }

        public override GroupBox Print()
        {
            var stackPanel = new StackPanel();
            var groupBox = new GroupBox { Header = "Результат поиска для " + Name, Content = stackPanel };
            Expander exp = null;
            //атрибуты
            if (Attributes.Header != null)
            {
                var tw = new TreeView();
                tw.Items.Add(Attributes);
                exp = new Expander {Header = "Атрибуты", Content = tw};
                stackPanel.Children.Add(exp);
            }
            //использование
            if (Usages.Count > 0)
            {
                exp = new Expander {Header = "Используется в:", Content = new ListBox {ItemsSource = Usages}};
                stackPanel.Children.Add(exp);
            }
            //Объекты
            if (Instances.Header != null)
            {
                var tw = new TreeView();
                tw.Items.Add(Instances);
                exp = new Expander { Header = "Объекты", Content = tw };
                stackPanel.Children.Add(exp);
            }
            //похожие запросы
            if (SimilarQueries.Count > 0)
            {
                exp = new Expander { Header = "Похожие запросы", Content = new ListBox { ItemsSource = SimilarQueries } };
                stackPanel.Children.Add(exp);
            }
            if (stackPanel.Children.Count > 0)
            {
                groupBox.Content = stackPanel;
                return groupBox;
            }
            return null;
        }
    }
}
