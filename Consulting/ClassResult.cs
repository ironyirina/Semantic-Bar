using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Kernel;

namespace Consulting
{
    class ClassResult : ObjectResult
    {
        /// <summary>
        /// Имена всех подклассов и экземпляров
        /// </summary>
        public TreeViewItem Instances { get; set; }

        public ClassResult()
        {
            Instances = new TreeViewItem();
        }

        public ClassResult(ObjectResult res)
        {  
           InfFromMetadata = res.InfFromMetadata;
           Instances = new TreeViewItem();
           Name = res.Name;
           SimilarQueries = res.SimilarQueries;
           Type = res.Type;
           UnnamedMeNode = res.UnnamedMeNode;
           WayToParent = res.WayToParent;
        }

        public override GroupBox Print()
        {
            var stackPanel = new StackPanel();
            var groupBox = new GroupBox { Header = "Результат поиска для " + Name, Content = stackPanel };
            Expander exp;
            //тип
            stackPanel.Children.Add(new Label { Content = Name + " - это " + Type });
            //иерархия
            if (WayToParent.Count > 0)
            {
                exp = new Expander { Header = "Иерархия", Content = new ListBox { ItemsSource = WayToParent } };
                stackPanel.Children.Add(exp);
            }
            //Экземпляры и потомки
            if (Instances.Header != null)
            {
                var tw = new TreeView();
                tw.Items.Add(Instances);
                exp = new Expander {Header = "Потомки", Content = tw};
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
