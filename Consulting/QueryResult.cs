using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kernel;

namespace Consulting
{
    /// <summary>
    /// Результат запроса
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// Слова из запроса
        /// </summary>
        public List<string> Names;

        /// <summary>
        /// неименованные вершины сем. сети, соответствующие словам из запроса
        /// </summary>
        public List<Node> UnnamedNodes;

        /// <summary>
        /// Список результатов для каждого из слов
        /// </summary>
        public List<WordResult> EveryWordResult { get; set; }

        /// <summary>
        /// Связи между понятиями
        /// </summary>
        public List<string> Connections { get; set; }

        /// <summary>
        /// Сообщение о выполнении запроса
        /// </summary>
        public string Message { get; set; }

        public QueryResult()
        {
            Names = new List<string>();
            UnnamedNodes = new List<Node>();
            EveryWordResult = new List<WordResult>();
            Connections = new List<string>();
        }
    }
}
