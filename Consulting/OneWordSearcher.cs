using System;
using Kernel;

namespace Consulting
{
    static class OneWordSearcher
    {
        /// <summary>
        /// Поиск одного слова
        /// </summary>
        /// <param name="unnamedNodeToSearch">Неименованная вершина, соответствующая слову из запроса</param>
        /// <param name="executeSimilarQuery"></param>
        /// <returns></returns>
        public static WordResult SearchOneWord(Node unnamedNodeToSearch, Action<string> executeSimilarQuery, WorkMemory workMemory)
        {
            var word = SemanticWeb.Web().GetNameForUnnamedNode(unnamedNodeToSearch, false);
            Node namedNodeToSearch =
                SemanticWeb.Web().FindNode(word);
            //определяем имя дуги, которой самый старший предок связан с #System
            workMemory.WorkedNodes.Add(namedNodeToSearch);
            workMemory.WorkedNodes.AddRange(SemanticWeb.Web().WayToSystemNodes);
            //Для метазнаний выполняем поиск по метазнаниям
            if (SemanticWeb.Web().OldestParentArc(unnamedNodeToSearch.ID) == "#MetaObjects")
                return MetadataSearch.SearchMetaData(unnamedNodeToSearch, word, executeSimilarQuery);
            //Если самая верхняя дуга имеет имя из метаобъектов, ищем слово из предметной области
            if (
                SemanticWeb.Web()
                    .GetAllMetaObjectNames()
                    .Contains(SemanticWeb.Web().OldestParentArc(unnamedNodeToSearch.ID)))
                return ObjectSearcher.SearchObjectData(unnamedNodeToSearch, word,
                    SemanticWeb.Web().OldestParentArc(unnamedNodeToSearch.ID), executeSimilarQuery);
            //throw new ArgumentException(SemanticWeb.ErrMsg + " Слово " + word + " не нашлось");
            return null;
        }

    }
}
