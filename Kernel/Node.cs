using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Kernel
{
    [DataContract]
    public class Node
    {
        private string _name;

        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Name
        {
            get { return _name; }
            set { 
                if (WordForms != null && WordForms.ContainsKey(_name))
                    WordForms.Remove(_name);
                _name = value;
                if (WordForms != null) 
                    WordForms.Add(_name, new List<string>());
            }
        }

        [DataMember]
        public Dictionary<string, List<string>> WordForms { get; private set; }
        [DataMember]
        public bool IsSystem { get; set; }
        [DataMember]
        public double X { get; set; }
        [DataMember]
        public double Y { get; set; }

        public override string ToString()
        {
            return ID + " - " + Name;
        }

        public Node()
        {
            Name = string.Empty;
            WordForms = new Dictionary<string, List<string>> {{Name, new List<string>()}};
        }

        #region Synonyms

        /// <summary>
        /// Добавление синонима
        /// </summary>
        /// <param name="synonym">Синоним</param>
        public void AddSynonym(string synonym)
        {
            if (WordForms.Keys.Contains(synonym.Trim().ToUpper()))
                throw new ArgumentException("Такой синоним уже существует");
            WordForms.Add(synonym.Trim(), new List<string>());
            SemanticWeb.IsChanged = true;
        }

        /// <summary>
        /// Изменение синонима
        /// </summary>
        /// <param name="oldName">Старое имя</param>
        /// <param name="newName">Новое имя</param>
        public void ChangeSynonym(string oldName, string newName)
        {
            if (!WordForms.Keys.Contains(oldName))
                throw new ArgumentException(SemanticWeb.ErrMsg + "Попытка переименовать несуществующий синоним");
            WordForms.Remove(oldName);
            WordForms.Add(newName.Trim(), new List<string>());
            SemanticWeb.IsChanged = true;
        }

        /// <summary>
        /// Удаление синонима
        /// </summary>
        /// <param name="synonym">Синоним</param>
        public void DeleteSynonym(string synonym)
        {
            WordForms.Remove(synonym);
            SemanticWeb.IsChanged = true;
        }

        #endregion

        #region WordForms

        /// <summary>
        /// Добавление формы слова
        /// </summary>
        /// <param name="word">Слово</param>
        /// <param name="form">Форма слова</param>
        public void AddWordForm(string word, string form)
        {
            if (!WordForms.ContainsKey(word))
                throw new ArgumentException(SemanticWeb.ErrMsg + "Несуществующий синоним при работе с формами слова");
            if (WordForms[word].Contains(form.Trim().ToUpper()))
                throw new ArgumentException("Такая форма слова уже существует");
            WordForms[word].Add(form.Trim());
            SemanticWeb.IsChanged = true;
        }

        /// <summary>
        /// Изменение формы слова
        /// </summary>
        /// <param name="word">слово</param>
        /// <param name="oldForm">старая форма</param>
        /// <param name="newForm">новая форма</param>
        public void ChangeWordForm(string word, string oldForm, string newForm)
        {
            if (!WordForms.ContainsKey(word))
                throw new ArgumentException(SemanticWeb.ErrMsg + "Несуществующий синоним при работе с формами слова");
            if (!WordForms[word].Contains(oldForm))
                throw new ArgumentException(SemanticWeb.ErrMsg + "Попытка изменить несуществующую форму слова");
            int index = WordForms[word].FindIndex(x => x.Trim().ToUpper() == oldForm.Trim().ToUpper());
            WordForms[word][index] = newForm.Trim();
            SemanticWeb.IsChanged = true;
        }

        /// <summary>
        /// Удаление формы слова
        /// </summary>
        /// <param name="word">слово</param>
        /// <param name="form">форма слова</param>
        public void DelWordForm(string word, string form)
        {
            if (!WordForms.ContainsKey(word))
                throw new ArgumentException(SemanticWeb.ErrMsg + "Несуществующий синоним при работе с формами слова");
            WordForms[word].Remove(form);
            SemanticWeb.IsChanged = true;
        }

        #endregion
    }
}
