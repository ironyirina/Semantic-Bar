using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kernel;

namespace KnowledgeAcquisitionComponent
{
    class Loader
    {
        #region Переменные

        public SemanticWeb SW { get; set; }

        private StreamReader _reader;

        private string _fileContent;

        private readonly string _fileName;

        private readonly List<string> _wordsToSkip = new List<string>
                                                {
                                                    "в",
                                                    "и"
                                                };

        private Node _entityNode;
        private Node _fileNode;
        private Node _namedFileNode;
        private string _currentArcName;
        private Node _complexEntity;

        private Action<string> _sendReport;

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

            var cw = new ConceptWorker(SW, 5);
            _sendReport("Разбиваем текст файла на понятия");
            cw.FindConcepts(_fileContent, null);

            if (!AnalyzeName(DefineName(cw.Concepts))) return; //определяем и анализируем имя

            var unknownWords = cw.FindConcepts(_fileContent, null);
            while (unknownWords.Count > 0)
            {
                _sendReport("Найдены неизвестные слова: " + unknownWords.Aggregate("", (s1, s2) => s1 + s2 + " ")); 
                var w = new UnknownWordsWindow {UnknownWords = unknownWords, SW = SW};
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
            var unnamedNode = SW.GetUnnamedNodeForName(concept);
            var type = SW.OldestParentArc(unnamedNode.ID);
            if (type != "#MetaObjects") return false;
            try
            {
                //Неименованная вершина Коктейль (#1)
                _sendReport("Создаём неименованную вершину для концепта");
                _entityNode = SW.AddNode(string.Empty);
                _sendReport("Проводим дугу от вершины SYSTEM с именем " + concept + " к неименованной вершине");
                SW.AddArc(SW.Atom("#System"), concept, _entityNode.ID);
                //Неименованная вершина Файл (#2)
                _fileNode = SW.AddNode("");
                SW.AddArc(_entityNode.ID, "#FileName", _fileNode.ID);
                //Вершина с именем файла (#3)
                _namedFileNode = SW.AddNode(_fileName);
                //(#2)-Name-(#3)
                SW.AddArc(_fileNode.ID, "#Name", _namedFileNode.ID);
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
                SW.DeleteNode(_entityNode);
                SW.DeleteNode(_fileNode);
                SW.DeleteNode(_namedFileNode);
                return null;
            }
            return concepts
                .Where(concept => !concept.IsRecognized)
                .Aggregate(string.Empty, (current, concept) => current + (concept.Name + " ")).Trim();
        }

        private bool AnalyzeName(string name)
        {
            _sendReport("Имя концепта " + name);
            try
            {
                _sendReport("Создаём вершину для имени и соединяем её дугой Name с концептом");
                var entityName = SW.AddNode(name);
                SW.AddArc(_entityNode.ID, "#Name", entityName.ID);
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
            var unnamedNode = SW.GetUnnamedNodeForName(word);
            return SW.GetAllAttr(unnamedNode.ID, "#HasAttribute").Count != 0;
        }

        private void Ananyze(string concept)
        {
            _sendReport("Слово " + concept);
            var unnamedNode = SW.GetUnnamedNodeForName(concept);
            var type = SW.OldestParentArc(unnamedNode.ID);
            _sendReport("Это " + type);
            if (type == "#MetaObjects")
            {
                //надо добавить проверку атрибута
                _currentArcName = concept;
                return;
            }
            //if (type != _currentArcName) return;
            try
            {
                if (IsComplex(type))
                {
                    //в нашем случае Действия
                    //создаём неименованную вершину (_complexEntity)
                    _complexEntity = SW.AddNode("");
                    //проводим дугу _currentArcName (Действия) от _entityNode к _complexEntity
                    SW.AddArc(_entityNode.ID, _currentArcName, _complexEntity.ID);
                    //проводим дугу #is_instance от _complexEntity к unnamedNode (Соответствующее действие)
                    SW.AddArc(_complexEntity.ID, "#is_instance", unnamedNode.ID);
                }
                else
                {
                    //в нашем случае Ингредиенты, ёмкость, инструмент
                    //Проводим дугу с именем _currentArcName (Ингредиенты) от _entityNode к unnamedNode 
                    SW.AddArc(_complexEntity == null ? _entityNode.ID : _complexEntity.ID, type, unnamedNode.ID);
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
