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

        public Action<string> ExecuteSimilarQuery { get; set; }

        public WordResult()
        {
            UnnamedMeNode = new Node();
        }

        public virtual GroupBox Print()
        {
            return null;
        }

    }
}
