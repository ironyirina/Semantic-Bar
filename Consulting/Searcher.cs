using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<List<Node>> _unnamedNodesInQuery;

        private readonly Action<string> _executeSimilarQuery;

        public WorkMemory WorkMemory { get; set; }

        private readonly bool _searchSynonyms;

        private readonly bool _searchClasses;

        private List<UnnamedSearcher> _unnamedSearcher;

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
            foreach (var unnamedNodesList in _unnamedNodesInQuery)
            {
                WorkMemory.WorkedNodes.AddRange(unnamedNodesList);
            }

            for (int i = 0; i < _nodesInQuery.Count; i++)
            {
                var node = _nodesInQuery[i];
                var i1 = i;
                var uNodeList = _unnamedNodesInQuery.Where(x => x.Count > i1).Select(x => x[i1]);
                foreach (var uNode in uNodeList)
                {
                    Node node1 = uNode;
                    WorkMemory.WorkedArcs.AddRange(SemanticWeb.Web().Arcs.Where(x => x.From == node1.ID && x.Name == "#Name"
                                                                                 && x.To == node.ID));
                }
            }

            _unnamedSearcher = new List<UnnamedSearcher>();
            foreach (var unnanedNodesList in _unnamedNodesInQuery)
            {
                _unnamedSearcher.Add(new UnnamedSearcher(_input, _executeSimilarQuery, WorkMemory, _searchClasses, unnanedNodesList));
            }
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

        #region Get Unnamed Nodes from the Query
        /// <summary>
        /// Находит неименованные вершины сем. сети, именами которых являются слова из запроса (т.е. выходит дуга "#Name" в _nodesInQuery)
        /// </summary>
        /// <returns>Список неименованных вершин, соответствующих словам из запроса</returns>
        private List<List<Node>> GetUnnamedNodesFromQuery()
        {
            var auxList = GetAuxiliaryUnnamedNodes();
            return CartesianProduct(auxList).Select(list => list.ToList()).ToList();
        }


        private IEnumerable<List<Node>> GetAuxiliaryUnnamedNodes()
        {
            /* Пример: Пусть шейкер - это инструмент и ёмкость, а апельсин - фрукт и сок.
             * Пусть есть соответствующие именованные вершины: {{Cocktail, 3}, {Shaker, 20}, {Orange, 15}}
             * И есть неименованные вершины, из которых выходят дуги Name:
             * {_, 2} --Name--> {Cocktail, 3}
             * {_, 19} --Name--> {Shaker, 20} // Ёмкость
             * {_, 21} --Name--> {Shaker, 20} // Инструмент
             * {_, 14} --Name--> {Orange, 15} // Фрукт
             * {_, 18} --Name--> {Orange, 15} // Сок
             * Тогда в результате запроса "Коктейль в шейкере с апельсином" будет сформирован список
             * _nodesInQuery = {{Cocktail, 3}, {Shaker, 20}, {Orange, 15}}
             * auxNodes = {{2}, {19, 21}, {14, 18}}
             * Далее из на основе этого списка будет получен список всех возможных значений, т.е. декартово произведение:
             * {{2, 19, 14}, {2, 21, 14}, {2, 19, 18}, {2, 21, 18}}.
             */
            var auxNodes = new List<List<Node>>();
            for (int i = 0; i < _nodesInQuery.Count; i++)
            {
                var namedNode = _nodesInQuery[i];
                auxNodes.Add(new List<Node>());
                auxNodes[i].AddRange(SemanticWeb.Web().GetAllUnnamedNodesForName(namedNode.Name));
            }
            return auxNodes;
        }

        /// <summary>
        /// Вычисление декартова произведения списков
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequences"></param>
        /// <returns></returns>
        static IEnumerable<IEnumerable<T>> CartesianProduct<T>(IEnumerable<IEnumerable<T>> sequences)
        { 
            IEnumerable<IEnumerable<T>> result = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(result,
                (current, s) => (from seq in current from item in s select seq.Concat(new[] {item}).ToList()));
        }
        #endregion

        #endregion

        public QueryResult Search()
        {
            if (_unnamedSearcher.Count == 0)
                return new QueryResult {Message = "Поиск не дал результатов"};
            if (_unnamedSearcher.Count == 1)
                return _unnamedSearcher[0].Search();
            var res = new QueryResult {Message = "Волею Хаоса сему понятию дана неоднозначность"};
            for (int i = 0; i < _unnamedSearcher.Count; i++)
            {
                var unnamedSearcher = _unnamedSearcher[i];
                var qr = unnamedSearcher.Search();
                string s = " для ";
                for (int j = 0; j < _unnamedNodesInQuery[i].Count; j++)
                {
                    var type = SemanticWeb.Web().GetClosestParentName(_unnamedNodesInQuery[i][j]);
                    s += " " + _nodesInQuery[j].Name + " ";
                    if (type != "#MetaObjects")
                        s += " (" + type + ") ";
                    s += "+";
                }
                qr.Message += s.Substring(0, s.Length - 1);
                res.PolysemanticResult.Add(qr);
            }
            return res;
        }
    }

}
