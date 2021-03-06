﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Kernel
{
    [DataContract]
    public class SemanticWeb
    {
        #region Переменные

        [DataMember]
        public List<Node> Nodes { get; private set; }

        [DataMember]
        public List<Arc> Arcs { get; private set; }

        public const string ErrMsg = "Needs to be fixed! ";

        private static SemanticWeb _web;

        public static bool IsChanged;

        public List<Node> WayToSystemNodes { get; private set; }

        public List<Arc> WayToSystemArcs { get; private set; }

        #endregion

        #region Списки служебных дуг и рёбер

        /// <summary>
        /// Дуги, которые могут выходить из любой вершины
        /// </summary>
        public static List<string> SystemArcs
        {
            get
            {
                return new List<string>
                           {
                               "#is_a",
                               "#is_instance",
                               "#Name",
                               "#HasAttribute"
                           };
            }
        }

        /// <summary>
        /// Дуги, которые могут выходить только из вершины System
        /// </summary>
        public static List<string> ArcsOnlyForSystem
        {
            get
            {
                return new List<string>
                           {
                               "#MetaRelations",
                               "#MetaObjects",
                               "#Relations"
                           };
            }
        } 

        public static List<string> AllReservedArcs
        {
            get { return SystemArcs.Union(ArcsOnlyForSystem).ToList(); }
        }

        #endregion

        #region Инициализация

        public static SemanticWeb Web()
        {
            return _web ?? (_web = new SemanticWeb());
        }

        private SemanticWeb()
        {
            Nodes = new List<Node>();
            Arcs = new List<Arc>();
            AddNode("#System");
        }
        #endregion

        #region Atom/Mota
        /// <summary>
        /// По имени возвращает идентификатор вершины (если она есть в сети) или создаёт
        /// новый идентификатор
        /// </summary>
        /// <param name="name">Имя вершины</param>
        /// <returns>Идентификатор вершины</returns>
        public int Atom(string name)
        {
            if (name == "")
                throw new ArgumentException(ErrMsg + "Не ищи неименованную вершину функцией Atom!");
            var existingNode = Nodes.SingleOrDefault(x => x.Name.Trim().ToUpper() == name.Trim().ToUpper());
            if (Nodes.Count == 0) return 1;
            return existingNode != null ? existingNode.ID : Nodes.Select(x => x.ID).Max() + 1;
        }

        /// <summary>
        /// По ID вершины возвращает её имя. Если такого ID нет, то генерирует исключение
        /// </summary>
        /// <param name="nodeID">ID вершины</param>
        /// <returns>Имя вершины</returns>
        public Node Mota(int nodeID)
        {
            var node = Nodes.SingleOrDefault(x => x.ID == nodeID);
            if (node == null)
                throw new ArgumentException(ErrMsg + "Вершины с ID = " + nodeID + "  нет в сем. сети");
            return node;
        } 
        #endregion

        #region Вершины

        #region Полезные вершинофункции
        /// <summary>
        /// Проверяет существование вершины с заданным именем
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public bool NodeExists(string nodeName)
        {
            return Nodes.Any(x => x.Name.Trim().ToUpper() == nodeName.Trim().ToUpper());
        }

        /// <summary>
        /// Возвращает вершину с данным именем.
        /// Если слово не встречается в сем. сети, возвращает null
        /// </summary>
        /// <param name="word">Слово, синоним или форма слова</param>
        /// <returns></returns>
        public Node FindNode(string word)
        {
            if (word.Trim() == string.Empty)
                throw new ArgumentException(ErrMsg + "Не ищи вершину с пустым именем в сети!");
            //проходим по всем синонимам и формам слов
            foreach (var node in Nodes)
            {
                if (node.Name.Trim().ToUpper() == word.Trim().ToUpper())
                    return node;
                for (int k = 0; k < node.WordForms[node.Name].Count(); k++)
                {
                    var wordForm = node.WordForms[node.Name][k];
                    if (wordForm.Trim().ToUpper() == word.Trim().ToUpper())
                        return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Возвращает вершину с данным именем или с таким синонимом (формой слова).
        /// Если слово не встречается в сем. сети, возвращает null
        /// </summary>
        /// <param name="word">Слово, синоним или форма слова</param>
        /// <returns></returns>
        public Node FindNodeWithSynonyms(string word)
        {
            if (word.Trim() == string.Empty)
                throw new ArgumentException(ErrMsg + "Не ищи вершину с пустым именем в сети!");
            //проходим по всем синонимам и формам слов
            foreach (var node in Nodes)
            {
                for (int j = 0; j < node.WordForms.Keys.Count(); j++)
                {
                    var syn = node.WordForms.Keys.ToList()[j];
                    if (syn.Trim().ToUpper() == word.Trim().ToUpper())
                        return node;
                    for (int k = 0; k < node.WordForms[syn].Count(); k++)
                    {
                        var wordForm = node.WordForms[syn][k];
                        if (wordForm.Trim().ToUpper() == word.Trim().ToUpper())
                            return node;
                    }
                }
            }
            return null;
        }

        public string GetClosestParentName(Node unnamedNode)
        {
            if (ArcExists(unnamedNode.ID, "#is_instance"))
                return GetNameForUnnamedNode(GetAttr(unnamedNode.ID, "#is_instance"), false);
            if (ArcExists(unnamedNode.ID, "#is_a"))
                return GetNameForUnnamedNode(GetAttr(unnamedNode.ID, "#is_a"), false);
            return OldestParentArc(unnamedNode.ID);
        }
        #endregion

        #region Добавление
        /// <summary>
        /// Добавление вершины
        /// </summary>
        /// <param name="nodeName">Имя вершины</param>
        public Node AddNode(string nodeName)
        {
            int intValue;
            Node node;
            if (int.TryParse(nodeName.Trim().ToUpper(), out intValue) || nodeName == string.Empty)
            {
                node = new Node
                           {
                               ID = Nodes.Select(x => x.ID).Max() + 1,
                               Name = nodeName.Trim()
                           };
            }
            else
            {
                var count = Nodes.Count(x => x.Name.Trim().ToUpper() == nodeName.Trim().ToUpper());
                if (count > 0)
                    nodeName += (count + 1).ToString();

                node = new Node { ID = Atom(nodeName), Name = nodeName.Trim() };
            }
            Nodes.Add(node);
            IsChanged = true;
            return node;
        } 
        #endregion

        #region Изменение
        /// <summary>
        /// Изменение имени вершины
        /// </summary>
        /// <param name="nodeID">ID вершины</param>
        /// <param name="newName">Новое имя</param>
        public void ChangeNodeName(int nodeID, string newName)
        {
            var node = Mota(nodeID);
            if (node == null)
                throw new ArgumentException(ErrMsg + 
                    "Мы пытаемся изменить имя несуществующей вершины");
            //foreach (var arc in Arcs.Where(x => x.From.ID == nodeID))
            //    arc.From.Name = newName;
            //foreach (var arc in Arcs.Where(x => x.To.ID == nodeID))
            //    arc.To.Name = newName;
            node.ChangeSynonym(node.Name, newName);
            node.Name = newName;
            IsChanged = true;
        } 

        /// <summary>
        /// Изменение координат вершины
        /// </summary>
        /// <param name="nodeID">ID вершины</param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        public void ChangeNodeCoordinates(int nodeID, double newX, double newY)
        {
            var node = Mota(nodeID);
            node.X = newX;
            node.Y = newY;
            IsChanged = true;
        }
        #endregion

        #region Удаление
        /// <summary>
        /// Удаление вершины
        /// </summary>
        /// <param name="nodeName">Имя вершниы</param>
        public void DeleteNodeByName(string nodeName)
        {
            var node = Nodes.Where(x => x.Name == nodeName).ToList();
            if (node.Count() != 1)
                throw new ArgumentException("Невозможно удалить вершину по имени, т.к. вершин" +
                                            "с таким именем несколько");
            DeleteNode(node[0]);
            IsChanged = true;
        }

        /// <summary>
        /// Удаление вершины
        /// </summary>
        /// <param name="node">Вершина</param>
        public void DeleteNode(Node node)
        {
            DeleteNode(node.ID);
            IsChanged = true;
        }

        /// <summary>
        /// Удаление вершины
        /// </summary>
        /// <param name="nodeID">ID вершины</param>
        public void DeleteNode(int nodeID)
        {
            if (Nodes.All(x => x.ID != nodeID))
                throw new ArgumentException(ErrMsg + "Мы пытаемся удалить несуществующую вершину");
            //Удаляем входящие и исходящие дуги
            var arc = Arcs.Where(x => (x.From == nodeID || x.To == nodeID)).ToList();
            for (int i = arc.Count - 1; i >= 0; i--)
                DeleteArc(arc[i]);
            //удаляем вершину
            Nodes.Remove(Nodes.Single(x => x.ID == nodeID));
            IsChanged = true;
        } 
        #endregion
        #endregion

        #region Дуги

        #region Полезные дугофункции
        /// <summary>
        /// Проверяет существование дуги
        /// </summary>
        /// <param name="fromID">ID вершины, откуда выходит дуга</param>
        /// <param name="arcName">имя дуги</param>
        /// <param name="toID">ID вершины, куда входит дуга</param>
        /// <returns>true, если дуга существует</returns>
        public bool ArcExists(int fromID, string arcName, int toID)
        {
            return Arcs.Any(x => x.From == fromID &&
                                 x.Name.ToUpper() == arcName.Trim().ToUpper() &&
                                 x.To == toID);
        }

        /// <summary>
        /// Проверяет существование дуги
        /// </summary>
        /// <param name="fromID">ID вершины, откуда выходит дуга</param>
        /// <param name="arcName">имя дуги</param>
        /// <returns>true, если дуга существует</returns>
        public bool ArcExists(int fromID, string arcName)
        {
            return Arcs.Any(x => x.From == fromID &&
                                 x.Name.ToUpper() == arcName.Trim().ToUpper());
        }

        /// <summary>
        /// Получает вершину, куда можно попасть из данной вершины по данной дуге
        /// </summary>
        /// <param name="fromID">ID вершины, откуда выходит дуга</param>
        /// <param name="arcName">имя дуги</param>
        /// <returns></returns>
        public Node GetAttr(int fromID, string arcName)
        {
            try
            {
                var t = Arcs.SingleOrDefault(x => x.From == fromID &&
                                                  x.Name.ToUpper() == arcName.Trim().ToUpper());
                return t != null ? Mota(t.To) : null;
            }
            catch (Exception e)
            {
                throw new ArgumentException(ErrMsg + e.Message);
            }
        } 

        /// <summary>
        /// Получает список вершин, куда можно попасть из данной вершины по данной дуге
        /// </summary>
        /// <param name="fromID">ID вершины</param>
        /// <param name="arcName">Имя дуги</param>
        /// <returns></returns>
        public List<Node> GetAllAttr(int fromID, string arcName)
        {
            return Arcs.Where(x => x.From == fromID &&
                                   x.Name.ToUpper() == arcName.Trim().ToUpper())
                                   .Select(x => Mota(x.To)).ToList();
        }

        /// <summary>
        /// Получает список вершин, из которых можно попасть в данную вершину nodeID по 
        /// данной дуге arcName. Список может оказаться пустым.
        /// </summary>
        /// <param name="nodeID">ID вершины, куда хотим попасть</param>
        /// <param name="arcName">имя дуги</param>
        /// <returns></returns>
        public List<Node> GetNodesDirectedToMe(int nodeID, string arcName)
        {
            return Arcs
                .Where(x => x.To == nodeID &&
                                   x.Name.Trim().ToUpper() == arcName.Trim().ToUpper())
                .Select(x => Mota(x.From)).ToList();
        }

        /// <summary>
        /// Получает список дуг, которые входят в данную вершину. Список может оказаться пустым.
        /// </summary>
        /// <param name="nodeID">ID вершины, куда хотим попасть</param>
        /// <returns></returns>
        public List<Arc> GetArcsDirectedToMe(int nodeID)
        {
            return Arcs.Where(x => x.To == nodeID).ToList();
        }
        
        /// <summary>
        /// Возвращает список дуг, которые проходят между двумя заданными вершинами
        /// </summary>
        /// <param name="fromID">Откуда</param>
        /// <param name="toID">Куда</param>
        /// <returns></returns>
        public List<Arc> GetArcsBetweenNodes(int fromID, int toID)
        {
            return Arcs.Where(x => x.From == fromID && x.To == toID).ToList();
        }

        /// <summary>
        /// Получает список вершин, куда можно попасть из данной вершины
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public List<Node> GetNodesFromMe(int nodeID)
        {
            return Arcs.Where(x => x.From == nodeID).Select(x => Mota(x.To)).ToList();
        }

        /// <summary>
        /// Для неименованной вершины получает имя соответствующей именованной вершины
        /// </summary>
        /// <param name="unnamedNode"></param>
        /// <param name="searchParent"> </param>
        /// <returns></returns>
        public string GetNameForUnnamedNode(Node unnamedNode, bool searchParent)
        {
            if (ArcExists(unnamedNode.ID, "#Name"))
            {
                var nameNode = GetAllAttr(unnamedNode.ID, "#Name");
                if (nameNode.Count == 1)
                    return nameNode[0].Name;
                throw new ArgumentException(ErrMsg + "Из неименованной вершины выходит несколько дуг #Name");
            }
            if (searchParent && ArcExists(unnamedNode.ID, "#is_instance"))
            {
                var parentNode = GetAttr(unnamedNode.ID, "#is_instance");
                return GetNameForUnnamedNode(parentNode, false);
            }
            return null;
        }

        /// <summary>
        /// По имени получает неименованные вершины, откуда выходит дуга Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Node GetUnnamedNodesForName(string name)
        {
            if (name == string.Empty)
                throw new ArgumentException(ErrMsg + "Имя пустое");
            var namedNode = Mota(Atom(name));
            var unnamedNode = GetNodesDirectedToMe(namedNode.ID, "#Name").ToList();
            if (unnamedNode.Count > 1)
                throw new ArgumentException(ErrMsg + "Волею Хаоса сему понятию дана неоднозначность " + name);
            if (unnamedNode.Count == 0)
                throw new ArgumentException("Такого имени нет в сети");
            return unnamedNode[0];
        }

        public List<Node> GetAllUnnamedNodesForName(string name)
        {
            if (name == string.Empty)
                throw new ArgumentException(ErrMsg + "Имя пустое");
            var namedNode = Mota(Atom(name));
            return GetNodesDirectedToMe(namedNode.ID, "#Name").ToList();
        }
        #endregion

        #region Добавление

        /// <summary>
        /// Добавление дуги
        /// </summary>
        /// <param name="fromID">ID вершины, откуда идёт дуга</param>
        /// <param name="arcName">Имя дуги</param>
        /// <param name="toID">ID вершины, куда идёт дуга</param>
        public Arc AddArc(int fromID, string arcName, int toID)
        {
            var fromNode = Mota(fromID);
            var toNode = Mota(toID);
            if (ArcExists(fromID, arcName, toID))
                throw new ArgumentException(string.Format("Между вершинами {0} и {1} " +
                                                          "уже есть дуга {2}",
                                                          fromNode.Name, toNode.Name, arcName));
            if (fromID == toID)
                throw new ArgumentException("Нельзя ссылаться на себя");
            //Добавляем дугу в список
            var arc = new Arc
                          {
                              From = fromNode.ID,
                              Name = arcName.Trim(),
                              To = toNode.ID
                          };
            Arcs.Add(arc);
            IsChanged = true;
            return arc;
        }
        #endregion

        #region Изменение
        /// <summary>
        /// Изменение имени дуги
        /// </summary>
        /// <param name="fromID">ID вершины, откуда идёт дуга</param>
        /// <param name="oldName">Старое имя дуги</param>
        /// <param name="newName">Новое имя дуги</param>
        /// <param name="toID"> </param>
        public void ChangeArcName(int fromID, string oldName, string newName, int toID)
        {
            DeleteArc(fromID, oldName, toID);
            AddArc(fromID, newName, toID);
        }

        /// <summary>
        /// Изменение вершины, откуда выходит дуга
        /// </summary>
        /// <param name="oldFromID">Раньше дуга выходила отсюда</param>
        /// <param name="newFromID">Сейчас дуга будет выходить отсюда</param>
        /// <param name="arcName">Имя дуги</param>
        /// <param name="toID">Имя вершины, куда идёт дуга</param>
        public void ChangeArcDirectionFrom(int oldFromID, int newFromID, string arcName, int toID)
        {
            DeleteArc(oldFromID, arcName, toID);
            AddArc(newFromID, arcName, toID);
        }

        /// <summary>
        /// Изменение вершины, куда идёт дуга
        /// </summary>
        /// <param name="fromID">ID вершины, откуда выходит дуга</param>
        /// <param name="arcName">Имя дуги</param>
        /// <param name="oldToID">Раньше дуга входила сюда</param>
        /// <param name="newToID">Сейчас дуга будет входить сюда</param>
        public void ChangeArcDirectionTo(int fromID, string arcName, int oldToID, int newToID)
        {
            DeleteArc(fromID, arcName, oldToID);
            AddArc(fromID, arcName, newToID);
        } 
        #endregion

        #region Удаление
        /// <summary>
        /// Удаляет дугу с именем arcName, проходящую между вершинами from и to.
        /// </summary>
        /// <param name="fromID">ID вершины, откуда выходит дуга</param>
        /// <param name="arcName">Имя дуги</param>
        /// <param name="toID">ID вершины куда идёт дуга</param>
        public void DeleteArc(int fromID, string arcName, int toID)
        {
            try
            {
                var arcToRemove = Arcs.SingleOrDefault(x => x.From == fromID &&
                                               x.Name.Trim().ToUpper() == arcName.Trim().ToUpper() &&
                                               x.To == toID);
                Arcs.Remove(arcToRemove);
                IsChanged = true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ErrMsg + ex.Message);
            }
            
        } 

        /// <summary>
        /// Удаляет дугу
        /// </summary>
        /// <param name="arc">Дуга, которую надо удалить</param>
        public void DeleteArc(Arc arc)
        {
            DeleteArc(arc.From, arc.Name, arc.To);
        }
        #endregion
        
        #endregion

        #region Working with metadata

        /// <summary>
        /// Получает возможные имена для дуг
        /// </summary>
        /// <param name="fromID">ID вершины</param>
        /// <returns></returns>
        public List<string> GetAllowedArcNames(int fromID)
        {
            var names = new List<string>();
            /* Ищем самого верхнего предка по дугам is_a, is_instance и смотрим, какой дугой
             * связан этот предок с вершиной System.
             * Если fromID = System.ID, то возможны дуги из MetaObjects.
             * Если последняя дуга MetaRelations или MetaObjects, то ничего.
             * Если последняя дуга Relations, то возможны дуги из MetaRelations.
             * В остальных случаях мы имеем дело с дугами из предметной области.
             * Здесь помимо Relations и MetaRelations допустимы дуги из соответствующих 
             * атрибутов MetaObjects.
             */
            //Если имеем дело с вершиной System
            if (fromID == Atom("#System"))
            {
                names.AddRange(ArcsOnlyForSystem);
                var metaObjNodes = GetAllAttr(fromID, "#MetaObjects");
                names.AddRange(metaObjNodes
                                   .Where(x => ArcExists(x.ID, "#Name"))
                                   .Select(x => GetAttr(x.ID, "#Name").Name).ToList());

            }
            else
            {
                names.AddRange(SystemArcs);
                //Находим самого верхнего предка и дугу, которой он связан с System
                string oldestArcName = "";
                try
                {
                    oldestArcName = OldestParentArc(fromID);
                }
                catch {}
                finally
                {
                    switch (oldestArcName)
                    {
                        case "#MetaRelations":
                        case "#MetaObjects":
                        case "":
                            break;
                        case "#Relations":
                            names.AddRange(ArcForRelations());
                            break;
                        default:
                            names.AddRange(ArcForRelations());
                            names.AddRange(ArcsForObjectsFromRelations());
                            names.AddRange(ArcsForObjectsFromAttributes(oldestArcName));
                            break;
                    }
                }
            }
            return names;
        }

        /// <summary>
        /// Для неименованной вершины nodeID находит самого верхнего предка по дугам is_a и is_instance
        /// и возвращает имя дуги, которой этот предок связан с вершиной System
        /// </summary>
        /// <param name="nodeID">ID вершины</param>
        /// <returns>имя дуги, которой предок связан с вершиной System</returns>
        public string OldestParentArc(int nodeID)
        {
            WayToSystemNodes = new List<Node>();
            WayToSystemArcs = new List<Arc>();
            Node parentNode = Mota(nodeID);
            WayToSystemNodes.Add(parentNode);
            if (ArcExists(parentNode.ID, "#is_instance"))
            {
                WayToSystemArcs.Add(Arcs.Single(x => x.From == parentNode.ID && x.Name == "#is_instance"));
                parentNode = GetAttr(parentNode.ID, "#is_instance");
                WayToSystemNodes.Add(parentNode);
            }
            while (ArcExists(parentNode.ID, "#is_a"))
            {
                WayToSystemArcs.Add(Arcs.Single(x => x.From == parentNode.ID && x.Name == "#is_a"));
                parentNode = GetAttr(parentNode.ID, "#is_a");
                WayToSystemNodes.Add(parentNode);
            }
            var arcToSystem = GetArcsBetweenNodes(Atom("#System"), parentNode.ID).ToList();
            if (!arcToSystem.Any())
                return string.Empty;
            if (arcToSystem.Count()> 1)
                throw new ArgumentException("Между родителем и системной вершиной больше 1 дуги");
            WayToSystemArcs.Add(Arcs.Single(x => x.From == Atom("#System") && x.Name == arcToSystem[0].Name && x.To == parentNode.ID));
            WayToSystemNodes.Add(Mota(Atom("#System")));
            return arcToSystem[0].Name;
        }

        /// <summary>
        /// Возвращает дуги, описанные в MetaRelations
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> ArcForRelations()
        {
            var metaRel = GetAllAttr(Atom("#System"), "#MetaRelations");
            return metaRel.Select(x => GetAttr(x.ID, "#Name").Name);
        }

        /// <summary>
        /// Возвращает дуги, описанные в Relations
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> ArcsForObjectsFromRelations()
        {
            var rel = GetAllAttr(Atom("#System"), "#Relations");
            return rel.Select(x => GetAttr(x.ID, "#Name").Name);
        }

        /// <summary>
        /// Возвращает имена дуг, доступные для объектов, т.е. 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> ArcsForObjectsFromAttributes(string oldestParentArcName)
        {
            try
            {
                //проходим по дугам с именем #MetaObjects => находим все метаобъекты
                List<Node> metaObj = GetAllAttr(Atom("#System"), "#MetaObjects");
                Node nodeWeAreLookingFor = metaObj
                    .Single(x => GetAttr(x.ID, "#Name").Name == oldestParentArcName);
                List<Node> attributes = GetAllAttr(nodeWeAreLookingFor.ID, "#HasAttribute");
                return attributes.Select(x => GetAttr(x.ID, "#Name").Name);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ErrMsg + ex.Message);
            }
        }

        /// <summary>
        /// Возвращает имена всех метаобъетов, имеющихся в системе
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllMetaObjectNames()
        {
            return GetAllAttr(Atom("#System"), "#MetaObjects").Select(unnamedNode => GetNameForUnnamedNode(unnamedNode, false)).ToList();
        }

        /// <summary>
        /// Возвращает имена тех метаобъектов, которые имеют атрибуты, но не являются ничьим атрибутом
        /// </summary>
        /// <returns></returns>
        public List<string> GetMainMetaObjectNames()
        {
            var res = new List<string>();
            //В нашем случае возвращает Коктейль
            foreach (var metaObj in GetAllAttr(Atom("#System"), "#MetaObjects"))
            {
                int attrCount = GetAllAttr(metaObj.ID, "#HasAttribute").Count();
                int usagesCount = GetArcsDirectedToMe(metaObj.ID).Count(x => x.Name == "#HasAttribute");
                if (attrCount > 0 && usagesCount == 0)
                    res.Add(GetNameForUnnamedNode(metaObj, false));
            }
            return res;
        }

        public List<Node> GetMainMetaObjectNamedNodes()
        {
            return GetMainMetaObjectNames().Select(x => Mota(Atom(x))).ToList();
        }

        /// <summary>
        /// Возвращает имена всех отношений, имеющихся в системе
        /// </summary>
        /// <returns></returns>
        public List<string> GetRelationNames()
        {
            return GetAllAttr(Atom("#System"), "#Relations").Select(unnamedNode => GetNameForUnnamedNode(unnamedNode, false)).ToList();
        }

        #endregion

        #region Сериализация/Десериализация
        /// <summary>
        /// Сериализация
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public static void WriteToXml(string path)
        {
            var xw = new XmlTextWriter(path, Encoding.UTF8) { Formatting = Formatting.Indented };
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(xw);
            var ser = new DataContractSerializer(typeof(SemanticWeb));
            ser.WriteObject(writer, _web);
            writer.Close();
            xw.Close();
            IsChanged = false;
        }

        /// <summary>
        /// Десериализация
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public static void ReadFromXml(string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    var reader = XmlDictionaryReader.CreateTextReader(fs, Encoding.UTF8,
                                                                      new XmlDictionaryReaderQuotas(), null);
                    var ser = new DataContractSerializer(typeof(SemanticWeb));
                    _web = (SemanticWeb)ser.ReadObject(reader);
                }
                IsChanged = false;
            }
            catch
            {
                _web = new SemanticWeb();
            }
        } 
        #endregion

        #region Closing
        //public static void MakeEverythingGood()
        //{
        //    foreach (var node in _web.Nodes)
        //    {
        //        if (node.Synonyms.Count == 0)
        //            node.Synonyms.Add(node.Name);
        //        foreach (var synonym in node.Synonyms)
        //        {
        //            node.WordForms.Add(synonym, new List<string>());
        //        }
        //    }
        //}

        public static void Close()
        {
            _web = null;
        } 
        #endregion

    }
}
