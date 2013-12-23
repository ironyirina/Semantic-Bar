using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Kernel;

namespace Consulting
{
    static class ObjectSearcher
    {
        public static List<Node> GetAttrValues(Node unnamedNodeToSearch, string arcName)
        {
            var attrValues = SemanticWeb.Web().GetAllAttr(unnamedNodeToSearch.ID, arcName);
            if (attrValues.Count == 0)
            {
                var parents = FindParentNodes(unnamedNodeToSearch);
                if (parents.Count > 1)
                {
                    return GetAttrValues(parents[1], arcName);
                }
            }
            return attrValues;
        }

        public static TreeViewItem MetadataInf(Node unnamedNodeToSearch, string word, string type, TreeViewItem attributes)
        {
            var treeNode = new TreeViewItem { Header = type + " " + word }; //type word (Коктейль Bellini) 

            foreach (TreeViewItem attribute in attributes.Items) //ингредиент, инструмент, ёмкость
            {
                List<Node> attrValues = GetAttrValues(unnamedNodeToSearch, attribute.Header.ToString());
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

        public static ObjectResult SearchObjectData(Node unnamedNodeToSearch, string word, string oldestArcName,
            Action<string> executeSimilarQuery)
        {
            var objectResult = new ObjectResult
            {
                Name = word,
                Type = oldestArcName,
                ExecuteSimilarQuery = executeSimilarQuery
            };

            //список атрибутов из метазаний
            MetaResult metaInf = MetadataSearch.SearchMetaData(SemanticWeb.Web().GetUnnamedNodesForName(oldestArcName),
                oldestArcName, executeSimilarQuery);
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
                    objectResult.InfFromFile.Add(
                        "Файл с рецептом не найден. Вам придётся довольствоваться лишь знаниями из сети :(");
                }
            }

            //InfFromMetadata
            objectResult.InfFromMetadata = MetadataInf(unnamedNodeToSearch, word, oldestArcName, metaInf.Attributes);
            foreach (var parent in FindParentNodes(unnamedNodeToSearch))
            {
                var parentMetadata = MetadataInf(parent, word, oldestArcName, metaInf.Attributes);
            }

            //SimilarQueries
            objectResult.SimilarQueries = SemanticWeb.Web().GetMainMetaObjectNames()
                .Where(x => x != oldestArcName)
                .Select(mainObj => mainObj + ", где используется " + word).ToList();

            //если то, что мы ищем, является классом и имеет подклассы и экземпляры, то находим их все
            if (SemanticWeb.Web().GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_a").Any() ||
                SemanticWeb.Web().GetNodesDirectedToMe(unnamedNodeToSearch.ID, "#is_instance").Any())
            {
                var classResult = new ClassResult(objectResult) {Instances = MetadataSearch.AddInstances(unnamedNodeToSearch, false)};
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
        public static List<string> FindParents(Node unnamedNodeToSearch)
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

        public static List<Node> FindParentNodes(Node unnamedNodeToSearch)
        {
            var parents = new List<Node>();
            var tmpNode = unnamedNodeToSearch;
            if (SemanticWeb.Web().ArcExists(tmpNode.ID, "#is_instance"))
            {
                parents.Add(tmpNode);
                tmpNode = SemanticWeb.Web().GetAttr(tmpNode.ID, "#is_instance");
            }
            do
            {
                parents.Add(tmpNode);
                tmpNode = SemanticWeb.Web().GetAttr(tmpNode.ID, "#is_a");
            } while (tmpNode != null && SemanticWeb.Web().ArcExists(tmpNode.ID, "#is_a"));
            if (tmpNode != null) parents.Add(tmpNode);
            return parents;
        }
    }
}
