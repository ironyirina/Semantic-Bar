using System.Collections.Generic;
using System.Linq;

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

        private readonly bool _searchSynonyms;

        private List<string> _words;

        public List<MyConceptStruct> Concepts { get; private set; }

        private readonly int _maxLen;

        private readonly List<string> _wordsToSkip = new List<string>
                                                {
                                                    "в",
                                                    "и",
                                                    "с"
                                                };

        #endregion

        #region Инициализация

        public ConceptWorker(int maxLen, bool searchSynonyms)
        {
            _maxLen = maxLen;
            _searchSynonyms = searchSynonyms;
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

        public List<string> FindAll(string query, IEnumerable<string> wordsToSkipHere)
        {
            var skipHere = wordsToSkipHere == null ? _wordsToSkip : _wordsToSkip.Union(wordsToSkipHere).ToList();
            _query = query;
            _words = _query.Split(' ', ',', '.', ':', ')', '\r', '\n').Where(x => x != string.Empty).ToList();
            Concepts.Clear();
            while (_words.Count > 0)
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
                    Concepts.Add(new MyConceptStruct { Name = _words[0], IsRecognized = false });
                    _words.RemoveAt(0);
                }
                else
                {
                    Concepts.Add(nearestWords[successIndex]);
                    _words.RemoveRange(0, successIndex + 1);
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
                var recognozedNode = _searchSynonyms
                                         ? SemanticWeb.Web().FindNodeWithSynonyms(s)
                                         : SemanticWeb.Web().FindNode(s);
                nearestWords.Add(new MyConceptStruct
                                     {
                                         Name = recognozedNode == null
                                                    ? s.Trim()
                                                    : recognozedNode.Name,
                                         IsRecognized = recognozedNode != null
                                     });
                k++;
            }
            return nearestWords;
        }
    }
}
