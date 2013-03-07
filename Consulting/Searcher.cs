using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
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
        /// Сем. сеть
        /// </summary>
        private readonly SemanticWeb _sw;

        /// <summary>
        /// Вершины сети, соответствующие словам из запроса
        /// </summary>
        private readonly List<Node> _nodesInQuery;

        /// <summary>
        /// неименованные вершины сем. сети, соответствующие словам из запроса
        /// </summary>
        private List<Node> _unnamedNodesInQuery;
        
        #endregion

        #region Инициализация

        public Searcher(string input, SemanticWeb sw)
        {
            _sw = sw;
            _input = input;
            var cw = new ConceptWorker(_sw, 5);
            cw.FindConcepts(_input, null);
            _inputParts = cw.Concepts.Where(x => x.IsRecognized).Select(x => x.Name).ToList();
            _nodesInQuery = GetNodesFromQuery();
            _unnamedNodesInQuery = GetUnnamedNodesFromQuery();
        }

        public Searcher(SemanticWeb sw)
        {
            _sw = sw;
        }

        /// <summary>
        /// По словам из запроса ищет вершины сем. сети с соответствующими именами
        /// </summary>
        /// <returns>Список именованных вершин, соответствующих словам из запроса</returns>
        private List<Node> GetNodesFromQuery()
        {
            return _inputParts
                .Where(word => _sw.NodeExists(word) && !string.IsNullOrEmpty(word))
                .Select(word => _sw.Mota(_sw.Atom(word))).ToList();
        }

        /// <summary>
        /// Находит неименованные вершины сем. сети, именами которых являются слова из запроса (т.е. выходит дуга "#Name" в _nodesInQuery)
        /// </summary>
        /// <returns>Список неименованных вершин, соответствующих словам из запроса</returns>
        private List<Node> GetUnnamedNodesFromQuery()
        {
            return _nodesInQuery
                .Select(node => _sw.GetNodesDirectedToMe(node.ID, "#Name")
                    .ToList()[0]).ToList();
        }

        #endregion

        #region Поиск

        /// <summary>
        /// Поиск
        /// </summary>
        /// <returns></returns>
        public QueryResult Search()
        {
            var queryResult = new QueryResult();
            if (_nodesInQuery.Count == 0) //если в сем сети нет ни одной вершины, имя которой совпадает со словом из запроса
                queryResult.Message = "Поиск не дал результатов.";
            else
            {
                for (int i = 0; i < _unnamedNodesInQuery.Count; i++)
                { //ищем каждое слово по отдельности
                    var node = _unnamedNodesInQuery[i];
                    queryResult.EveryWordResult.Add(SearchOneWord(node, _nodesInQuery[i].Name));
                }
                //ещё куча всяких неясных вещей
            }
            return queryResult;
        }

        /// <summary>
        /// Поиск одного слова
        /// </summary>
        /// <param name="unnamedNodeToSearch">Неименованная вершина, соответствующая слову из запроса</param>
        /// <param name="word">Слово, которое ищем</param>
        /// <returns></returns>
        public WordResult SearchOneWord(Node unnamedNodeToSearch, string word)
        {
            //определяем имя дуги, которой самый старший предок связан с #System
            var oldestArcName = _sw.OldestParentArc(unnamedNodeToSearch.ID);
            //Для метазнаний выполняем поиск по метазнаниям
            if (oldestArcName == "#MetaObjects")
                return SearchMetaData(unnamedNodeToSearch, word);
            //Если самая верхняя дуга имеет имя из метаобъектов, ищем слово из предметной области
            if (_sw.GetAllMetaObjectNames().Contains(oldestArcName)) 
                return SearchObjectData(unnamedNodeToSearch, word, oldestArcName);
            throw new NotImplementedException();
        }

        #region Поиск по метазнаниям
        /// <summary>
        /// Возвращает имена вершин, куда можно попасть из unnamedNodeToSearch по дугам HasAttribute
        /// </summary>
        /// <param name="unnamedNodeToSearch">Неименованная вершина, соответствующая слову из запроса</param>
        /// <param name="word">Слово, которое ищем</param>
        /// <returns></returns>
        private TreeViewItem AddAttribute(Node unnamedNodeToSearch, string word)
        {
            var treeNode = new TreeViewItem { Header = word };
            var attributes = _sw.GetAllAttr(unnamedNodeToSearch.ID, "#HasAttribute");
            foreach (Node attributeUnnamed in attributes)
            {
                string name = _sw.GetNameForUnnamedNode(attributeUnnamed);
                treeNode.Items.Add(AddAttribute(attributeUnnamed, name));
            }
            return treeNode;
        }

        /// <summary>
        /// Возвращает имена вершин, откуда можно попасть в unnamedNodeToSearch по дугам HasAttribute
        /// </summary>
        /// <param name="unnamedNodeToSearch"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        private List<string> AddUsages(Node unnamedNodeToSearch, string word)
        {
            var usages = new List<string> {word};
            var unnamedNodesUsingMe = _sw.GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#HasAttribute").ToList();
            foreach (var unnnamedNode in unnamedNodesUsingMe)
            {
                var name = _sw.GetNameForUnnamedNode(unnnamedNode);
                foreach (var s in AddUsages(unnnamedNode, name))
                {
                    if (!usages.Contains(s))
                        usages.Add(s);
                }
            }
            return usages;
        }

        /// <summary>
        /// Возвращает имя вершины и имена всех её потомков
        /// </summary>
        /// <param name="unnamedNodeToSearch">Вершина, куда попадаем по дуге WORD из SYSTEM</param>
        /// <param name="onlyClasses"> </param>
        /// <returns></returns>
        public TreeViewItem AddInstances(Node unnamedNodeToSearch, bool onlyClasses)
        {
            var instance = new TreeViewItem {Header = _sw.GetNameForUnnamedNode(unnamedNodeToSearch)};
            var children = _sw.GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_a");
            if (!onlyClasses)
                children = children.Union(_sw.GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_instance")).ToList();
            foreach (var child in children)
            {
                instance.Items.Add(AddInstances(child, onlyClasses));
            }
            return instance;
        }
        
        /// <summary>
        /// Получает дерево всех потомков метаобъекта (с экземплярами)
        /// </summary>
        /// <param name="name"> имя, например, Ингредиент</param>
        /// <returns></returns>
        public TreeViewItem AddInstancesOfMetaObject(string name)
        {
            var res = new TreeViewItem();
            List<Node> instances = _sw.GetAllAttr(_sw.Atom("#System"), name);
            res.Header = name;
            foreach (var instance in instances)
            {
                res.Items.Add(AddInstances(instance, false));
            }
            return res;
        }

        /// <summary>
        /// Получает дерево всех потомков метаобъекта (без экземпляров)
        /// </summary>
        /// <param name="name"> имя, например, Ингредиент</param>
        /// <returns></returns>
        public TreeViewItem AddSubClassesOfMetaObject(string name)
        {
            var res = new TreeViewItem();
            List<Node> instances = _sw.GetAllAttr(_sw.Atom("#System"), name);
            res.Header = name;
            foreach (var instance in instances)
            {
                res.Items.Add(AddInstances(instance, true));
            }
            return res;
        }

        /// <summary>
        /// Поиск по метазнаниям
        /// </summary>
        /// <param name="unnamedNodeToSearch"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public MetaResult SearchMetaData(Node unnamedNodeToSearch, string word)
        {
            var metaResult = new MetaResult
                                 {
                                     Name = word,
                                     //Добавляем атрибуты
                                     Attributes = AddAttribute(unnamedNodeToSearch, word),
                                     //Добавляем понятия, где используется word
                                     Usages = AddUsages(unnamedNodeToSearch, word),
                                     //Добавляем экземпляры
                                     //Находим все дуги с именем WORD, выходящие из вершины SYSTEM
                                     Instances = AddInstancesOfMetaObject(word)
                                 };
            return metaResult;
        } 
        #endregion

        private ObjectResult SearchObjectData(Node unnamedNodeToSearch, string word, string oldestArcName)
        {
            var objectResult = new ObjectResult {Name = word, Type = oldestArcName};
            //список атрибутов из метазаний
            MetaResult metaInf = SearchMetaData(_sw.GetUnnamedNodeForName(oldestArcName), oldestArcName);
            objectResult.InfFromMetadata.Header = metaInf.Attributes.Header + " " + word;
            List<string> attrNames = (from TreeViewItem aa 
                                      in metaInf.Attributes.Items.SourceCollection 
                                      select aa.Header.ToString()).ToList();
            if (attrNames.Contains("#FileName"))
            {
                try
                {
                    var sr = new StreamReader(_sw.GetNameForUnnamedNode(_sw.GetAttr(unnamedNodeToSearch.ID, "#FileName")));
                    while (!sr.EndOfStream)
                    {
                        objectResult.InfFromMetadata.Items.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                finally
                {
                    
                }
            }
            //список всех родиельских классов
            objectResult.WayToParent = FindParents(unnamedNodeToSearch);
            //если то, что мы ищем, является классом и имеет подклассы и экземпляры, то находим их все
            if (_sw.GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_a").Any() ||
                _sw.GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_instance").Any())
            { 
                var classResult = new ClassResult(objectResult) {Instances = AddInstances(unnamedNodeToSearch, false)};
                return classResult;
            }

            //в противном случае возвращаем то, что уже нашли
            return objectResult;
        }

        /// <summary>
        /// Ищет родителей (по дугам is_a и is_instance) для данной вершины и возвращает имена родителей
        /// </summary>
        /// <param name="unnamedNodeToSearch"></param>
        /// <returns></returns>
        private List<string> FindParents(Node unnamedNodeToSearch)
        {
            var parents = new List<string>();
            var tmpNode = unnamedNodeToSearch;
            if (_sw.ArcExists(tmpNode.ID, "#is_instance"))
            {
                parents.Add(_sw.GetNameForUnnamedNode(tmpNode));
                tmpNode = _sw.GetAttr(tmpNode.ID, "#is_instance");
            }
            do
            {
                parents.Add(_sw.GetNameForUnnamedNode(tmpNode));
                tmpNode = _sw.GetAttr(tmpNode.ID, "#is_a");
            } while (tmpNode != null && _sw.ArcExists(tmpNode.ID, "#is_a"));
            return parents;
        }
        #endregion
    }
}
