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
            var groupBox = base.Print();
            //Экземпляры и потомки
            if (Instances.Header != null)
            {
                var tw = new TreeView();
                tw.Items.Add(Instances);
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
