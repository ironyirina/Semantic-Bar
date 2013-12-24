using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Kernel;

namespace Consulting
{
    public class UnnamedSearcher
    {
        private readonly string _input;
        private readonly Action<string> _executeSimilarQuery;
        private readonly WorkMemory _workMemory;
        private readonly bool _searchClasses;
        //private readonly List<Node> _unnamedNodesInQuery;

        private readonly Dictionary<string, Node> _query; 

        public UnnamedSearcher(string input, Action<string> executeSimilarQuery, WorkMemory workMemory,
            bool searchClasses, IEnumerable<Node> unnamedNodesInQuery)
        {
            _input = input;
            _executeSimilarQuery = executeSimilarQuery;
            _workMemory = workMemory;
            _searchClasses = searchClasses;
            var dictionary = new Dictionary<string, Node>();
            foreach (Node node in unnamedNodesInQuery)
                if (!dictionary.ContainsKey(SemanticWeb.Web().GetNameForUnnamedNode(node, false)))
                dictionary.Add(SemanticWeb.Web().GetNameForUnnamedNode(node, false), node);
            _query = dictionary;
        }

        private Node SafelyGetNodeByName(string name, string desiredContext)
        {
            if (_query.ContainsKey(name))
                return _query[name];

            var nodes = SemanticWeb.Web().GetAllUnnamedNodesForName(name);
            if (nodes.Count == 1)
                return nodes[0];
            if (desiredContext == null)
                throw new ArgumentException("What a fuck?!");
            var res = nodes.FirstOrDefault(x => SemanticWeb.Web().GetClosestParentName(x) == desiredContext);
            if (res != null)
                return res;
            throw new ArgumentException("What a fuck?!");

        }

        public QueryResult Search()
        {
            var queryResult = new QueryResult { Query = _input, ExecuteSimilarQuery = _executeSimilarQuery };
            if (_query.Count == 0)
                //если в сем сети нет ни одной вершины, имя которой совпадает со словом из запроса
                queryResult.Message = "Поиск не дал результатов.";
            else
            {
                foreach (var node in _query.Values)
                {
                    var name = SemanticWeb.Web().GetNameForUnnamedNode(node, true);
                    var arc = SemanticWeb.Web().OldestParentArc(node.ID);
                    _workMemory.WorkedNodes.AddRange(SemanticWeb.Web().WayToSystemNodes);
                    _workMemory.WorkedArcs.AddRange(SemanticWeb.Web().WayToSystemArcs);
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
                    //Комменты будут на конкретном примере, иначе нифига не понятно
                    //Пример: "Коктейль с вишенкой"
                    //Ищем список всех атрибутов Главного Метаобъекта, перечисленных в запросе. В данном случае - {"Вишня"}
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
                    if (queryResult.JointResult.Count == 0)
                    {
                        queryResult.Message = "Поиск не дал результатов";
                    }
                    if (_searchClasses)
                    {
                        foreach (var attr in attrsType1)
                        {
                            parents.AddRange(
                                ObjectSearcher.FindParents(attr).Where(x => x != attr.Name));
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
                    foreach (Node node in _query.Values)
                    {
                        //ищем каждое слово по отдельности
                        var wordRes = OneWordSearcher.SearchOneWord(node, _executeSimilarQuery, _workMemory);
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
            if (_query.Count == 1) return false;
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames(); //список всех mainObj системы
            bool queryContainsMainObject = _query.Keys.Count(mainObjs.Contains) == 1;
            //правда ли, что в запросе есть 1 mainObj?
            if (!queryContainsMainObject) return false; //если нет, то печаль
            string name = _query.Keys.Single(mainObjs.Contains); //имя этого mainObj
            IEnumerable<string> mainObjAttrs = MetadataSearch.GetAttrList(SafelyGetNodeByName(name, "#MetaObject"), name);
            //список атрибутов mainObj
            int attrCount = 0;
            for (int i = 0; i < _query.Count; i++)
            {
                Node tmpNode = _query.ElementAt(i).Value;
                var objAttrs = mainObjAttrs as IList<string> ?? mainObjAttrs.ToList();
                if (objAttrs.Contains(SemanticWeb.Web().OldestParentArc(tmpNode.ID)))
                {
                    attrCount++;
                }
            }
            //mainObjAttrs.Count(x => _nodesInQuery.Any(t => _wordTypes[t.Name] == x));
            //количество этих атрибутов в запросе
            return attrCount == _query.Count - 1; //если их больше 0, то успех
        }

        private string GetMainObjNameType1()
        {
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames();
            return _query.Keys.Single(mainObjs.Contains);
        }

        private IEnumerable<Node> GetMainObjAttrNamesType1()
        {
            // Коктейль
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames();
            // Коктейль
            string name = _query.Keys.Single(mainObjs.Contains);
            // Ингредиент
            Node n = SafelyGetNodeByName(name, null);
            IEnumerable<string> mainObjAttrs = MetadataSearch.GetAttrList(n, name);

            return _query.Values.Where((t, i) => mainObjAttrs.Contains(SemanticWeb.Web().OldestParentArc(_query.Values.ElementAt(i).ID))).ToList();
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

            bool queryContainsMainObject = _query.Keys.Count(mainObjNames.Contains) == 1;
            if (!queryContainsMainObject) return false;

            string name = SemanticWeb.Web().OldestParentArc(SafelyGetNodeByName(_query.Keys.Single(mainObjNames.Contains), null).ID);
            var mainObjAttrs = MetadataSearch.GetAttrList(SafelyGetNodeByName(name, "#MetaObjects"), name);

            int attrCount = 0;
            foreach (string x in mainObjAttrs)
            {
                for (int i = 0; i < _query.Count; i++)
                {
                    if (SemanticWeb.Web().OldestParentArc(_query.ElementAt(i).Value.ID) == x)
                    {
                        attrCount++;
                        break;
                    }
                }
            }
            return attrCount >= 2;
        }

        private string GetMainObjNameType2()
        {
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames(); //Например, "Коктейль"
            string res = string.Empty;

            for (int i = 0; i < _query.Count && res == string.Empty; i++)
            {
                if (mainObjs.Contains(SemanticWeb.Web().OldestParentArc(_query.ElementAt(i).Value.ID)))
                    res = _query.ElementAt(i).Key;
            }
            return res;
        }

        private IEnumerable<Node> GetMainObjAttrNamesType2()
        {
            List<string> mainObjs = SemanticWeb.Web().GetMainMetaObjectNames();

            string name = string.Empty;
            for (int i = 0; i < _query.Count && name == string.Empty; i++)
            {
                if (mainObjs.Contains(SemanticWeb.Web().OldestParentArc(_query.ElementAt(i).Value.ID)))
                {
                    name = _query.ElementAt(i).Key;
                }
            }
            string metaName = SemanticWeb.Web().OldestParentArc(SafelyGetNodeByName(name, null).ID);
            var mainObjAttrs = MetadataSearch.GetAttrList(SafelyGetNodeByName(metaName, null), metaName);
            return
                _query.Values.Where(
                    (t, i) =>
                        mainObjAttrs.Contains(SemanticWeb.Web().OldestParentArc(_query.ElementAt(i).Value.ID)) &&
                        SemanticWeb.Web().GetNameForUnnamedNode(t, false) != name).ToList();
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
        /// <param name="attrNode">Значение атрибута, например, Бокал</param>
        /// <returns>Например, список коктейлей в бокале</returns>
        private List<string> MainObjWithAttrsExecute(string mainObj, Node attrNode)
        {
            //коктейли в V-образном бокале для мартини
            var children = MetadataSearch.ToListWithHeader(MetadataSearch.AddInstances(attrNode, false));
            var res = new List<string>();
            foreach (var child in children)
            {
                res.AddRange(Type1OneWord(mainObj, SafelyGetNodeByName(child, SemanticWeb.Web().GetNameForUnnamedNode(attrNode, false))));
            }
            return res.Distinct().ToList();
        }

        private IEnumerable<string> Type1OneWord(string mainObj, Node attrNode)
        {
            //находим все экземпляры MainObject
            // Список всех коктейлей
            List<string> instancesNames = MetadataSearch.ToList(MetadataSearch.AddInstancesOfMetaObject(mainObj));
            // Список всех неименованных вершин - коктейлей
            var instances = instancesNames.Select(x => SafelyGetNodeByName(x, SemanticWeb.Web().GetMainMetaObjectNames()[0]));
            // Список всех атрибутов метаобъекта (Ингредиент, Ёмкость, Действие...)
            var attrs = MetadataSearch.ToListWithHeader(MetadataSearch.AddAttribute(SafelyGetNodeByName(mainObj, null), mainObj));
            //Вершина, соответствующая экземпляру атрибута, который ищем (например, неименованный узел для Вишни)
            var list = new List<string>();
            foreach (Node instanceNode in instances)
            {
                string res = GetAttrNameIfExists(instanceNode, attrNode, attrs);
                if (res != null)
                {
                    if (res == SemanticWeb.Web().GetNameForUnnamedNode(attrNode, false))
                        list.Add(SemanticWeb.Web().GetNameForUnnamedNode(instanceNode, false));
                    _workMemory.WorkedArcs.AddRange(SemanticWeb.Web().WayToSystemArcs);
                    _workMemory.WorkedNodes.AddRange(SemanticWeb.Web().WayToSystemNodes);
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

            AddWayToWorkMemory(mainNode, attrNode, attrs);
            return SemanticWeb.Web().GetNameForUnnamedNode(attrNode, true);
        }

        private List<Node> Reached(Node fromNode, List<string> arcNames)
        {
            var res = new List<Node> { fromNode };
            var reachedNodes = new List<Node>();
            if (!arcNames.Contains("#is_instance"))
                arcNames.Add("#is_instance");
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

        private IEnumerable<Arc> FindAWay(Node from, Node to, List<string> arcNames)
        {
            //Список вершин, непосредственно достижимых из from
            var oneStep = new List<Node>();
            foreach (var arcName in arcNames)
            {
                oneStep.AddRange(SemanticWeb.Web().GetAllAttr(from.ID, arcName));
            }
            if (oneStep.Contains(to))
                return new List<Arc>(SemanticWeb.Web().GetArcsBetweenNodes(from.ID, to.ID).Where(x => arcNames.Contains(x.Name)));
            return (from node in oneStep
                where Reached(node, arcNames).Contains(to)
                select
                    new List<Arc>(
                        SemanticWeb.Web().GetArcsBetweenNodes(@from.ID, node.ID).Where(x => arcNames.Contains(x.Name)))
                        .Union(FindAWay(node, to, arcNames))).FirstOrDefault();
        }

        /// <summary>
        /// Добавляет в рабочую память те вершины и дуги, которые есть по пути от вершины from к вершине to.
        /// Вершина to обязательно должна быть достижими по дуга arcNames из from
        /// </summary>
        /// <param name="from">Откуда идём</param>
        /// <param name="to">Куда идём</param>
        /// <param name="arcNames">По каким дугам идём</param>
        private void AddWayToWorkMemory(Node from, Node to, List<string> arcNames)
        {
            var reachedNodes = Reached(from, arcNames);
            if (!reachedNodes.Contains(to))
                throw new ArgumentException(string.Format("Вершина {0} не достижима из {1} по arcNames", from, to));
            var way = FindAWay(from, to, arcNames);
            if (way == null)
                return;
            var nodesFromWay =
                way.ToList().Select(x => x.From).Union(way.ToList().Select(x => x.To)).Distinct().Select(x => SemanticWeb.Web().Mota(x));
            _workMemory.WorkedArcs.AddRange(way);
            _workMemory.WorkedNodes.AddRange(nodesFromWay);
        }

        #endregion

        #region Запрос 2-го типа

        //Тип запроса 2 (ConcreteMainObject + ConcreteMainObject.Attribute)
        /* Например: есть ли в French75 вишня?
         * используется ли в WinterChill бокал?
         */

        private bool Type2Execute(string mainName, IEnumerable<Node> attNodes)
        {
            var metaName = SemanticWeb.Web().OldestParentArc(SafelyGetNodeByName(mainName, null).ID); //тип mainName
            //список атрибутов для metaName
            string res = null;
            var childrenNames = new List<string>();
            foreach (var attrNode in attNodes)
            {
                var attrs =
                    MetadataSearch.ToListWithHeader(
                        MetadataSearch.AddAttribute(SafelyGetNodeByName(metaName, null), metaName));
                var instanceNode = SafelyGetNodeByName(mainName, SemanticWeb.Web().GetMainMetaObjectNames()[0]);

                childrenNames = MetadataSearch.ToListWithHeader(MetadataSearch.AddInstances(attrNode, false));

                List<Node> children = childrenNames
                    .Select(x => SafelyGetNodeByName(x, SemanticWeb.Web().GetNameForUnnamedNode(attrNode, false))).
                    ToList();

                res = null;
                for (int i = 0; i < children.Count && res == null; i++)
                {
                    res = GetAttrNameIfExists(instanceNode, attrNode, attrs);
                }
            }

            if (res != null)
            {
                if (childrenNames.Contains(res)) return true;
            }
            return false;
        }

        #endregion
    }
}
