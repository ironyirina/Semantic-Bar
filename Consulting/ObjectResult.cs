using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
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
        /// Информация из файла (для коктейлей)
        /// </summary>
        public List<string> InfFromFile { get; set; } 

        /// <summary>
        /// Список всех предков для вершины (если у вершины есть дуги is_a или is_instance)
        /// </summary>
        public List<string> WayToParent { get; set; }

        /// <summary>
        /// Запросы по дугам из Relations (более сладкий и тп)
        /// </summary>
        public List<string> RelativeQueries { get; set; }

        /// <summary>
        /// Связанные запросы
        /// </summary>
        public List<string> SimilarQueries { get; set; }

        public ObjectResult()
        {
            InfFromMetadata = new TreeViewItem();
            WayToParent = new List<string>();
            InfFromFile = new List<string>();
            RelativeQueries = new List<string>();
            SimilarQueries = new List<string>();
        }

        public override GroupBox Print()
        {
            var stackPanel = new StackPanel();
            var groupBox = new GroupBox { Header = "Результат поиска для " + Name, Content = stackPanel };
            Expander exp;
            //тип
            stackPanel.Children.Add(new Label {Content = Name + " - это " + Type});
            //иерархия
            if (WayToParent.Count > 1)
            {
                var lb = new ListBox {ItemsSource = WayToParent};
                lb.MouseDoubleClick += (sender, args) => ExecuteSimilarQuery(lb.SelectedItem.ToString());
                exp = new Expander {Header = "Иерархия", Content = lb};
                stackPanel.Children.Add(exp);
            }
            //информация из файла
            if (InfFromFile != null && InfFromFile.Count > 0)
            {
                var lb = new ListBox { ItemsSource = InfFromFile };
                lb.MouseDoubleClick += (sender, args) => ExecuteSimilarQuery(lb.SelectedItem.ToString());
                exp = new Expander { Header = "Информация из файла", Content = lb };
                stackPanel.Children.Add(exp);
            }
            //InfFromMetadata
            if (InfFromMetadata.Header != null && InfFromMetadata.Items.Count > 0)
            {
                
                var tw = new TreeView();
                tw.Items.Add(InfFromMetadata);
                tw.MouseDoubleClick += (sender, args) => ExecuteSimilarQuery(((TreeViewItem)tw.SelectedItem).Header.ToString());
                exp = new Expander { Header = "Общая информация (из метазнаний)", Content = tw };
                stackPanel.Children.Add(exp);
            }
            //похожие запросы
            if (SimilarQueries!=null && SimilarQueries.Count > 0)
            {
                var lb = new ListBox { ItemsSource = SimilarQueries };
                lb.MouseDoubleClick += (sender, args) => ExecuteSimilarQuery(lb.SelectedItem.ToString());
                exp = new Expander { Header = "Связанные запросы", Content = lb };
                stackPanel.Children.Add(exp);
            }
            //Relative Queries
            if (RelativeQueries != null && RelativeQueries.Count > 0)
            {
                var lb = new ListBox {ItemsSource = RelativeQueries};
                lb.MouseDoubleClick += RelativeQueryOnMouseDoubleClick;
                exp = new Expander {Header = "Ещё...", Content = lb};
                stackPanel.Children.Add(exp);
            }
            if (stackPanel.Children.Count > 0)
            {
                groupBox.Content = stackPanel;
                return groupBox;
            }
            return null;
        }

        private void RelativeQueryOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
