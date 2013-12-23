using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Kernel;

namespace Consulting
{
    public static class MetadataSearch
    {
        public static IEnumerable<string> GetAttrList(Node unnamedNodeToSearch, string word)
        {
            var attributes = new List<string> { word };
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
        public static TreeViewItem AddAttribute(Node unnamedNodeToSearch, string word)
        {
            var treeNode = new TreeViewItem { Header = word };
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
        public static IEnumerable<string> AddUsages(Node unnamedNodeToSearch, string word)
        {
            var usages = new List<string> { word };
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
        public static TreeViewItem AddInstances(Node unnamedNodeToSearch, bool onlyClasses)
        {
            var name = SemanticWeb.Web().GetNameForUnnamedNode(unnamedNodeToSearch, false);
            if (string.IsNullOrEmpty(name))
                return null;
            var instance = new TreeViewItem { Header = SemanticWeb.Web().GetNameForUnnamedNode(unnamedNodeToSearch, false) };
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

        public static List<string> ToList(TreeViewItem twi)
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

        public static List<string> ToListWithHeader(TreeViewItem twi)
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
        public static TreeViewItem AddInstancesOfMetaObject(string name)
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
        public static TreeViewItem AddSubClassesOfMetaObject(string name)
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
        /// <param name="executeSimilarQuery"></param>
        /// <returns></returns>
        public static MetaResult SearchMetaData(Node unnamedNodeToSearch, string word , Action<string> executeSimilarQuery)
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
                ExecuteSimilarQuery = executeSimilarQuery
            };
            return metaResult;
        }
    }
}
