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
            ExecuteSimilarQuery = res.ExecuteSimilarQuery;
        }

        public override GroupBox Print()
        {
            var groupBox = base.Print();
            var stackPanel = (StackPanel)groupBox.Content;
            //Экземпляры и потомки
            if (Instances.Header != null)
            {
                var tw = new TreeView();
                tw.Items.Add(Instances);
                tw.MouseDoubleClick += (sender, args) => ExecuteSimilarQuery(((TreeViewItem)tw.SelectedItem).Header.ToString());
                var exp = new Expander {Header = "Потомки", Content = tw};
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
