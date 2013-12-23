using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Kernel;

namespace Consulting
{
    public class Searcher
    {
        #region Переменные

        /// <summary>
        /// Запрос, введённый пользователем
        /// </summary>
        private readonly string _input;

        /// <summary>
        /// Запрос, разбитый на слова (смысловые части)
        /// </summary>
        private readonly List<string> _inputParts;

        /// <summary>
        /// Вершины сети, соответствующие словам из запроса
        /// </summary>
        private readonly List<Node> _nodesInQuery;

        /// <summary>
        /// неименованные вершины сем. сети, соответствующие словам из запроса
        /// </summary>
        private readonly List<Node> _unnamedNodesInQuery;

        private readonly Action<string> _executeSimilarQuery;

        public WorkMemory WorkMemory;

        private readonly bool _searchSynonyms;

        private readonly bool _searchClasses;

        private UnnamedSearcher _unnamedSearcher;

        #endregion

        #region Инициализация

        public Searcher(string input, Action<string> executeSimilarQuery, bool searchSynonyms, bool searchClasses)
        {
            WorkMemory = new WorkMemory();
            _executeSimilarQuery = executeSimilarQuery;
            _searchSynonyms = searchSynonyms;
            _searchClasses = searchClasses;
            _input = input;
            var cw = new ConceptWorker(SemanticWeb.Web().Nodes.Max(x => x.Name.Length), _searchSynonyms);
            cw.FindAll(_input, null);
            _inputParts = cw.Concepts.Where(x => x.IsRecognized).Select(x => x.Name).ToList();

            _nodesInQuery = GetNodesFromQuery();
            WorkMemory.WorkedNodes.AddRange(_nodesInQuery);
            _unnamedNodesInQuery = GetUnnamedNodesFromQuery();
            WorkMemory.WorkedNodes.AddRange(_unnamedNodesInQuery);

            for (int i = 0; i < _nodesInQuery.Count; i++)
            {
                var node = _nodesInQuery[i];
                var uNode = _unnamedNodesInQuery[i];
                WorkMemory.WorkedArcs.AddRange(SemanticWeb.Web().Arcs.Where(x => x.From == uNode.ID && x.Name == "#Name"
                                                                                 && x.To == node.ID));
            }

            _unnamedSearcher = new UnnamedSearcher(_input, _executeSimilarQuery, WorkMemory, _searchClasses, _unnamedNodesInQuery); 
        }

        public Searcher(Action<string> executeSimilarQuery, bool searchSynonyms, bool searchClasses)
        {
            _executeSimilarQuery = executeSimilarQuery;
            _searchSynonyms = searchSynonyms;
            _searchClasses = searchClasses;
            WorkMemory = new WorkMemory();
        }

        /// <summary>
        /// По словам из запроса ищет вершины сем. сети с соответствующими именами
        /// </summary>
        /// <returns>Список именованных вершин, соответствующих словам из запроса</returns>
        private List<Node> GetNodesFromQuery()
        {
            return _inputParts
                .Where(
                    word =>
                    SemanticWeb.Web().NodeExists(word) && !string.IsNullOrEmpty(word) &&
                    word.Trim().ToUpper() != "#System".ToUpper())
                .Select(word => SemanticWeb.Web().Mota(SemanticWeb.Web().Atom(word))).ToList();
        }

        /// <summary>
        /// Находит неименованные вершины сем. сети, именами которых являются слова из запроса (т.е. выходит дуга "#Name" в _nodesInQuery)
        /// </summary>
        /// <returns>Список неименованных вершин, соответствующих словам из запроса</returns>
        private List<Node> GetUnnamedNodesFromQuery()
        {
            return _nodesInQuery
                .SelectMany(node => SemanticWeb.Web().GetNodesDirectedToMe(node.ID, "#Name")).ToList();
        }

        #endregion

        public QueryResult Search()
        {
            return _unnamedSearcher.Search();
        }
    }

}
