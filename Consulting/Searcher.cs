using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Тип слова, например: wordTypes["Коктейль"] = "#MetaObjects", wordTypes["Сок"] = "Ингредиент"
        /// </summary>
        private readonly Dictionary<string, string> _wordTypes;

        #endregion

        #region Инициализация

        public Searcher(string input, Action<string> executeSimilarQuery, bool searchSynonyms, bool searchClasses)
        {
            WorkMemory = new WorkMemory();
            _executeSimilarQuery = executeSimilarQuery;
            _searchSynonyms = searchSynonyms;
            _searchClasses = searchClasses;
            _input = input;
            var cw = new ConceptWorker(5, _searchSynonyms);
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

            _wordTypes = new Dictionary<string, string>();
        }

        public Searcher(Action<string> executeSimilarQuery, bool searchSynonyms, bool searchClasses)
        {
            _executeSimilarQuery = executeSimilarQuery;
            _searchSynonyms = searchSynonyms;
            _searchClasses = searchClasses;
            WorkMemory = new WorkMemory();
            _wordTypes = new Dictionary<string, string>();
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
                .Select(node => SemanticWeb.Web().GetNodesDirectedToMe(node.ID, "#Name")
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
            var queryResult = new QueryResult {Query = _input, ExecuteSimilarQuery = _executeSimilarQuery};
            if (_nodesInQuery.Count == 0)
                //если в сем сети нет ни одной вершины, имя которой совпадает со словом из запроса
                queryResult.Message = "Поиск не дал результатов.";
            else
            {
                foreach (var node in _unnamedNodesInQuery)
                {
                    var name = SemanticWeb.Web().GetNameForUnnamedNode(node, true);
                    if (_wordTypes.Keys.All(x => x != name))
                    _wordTypes.Add(name,
                                   SemanticWeb.Web().OldestParentArc(node.ID));
                    WorkMemory.WorkedNodes.AddRange(SemanticWeb.Web().WayToSystemNodes);
                    WorkMemory.WorkedArcs.AddRange(SemanticWeb.Web().WayToSystemArcs);
                }
                //Тип запроса 1 (MainObjWithAttrs):
                //MainObject + ConcreteMainObject.Attribute { + ConcreteMainObject.Attribute }
                /*Например: 
                 * Коктейль с соком
                 * Коктейль, где используется Бокал
                 * Коктейль с соком в бокале
                 * и т.д. */

                #region Тип 1

                if (IsMainObjWithAttrs())
                {
                    var attrsType1 = GetMainObjAttrNamesType1().ToList();
                    var parents = new List<string>();
                    if (attrsType1.Count > 0)
                    {
                        queryResult.JointResult = MainObjWithAttrsExecute(GetMainObjNameType1(), attrsType1[0]);
                    }
                    for (int i = 1; i < attrsType1.Count; i++)
                    {
                        queryResult.JointResult = queryResult.JointResult
                            .Intersect(MainObjWithAttrsExecute(GetMainObjNameType1(), attrsType1[i])).ToList();
                    }
                    if (_searchClasses)
                    {
                        foreach (string attr in attrsType1)
                        {
                            parents.AddRange(
                                FindParents(SemanticWeb.Web().GetUnnamedNodeForName(attr)).Where(x => x != attr));
                        }
                        queryResult.GeneralResult =
                            parents.Select(x => GetMainObjNameType1() + ", где используется " + x).ToList();
                    }
                }
                    #endregion

                    //Тип запроса 2 (ConcreteMainObject + ConcreteMainObject.Attribute)
                    /* Например: есть ли в French75 вишня?
                 * используется ли в WinterChill бокал?
                 */

                    #region Тип 2

                else if (IsConcreteMainObjWithAttr())
                {
                    queryResult.Message = Type2Execute(GetMainObjNameType2(), GetMainObjAttrNamesType2()) ? "Да" : "Нет";
                }
                    #endregion

                    //Для всех остальных запросов просто ищем каждое слово по отдельности
                else
                {
                    for (int i = 0; i < _unnamedNodesInQuery.Count; i++)
                    {
                        //ищем каждое слово по отдельности
                        var wordRes = SearchOneWord(_nodesInQuery[i], _unnamedNodesInQuery[i], _nodesInQuery[i].Name);
                        if (wordRes != null)
                            queryResult.EveryWordResult.Add(wordRes);
                    }
                }
            }
            return queryResult;
        }

        #region Определение типа запроса

        private bool IsMainObjWithAttrs()
        {
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames(); //список всех mainObj системы
            bool queryContainsMainObject = _nodesInQuery.Count(x => mainObjs.Contains(x.Name)) == 1;
            //правда ли, что в запросе есть 1 mainObj?
            if (!queryContainsMainObject) return false; //если нет, то печаль
            string name = _nodesInQuery.Single(x => mainObjs.Contains(x.Name)).Name; //имя этого mainObj
            var mainObjAttrs = GetAttrList(SemanticWeb.Web().GetUnnamedNodeForName(name), name);
            //список атрибутов mainObj
            int attrCount = mainObjAttrs.Count(x => _nodesInQuery.Any(t => _wordTypes[t.Name] == x));
            //количество этих атрибутов в запросе
            return attrCount > 0; //если их больше 0, то успех
        }

        private string GetMainObjNameType1()
        {
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames();
            return _nodesInQuery.Single(x => mainObjs.Contains(x.Name)).Name;
        }

        private IEnumerable<string> GetMainObjAttrNamesType1()
        {
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames();
            string name = _nodesInQuery.Single(x => mainObjs.Contains(x.Name)).Name;
            var mainObjAttrs = GetAttrList(SemanticWeb.Web().GetUnnamedNodeForName(name), name);
            return _nodesInQuery.Where(x => mainObjAttrs.Contains(_wordTypes[x.Name])).Select(x => x.Name);
        }

        private bool IsConcreteMainObjWithAttr()
        {
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames();
            var mainObjNames = new List<string>();
            foreach (var mainObj in mainObjs)
            {
                mainObjNames.AddRange(SemanticWeb.Web().GetAllAttr(
                    SemanticWeb.Web().Mota(SemanticWeb.Web().Atom("#System")).ID,
                    mainObj).Select(x => SemanticWeb.Web().GetNameForUnnamedNode(x, false)).ToList());
            }

            bool queryContainsMainObject = _nodesInQuery.Count(x => mainObjNames.Contains(x.Name)) == 1;
            if (!queryContainsMainObject) return false;

            string name = SemanticWeb.Web().OldestParentArc(SemanticWeb.Web().GetUnnamedNodeForName(
                _nodesInQuery.Single(x => mainObjNames.Contains(x.Name)).Name).ID);
            var mainObjAttrs = GetAttrList(SemanticWeb.Web().GetUnnamedNodeForName(name), name);


            int attrCount = mainObjAttrs.Count(x => _nodesInQuery.Any(t => _wordTypes[t.Name] == x));
            return attrCount == 2;
        }

        private string GetMainObjNameType2()
        {
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames(); //Например, "Коктейль"
            return _nodesInQuery.Single(x => mainObjs.Contains(_wordTypes[x.Name])).Name;
        }

        private string GetMainObjAttrNamesType2()
        {
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames();
            string name = _nodesInQuery.Single(x => mainObjs.Contains(_wordTypes[x.Name])).Name;
            string metaName = _wordTypes[name];
            var mainObjAttrs = GetAttrList(SemanticWeb.Web().GetUnnamedNodeForName(metaName), metaName);
            return _nodesInQuery.Single(x => mainObjAttrs.Contains(_wordTypes[x.Name]) && x.Name != name).Name;
        }

        private bool IsAttrsForMetaobjectType3()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Запрос 1-го типа

        //MainObject + MainObject.AttributeValue { + MainObject.AttributeValue }
        /*Например: 
         * Коктейль с соком
         * Коктейль, где используется Бокал
         * Коктейль с соком в бокале
         * и т.д. */

        /// <summary>
        /// Выполняет запрос типа MainObject + MainObject.AttributeValue { + MainObject.AttributeValue }
        /// </summary>
        /// <param name="mainObj">Имя MainObject, например, Коктейль</param>
        /// <param name="attrValue">Значение атрибута, например, Бокал</param>
        /// <returns>Например, список коктейлей в бокале</returns>
        private List<string> MainObjWithAttrsExecute(string mainObj, string attrValue)
        {
            //коктейли в V-образном бокале для мартини
            var attrNode = SemanticWeb.Web().GetUnnamedNodeForName(attrValue);
            var children = ToListWithHeader(AddInstances(attrNode, false));
            var res = new List<string>();
            foreach (var child in children)
            {
                res.AddRange(Type1OneWord(mainObj, child));
            }
            return res.Distinct().ToList();
        }

        private IEnumerable<string> Type1OneWord(string mainObj, string attrValue)
        {
            //находим все экземпляры MainObject
            List<string> instancesNames = ToList(AddInstancesOfMetaObject(mainObj));
            var instances = instancesNames.Select(x => SemanticWeb.Web().GetUnnamedNodeForName(x));
            var attrs = ToListWithHeader(AddAttribute(SemanticWeb.Web().GetUnnamedNodeForName(mainObj), mainObj));
            var attrNode = SemanticWeb.Web().GetUnnamedNodeForName(attrValue);
            var list = new List<string>();
            foreach (Node instanceNode in instances)
            {
                string res = GetAttrNameIfExists(instanceNode, attrNode, attrs);
                if (res != null)
                {
                    if (res == attrValue) list.Add(SemanticWeb.Web().GetNameForUnnamedNode(instanceNode, false));
                }
            }
            return list;
        }

        /// <summary>
        /// Проверяет, соединены ли 2 вершины дугами, имена которых являются атрибутами mainNode
        /// </summary>
        /// <param name="mainNode"></param>
        /// <param name="attrNode"></param>
        /// <param name="attrs"> </param>
        /// <returns>Если вершины соединены, возвращает имя атрибута (attrNode -> Name), иначе - null</returns>
        private string GetAttrNameIfExists(Node mainNode, Node attrNode, List<string> attrs)
        {
            if (!Reached(mainNode, attrs).Contains(attrNode))
                return null;


            return SemanticWeb.Web().GetNameForUnnamedNode(attrNode, true);
        }

        private List<Node> Reached(Node fromNode, List<string> arcNames)
        {
            var res = new List<Node> {fromNode};
            var reachedNodes = new List<Node>();
            foreach (var arcName in arcNames)
            {
                reachedNodes.AddRange(SemanticWeb.Web().GetAllAttr(fromNode.ID, arcName));
            }
            var newNodes = reachedNodes.Where(x => !res.Contains(x));
            foreach (var newNode in newNodes)
            {
                var listx = Reached(newNode, arcNames);
                foreach (var node in listx.Where(x => !res.Contains(x)))
                {
                    res.Add(node);
                }
            }
            return res;
        }

        #endregion

        #region Запрос 2-го типа

        //Тип запроса 2 (ConcreteMainObject + ConcreteMainObject.Attribute)
        /* Например: есть ли в French75 вишня?
         * используется ли в WinterChill бокал?
         */

        private bool Type2Execute(string mainName, string attrValue)
        {
            var metaName = _wordTypes[mainName]; //тип mainName
            //список атрибутов для metaName
            var attrs = ToListWithHeader(AddAttribute(SemanticWeb.Web().GetUnnamedNodeForName(metaName), metaName));
            var attrNode = SemanticWeb.Web().GetUnnamedNodeForName(attrValue);
            var instanceNode = SemanticWeb.Web().GetUnnamedNodeForName(mainName);

            var childrenNames = ToListWithHeader(AddInstances(attrNode, false));

            List<Node> children = childrenNames
                .Select(x => SemanticWeb.Web().GetUnnamedNodeForName(x)).
                ToList();

            string res = null;
            //for (int i = 0; i < children.Count && res == null; i++)
            //{
            res = GetAttrNameIfExists(instanceNode, attrNode, attrs);
            //}

            if (res != null)
            {
                if (childrenNames.Contains(res)) return true;
            }
            return false;
        }

        #endregion

        #region Запрос 3-го типа

        //MainObjectValue + MainObject.Attribute { + MainObject.Attribute }
        /*Например: 
         * Ингредиенты WinterChill
         * Действия для приготовления коктейля French75
         * и т.д. */

        #endregion

        #region Для одного слова

        /// <summary>
        /// Поиск одного слова
        /// </summary>
        /// <param name="namedNodeToSearch">Именованная вершина, соответствующая слову из запроса</param>
        /// <param name="unnamedNodeToSearch">Неименованная вершина, соответствующая слову из запроса</param>
        /// <param name="word">Слово, которое ищем</param>
        /// <returns></returns>
        public WordResult SearchOneWord(Node namedNodeToSearch, Node unnamedNodeToSearch, string word)
        {
            //определяем имя дуги, которой самый старший предок связан с #System
            WorkMemory.WorkedNodes.Add(namedNodeToSearch);
            WorkMemory.WorkedNodes.AddRange(SemanticWeb.Web().WayToSystemNodes);
            //Для метазнаний выполняем поиск по метазнаниям
            if (_wordTypes[word] == "#MetaObjects")
                return SearchMetaData(unnamedNodeToSearch, word);
            //Если самая верхняя дуга имеет имя из метаобъектов, ищем слово из предметной области
            if (SemanticWeb.Web().GetAllMetaObjectNames().Contains(_wordTypes[word]))
                return SearchObjectData(unnamedNodeToSearch, word, _wordTypes[word]);
            //throw new ArgumentException(SemanticWeb.ErrMsg + " Слово " + word + " не нашлось");
            return null;
        }

        #region Поиск по метазнаниям

        private IEnumerable<string> GetAttrList(Node unnamedNodeToSearch, string word)
        {
            var attributes = new List<string> {word};
            var unnamedNodesIUse = SemanticWeb.Web().GetAllAttr(unnamedNodeToSearch.ID, "#HasAttribute").ToList();
            foreach (var unnnamedNode in unnamedNodesIUse)
            {
                var name = SemanticWeb.Web().GetNameForUnnamedNode(unnnamedNode, false);
                foreach (var s in GetAttrList(unnnamedNode, name))
                {
                    if (!attributes.Contains(s))
                        attributes.Add(s);
                }
            }
            return attributes;
        }

        /// <summary>
        /// Возвращает имена вершин, куда можно попасть из unnamedNodeToSearch по дугам HasAttribute
        /// </summary>
        /// <param name="unnamedNodeToSearch">Неименованная вершина, соответствующая слову из запроса</param>
        /// <param name="word">Слово, которое ищем</param>
        /// <returns></returns>
        private TreeViewItem AddAttribute(Node unnamedNodeToSearch, string word)
        {
            var treeNode = new TreeViewItem {Header = word};
            var attributes = SemanticWeb.Web().GetAllAttr(unnamedNodeToSearch.ID, "#HasAttribute");
            foreach (Node attributeUnnamed in attributes)
            {
                string name = SemanticWeb.Web().GetNameForUnnamedNode(attributeUnnamed, false);
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
        private IEnumerable<string> AddUsages(Node unnamedNodeToSearch, string word)
        {
            var usages = new List<string> {word};
            var unnamedNodesUsingMe =
                SemanticWeb.Web().GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#HasAttribute").ToList();
            foreach (var unnnamedNode in unnamedNodesUsingMe)
            {
                var name = SemanticWeb.Web().GetNameForUnnamedNode(unnnamedNode, false);
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
            var name = SemanticWeb.Web().GetNameForUnnamedNode(unnamedNodeToSearch, false);
            if (string.IsNullOrEmpty(name))
                return null;
            var instance = new TreeViewItem
                               {Header = SemanticWeb.Web().GetNameForUnnamedNode(unnamedNodeToSearch, false)};
            var children = SemanticWeb.Web().GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_a");
            if (!onlyClasses)
                children =
                    children.Union(SemanticWeb.Web().GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_instance")).
                        ToList();
            foreach (var child in children)
            {
                var newInstance = AddInstances(child, onlyClasses);
                if (newInstance != null)
                    instance.Items.Add(newInstance);
            }
            return instance;
        }

        private List<string> ToList(TreeViewItem twi)
        {
            var res = new List<string>();
            if (twi.Items.Count == 0)
                res.Add(twi.Header.ToString());
            else
            {
                foreach (TreeViewItem item in twi.Items)
                {
                    res.AddRange(ToList(item));
                }
            }
            return res;
        }

        private List<string> ToListWithHeader(TreeViewItem twi)
        {
            var res = new List<string>();
            if (twi.Items.Count == 0)
                res.Add(twi.Header.ToString());
            else
            {
                foreach (TreeViewItem item in twi.Items)
                {
                    if (!res.Contains(twi.Header.ToString()))
                        res.Add(twi.Header.ToString());
                    foreach (var xx in ToListWithHeader(item))
                    {
                        if (!res.Contains(xx))
                            res.Add(xx);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Получает дерево всех потомков метаобъекта (с экземплярами)
        /// </summary>
        /// <param name="name"> имя, например, Ингредиент</param>
        /// <returns></returns>
        public TreeViewItem AddInstancesOfMetaObject(string name)
        {
            var res = new TreeViewItem();
            List<Node> instances = SemanticWeb.Web().GetAllAttr(SemanticWeb.Web().Atom("#System"), name);
            res.Header = name;
            foreach (var instance in instances)
            {
                var newInstance = AddInstances(instance, false);
                if (newInstance != null) res.Items.Add(AddInstances(instance, false));
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
            List<Node> instances = SemanticWeb.Web().GetAllAttr(SemanticWeb.Web().Atom("#System"), name);
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
                                     Usages = AddUsages(unnamedNodeToSearch, word).Where(x => x != word).ToList(),
                                     //Добавляем экземпляры
                                     //Находим все дуги с именем WORD, выходящие из вершины SYSTEM
                                     Instances = AddInstancesOfMetaObject(word),
                                     ExecuteSimilarQuery = _executeSimilarQuery
                                 };
            return metaResult;
        }

        #endregion

        #region Поиск по объектам

        private TreeViewItem MetadataInf(Node unnamedNodeToSearch, string word, string type, TreeViewItem attributes)
        {
            var treeNode = new TreeViewItem {Header = type + " " + word}; //type word (Коктейль Bellini) 

            foreach (TreeViewItem attribute in attributes.Items) //ингредиент, инструмент, ёмкость
            {
                List<Node> attrValues = SemanticWeb.Web().GetAllAttr(unnamedNodeToSearch.ID, attribute.Header.ToString());
                //жидкость
                foreach (Node attrValue in attrValues) //жидкость
                {
                    string name = SemanticWeb.Web().GetNameForUnnamedNode(attrValue, true);
                    treeNode.Items.Add(MetadataInf(attrValue, name, SemanticWeb.Web().OldestParentArc(attrValue.ID),
                                                   attribute));
                }
            }
            return treeNode;
        }

        private ObjectResult SearchObjectData(Node unnamedNodeToSearch, string word, string oldestArcName)
        {
            var objectResult = new ObjectResult
                                   {Name = word, Type = oldestArcName, ExecuteSimilarQuery = _executeSimilarQuery};

            //список атрибутов из метазаний
            MetaResult metaInf = SearchMetaData(SemanticWeb.Web().GetUnnamedNodeForName(oldestArcName), oldestArcName);
            objectResult.InfFromMetadata.Header = metaInf.Attributes.Header + " " + word;
            List<string> attrNames = (from TreeViewItem aa
                                          in metaInf.Attributes.Items.SourceCollection
                                      select aa.Header.ToString()).ToList();

            //WayToParent
            objectResult.WayToParent = FindParents(unnamedNodeToSearch);

            //InfFromFile
            if (attrNames.Contains("Файл"))
            {
                try
                {
                    var sr =
                        new StreamReader(
                            SemanticWeb.Web().GetNameForUnnamedNode(
                                SemanticWeb.Web().GetAttr(unnamedNodeToSearch.ID, "Файл"), false));
                    while (!sr.EndOfStream)
                    {
                        objectResult.InfFromFile.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch (Exception)
                {
                    objectResult.InfFromFile.Add("Файл с рецептом не найден. Вам придётся довольствоваться лишь знаниями из сети :(");
                }
            }

            //InfFromMetadata
            objectResult.InfFromMetadata = MetadataInf(unnamedNodeToSearch, word, oldestArcName, metaInf.Attributes);

            //SimilarQueries
            objectResult.SimilarQueries = SemanticWeb.Web().GetMainMetaObjectNames()
                .Where(x => x != oldestArcName)
                .Select(mainObj => mainObj + ", где используется " + word).ToList();

            //RelativeQueries
            //objectResult.RelativeQueries = FindRelations(unnamedNodeToSearch);

            //если то, что мы ищем, является классом и имеет подклассы и экземпляры, то находим их все
            if (SemanticWeb.Web().GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_a").Any() ||
                SemanticWeb.Web().GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_instance").Any())
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
            if (SemanticWeb.Web().ArcExists(tmpNode.ID, "#is_instance"))
            {
                parents.Add(SemanticWeb.Web().GetNameForUnnamedNode(tmpNode, false));
                tmpNode = SemanticWeb.Web().GetAttr(tmpNode.ID, "#is_instance");
            }
            do
            {
                parents.Add(SemanticWeb.Web().GetNameForUnnamedNode(tmpNode, false));
                tmpNode = SemanticWeb.Web().GetAttr(tmpNode.ID, "#is_a");
            } while (tmpNode != null && SemanticWeb.Web().ArcExists(tmpNode.ID, "#is_a"));
            if (tmpNode != null) parents.Add(SemanticWeb.Web().GetNameForUnnamedNode(tmpNode, false));
            return parents;
        }

        #endregion

        #endregion

        #endregion
    }

}
