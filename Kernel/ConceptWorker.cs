using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel
{
    /// <summary>
    /// Разделяет строку на понятия, встречающиеся в сем. сети. Если встречаются неизвестные слова, спрашивает про них или пропускает 
    /// </summary>
    public class ConceptWorker
    {
        #region Переменные

        public struct MyConceptStruct
        {
            public string Name;
            public bool IsRecognized;
        }

        private string _query;

        private SemanticWeb _sw;

        private List<string> _words;

        public List<MyConceptStruct> Concepts { get; private set; }

        private readonly int _maxLen;

        private List<string> _wordsToSkip = new List<string>
                                                {
                                                    "в",
                                                    "и"
                                                };

        #endregion

        #region Инициализация

        public ConceptWorker(SemanticWeb sw, int maxLen)
        {
            _sw = sw;
            _maxLen = maxLen;
            Concepts = new List<MyConceptStruct>();
        }

        #endregion

        /// <summary>
        /// Ищет по сети слова и словосочетания из запроса. Возвращает список ненайденных слов
        /// </summary>
        /// <param name="query"> </param>
        /// <param name="wordsToSkipHere">Слова, которые здесь не учитываем </param>
        /// <returns></returns>
        public List<string> FindConcepts(string query, IEnumerable<string> wordsToSkipHere)
        {
            var skipHere = wordsToSkipHere == null ? _wordsToSkip : _wordsToSkip.Union(wordsToSkipHere).ToList();
            _query = query;
            _words = _query.Split(' ', ',', '.', ':', ')', '\r', '\n').Where(x => x != string.Empty).ToList();
            Concepts.Clear();
            bool isRecognizedAfterFailure = false;
            while (_words.Count > 0 && !isRecognizedAfterFailure)
            {
                int t;
                if (skipHere.Contains(_words[0]) || int.TryParse(_words[0], out t))
                {
                    _words.RemoveAt(0);
                    continue;
                }
                var nearestWords = AnalyzeWord();
                int successIndex = nearestWords.Any(x => x.IsRecognized) ? nearestWords.IndexOf(nearestWords.Last(x => x.IsRecognized)) : -1;
                if (successIndex < 0)
                {
                    //не нашли слово
                    Concepts.Add(new MyConceptStruct {Name = _words[0], IsRecognized = false});
                    _words.RemoveAt(0);
                }
                else
                {
                    if (Concepts.Count > 0 && !Concepts[Concepts.Count-1].IsRecognized)
                        isRecognizedAfterFailure = true;
                    Concepts.Add(nearestWords[successIndex]);
                    _words.RemoveRange(0, successIndex +1);
                }
            }
            return Concepts.Where(x => !x.IsRecognized).Select(x => x.Name).ToList();
        }

        private List<MyConceptStruct> AnalyzeWord()
        {
            var nearestWords = new List<MyConceptStruct>();
            int k = 1;
            while (k <= _maxLen && k <= _words.Count)
            {
                string s = "";
                for (int i = 0; i < k; i++)
                {
                    s += _words[i] + " ";
                }
                nearestWords.Add(new MyConceptStruct {Name = s.Trim(), IsRecognized = _sw.NodeExists(s)});
                k++;
            }
            return nearestWords;
        }
    }
}
