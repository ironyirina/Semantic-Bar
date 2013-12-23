using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Kernel;

namespace KnowledgeAcquisitionComponent
{
    class Loader
    {
        #region Переменные

        private StreamReader _reader;

        private string _fileContent;

        private readonly string _fileName;

        private Node _entityNode;
        private Node _fileNode;
        private Node _namedFileNode;
        private string _currentArcName;
        private Node _complexEntity;

        private readonly Action<string> _sendReport;

        private const string ErrMsg = "Need to be fixed! ";

        #endregion

        #region Инициализация
        public Loader(string fileName, Action<string> sendReport)
        {
            _fileName = fileName;
            _sendReport = sendReport;
        }

        #endregion

        #region Загрузка
        public void Load()
        {
            _sendReport("Начинаем загрузку из файла " + _fileName);
            _reader = new StreamReader(_fileName);
            _fileContent = _reader.ReadToEnd();
            var words = _fileContent.Split(new[] {' ', '\r', '\n', '.', ',', ':'}).Where(x => x != string.Empty).ToList();
            if (words.Count < 2) 
                _sendReport("Ошибка! Файл не содержит достаточно информации");

            if (!AnalyzeFirst(words[0])) return;

            var cw = new ConceptWorker(5, true);
            _sendReport("Разбиваем текст файла на понятия");
            cw.FindConcepts(_fileContent, null);

            if (!AnalyzeName(DefineName(cw.Concepts))) return; //определяем и анализируем имя

            var unknownWords = cw.FindConcepts(_fileContent, null);
            while (unknownWords.Count > 0)
            {
                _sendReport("Найдены неизвестные слова: " + unknownWords.Aggregate("", (s1, s2) => s1 + s2 + " "));
                var w = new UnknownWordsWindow { UnknownWords = unknownWords};
                w.ShowDialog();
                unknownWords = cw.FindConcepts(_fileContent, w.SkippedWords);
            }

            cw.Concepts.RemoveRange(0, 2);
            foreach (var concept in cw.Concepts.Select(x => x.Name))
            {
                Ananyze(concept);
            }
        } 
        
        private bool AnalyzeFirst(string concept)
        {
            _sendReport("Слово " + concept);
            var unnamedNode = SemanticWeb.Web().GetUnnamedNodesForName(concept);
            var type = SemanticWeb.Web().OldestParentArc(unnamedNode.ID);
            if (type != "#MetaObjects") return false;
            try
            {
                //Неименованная вершина Коктейль (#1)
                _sendReport("Создаём неименованную вершину для концепта");
                _entityNode = SemanticWeb.Web().AddNode(string.Empty);
                _sendReport("Проводим дугу от вершины SYSTEM с именем " + concept + " к неименованной вершине");
                SemanticWeb.Web().AddArc(SemanticWeb.Web().Atom("#System"), concept, _entityNode.ID);
                //Неименованная вершина Файл (#2)
                _fileNode = SemanticWeb.Web().AddNode("");
                SemanticWeb.Web().AddArc(_entityNode.ID, "Файл", _fileNode.ID);
                //Вершина с именем файла (#3)
                _namedFileNode = SemanticWeb.Web().AddNode(_fileName);
                //(#2)-Name-(#3)
                SemanticWeb.Web().AddArc(_fileNode.ID, "#Name", _namedFileNode.ID);
                return true;
            }
            catch (Exception e)
            {
                _sendReport("Ошибка! " + e.Message);
                return false;
            }
        }

        private string DefineName(List<ConceptWorker.MyConceptStruct> concepts)
        {
            //структура: Коктейль (+) Имя (-) { Имя (-)} Ингредиент (+)
            //если Коктейль (+) Имя (+) ... , то такое имя уже существует => откатываем изменения
            if (concepts.Count < 2 || !concepts[0].IsRecognized)
                throw new ArgumentException(ErrMsg + "Файл вообще не о коктейле, должно было сломаться на анализе первого слова");
            if (concepts[1].IsRecognized)
            {
                _sendReport("Ошибка! Объект с таким именем уже существует в системе. Откатываем изменения.");
                SemanticWeb.Web().DeleteNode(_entityNode);
                SemanticWeb.Web().DeleteNode(_fileNode);
                SemanticWeb.Web().DeleteNode(_namedFileNode);
                return null;
            }
            return concepts
                .Where(concept => !concept.IsRecognized)
                .Aggregate(string.Empty, (current, concept) => current + (concept.Name + " ")).Trim();
        }

        private bool AnalyzeName(string name)
        {
            if (name == null)
                return false;
            _sendReport("Имя концепта " + name);
            try
            {
                _sendReport("Создаём вершину для имени и соединяем её дугой Name с концептом");
                var entityName = SemanticWeb.Web().AddNode(name);
                SemanticWeb.Web().AddArc(_entityNode.ID, "#Name", entityName.ID);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        private bool IsComplex(string word)
        {
            var unnamedNode = SemanticWeb.Web().GetUnnamedNodesForName(word);
            return SemanticWeb.Web().GetAllAttr(unnamedNode.ID, "#HasAttribute").Count != 0;
        }

        private void Ananyze(string concept)
        {
            _sendReport("Слово " + concept);
            var unnamedNode = SemanticWeb.Web().GetUnnamedNodesForName(concept);
            var type = SemanticWeb.Web().OldestParentArc(unnamedNode.ID);
            _sendReport("Это " + type);
            if (type == "#MetaObjects")
            {
                //надо добавить проверку атрибута
                _currentArcName = concept;
                return;
            }
            try
            {
                if (IsComplex(type))
                {
                    //в нашем случае Действия
                    //создаём неименованную вершину (_complexEntity)
                    _complexEntity = SemanticWeb.Web().AddNode("");
                    //проводим дугу _currentArcName (Действия) от _entityNode к _complexEntity
                    SemanticWeb.Web().AddArc(_entityNode.ID, _currentArcName, _complexEntity.ID);
                    //проводим дугу #is_instance от _complexEntity к unnamedNode (Соответствующее действие)
                    SemanticWeb.Web().AddArc(_complexEntity.ID, "#is_instance", unnamedNode.ID);
                }
                else
                {
                    //в нашем случае Ингредиенты, ёмкость, инструмент
                    //Проводим дугу с именем _currentArcName (Ингредиенты) от _entityNode к unnamedNode 
                    SemanticWeb.Web().AddArc(_complexEntity == null ? _entityNode.ID : _complexEntity.ID, type, unnamedNode.ID);
                }
                _sendReport("Знания о концепте успешно добавлены");
            }
            catch (Exception e)
            {
                _sendReport("Ошибка! " + e.Message);
            }
        }

        #endregion
    }
}
