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
    /// Результат поиска объекта из предметной области
    /// </summary>
    class ObjectResult : WordResult
    {
        /// <summary>
        /// Тип вершины (например, Ингредиент, Коктейль и тд)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Информация по дугам, имена которых можно получить из метазнаний
        /// </summary>
        public TreeViewItem InfFromMetadata { get; set; }

        /// <summary>
        /// Список всех предков для вершины (если у вершины есть дуги is_a или is_instance)
        /// </summary>
        public List<string> WayToParent { get; set; }

        /// <summary>
        /// Используется в:
        /// </summary>
        public List<string> Usages { get; set; }

        public ObjectResult()
        {
            InfFromMetadata = new TreeViewItem();
            WayToParent = new List<string>();
        }

        public override GroupBox Print()
        {
            var stackPanel = new StackPanel();
            var groupBox = new GroupBox { Header = "Результат поиска для " + Name, Content = stackPanel };
            Expander exp;
            //тип
            stackPanel.Children.Add(new Label {Content = Name + " - это " + Type});
            //иерархия
            if (WayToParent.Count > 0)
            {
                exp = new Expander {Header = "Иерархия", Content = new ListBox {ItemsSource = WayToParent}};
                stackPanel.Children.Add(exp);
            }
            //InfFromMetadata
            if (InfFromMetadata.Header != null)
            {
                var tw = new TreeView();
                tw.Items.Add(InfFromMetadata);
                exp = new Expander { Header = "Общая информация", Content = tw };
                stackPanel.Children.Add(exp);
            }
            //используется в:
            if (Usages!=null && Usages.Count > 0)
            {
                exp = new Expander {Header = "Используется в:", Content = new ListBox {ItemsSource = Usages}};
                stackPanel.Children.Add(exp);
            }
            //похожие запросы
            if (SimilarQueries!=null && SimilarQueries.Count > 0)
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
