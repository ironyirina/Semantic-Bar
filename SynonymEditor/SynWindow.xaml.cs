using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Kernel;

namespace SynonymEditor
{
    /// <summary>
    /// Interaction logic for SynWindow.xaml
    /// </summary>
    public partial class SynWindow
    {
        #region Variables
        /// <summary>
        /// Путь к файлу сем. сети
        /// </summary>
        private readonly string _fileName;
        /// <summary>
        /// Выбранный синоним (для отображения форм слова)
        /// </summary>
        private string _selectedSynonym;
        /// <summary>
        /// Имя вершины - главный из синонимов
        /// </summary>
        private readonly string _word;

        private readonly Node _namedNode;
        private string _selectedWordForm;
        #endregion

        #region Initialization
        public SynWindow(string fileName, string word)
        {
            _fileName = fileName;
            _word = word;
            InitializeComponent();
            Title = "Синонимы к слову " + _word;
            _namedNode = SemanticWeb.Web().Mota(SemanticWeb.Web().Atom(_word));
            SetDataContext();
        }

        static SynWindow()
        {
            Apply = new RoutedUICommand("Apply", "Apply", typeof(SynWindow));
            Cancel = new RoutedUICommand("Cancel", "Cancel", typeof(SynWindow));
            AddSynonym = new RoutedUICommand("AddSynonym", "AddSynonym", typeof(SynWindow));
            ChangeSynonym = new RoutedUICommand("ChangeSynonym", "ChangeSynonym", typeof(SynWindow));
            DeleteSynonym = new RoutedUICommand("DeleteSynonym", "DeleteSynonym", typeof(SynWindow));
            AddWordForm = new RoutedUICommand("AddWordForm", "AddWordForm", typeof(SynWindow));
            ChangeWordForm = new RoutedUICommand("ChangeWordForm", "ChangeWordForm", typeof(SynWindow));
            DeleteWordForm = new RoutedUICommand("DeleteWordForm", "DeleteWordForm", typeof(SynWindow));
        } 

        private void SetDataContext()
        {
            lbSyn.DataContext = _namedNode.WordForms.Keys;
            if (_selectedSynonym != null && _namedNode.WordForms.ContainsKey(_selectedSynonym))
                lbWordForms.DataContext = _namedNode.WordForms[_selectedSynonym];
            lbSyn.Items.Refresh();
            lbWordForms.Items.Refresh();
        }

        private void LbSynSelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (lbSyn.SelectedItem == null) return;
            _selectedSynonym = (string)lbSyn.SelectedItem;
            lbWordForms.DataContext = _namedNode.WordForms[_selectedSynonym];
        }
        #endregion

        #region Apply/Cancel/Close

        #region Применить
        public static RoutedUICommand Apply { get; private set; }

        private void ApplyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SemanticWeb.WriteToXml(_fileName);
            SetDataContext();
        }

        private void ApplyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SemanticWeb.IsChanged;
        }
        #endregion

        #region Отменить
        public static RoutedUICommand Cancel { get; private set; }
        private void CancelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SemanticWeb.ReadFromXml(_fileName);
            SetDataContext();
        }
        private void CancelCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SemanticWeb.IsChanged;
        }
        #endregion

        #region Закрыть
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!SemanticWeb.IsChanged) return;
            var d = MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNoCancel);
            switch (d)
            {
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
                case MessageBoxResult.Yes:
                    Apply.Execute(null, null);
                    break;
            }
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #endregion

        #region Synonyms

        #region Добавить

        public static RoutedUICommand AddSynonym { get; private set; }

        public void AddExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var f = new AddChangeSynWindow("Добавление синонима", "Синоним:", _word, string.Empty, true);
            f.EventAddAgain += SynEventHandler;
            f.ShowDialog();
            f.EventAddAgain -= SynEventHandler;
        }

        private void SynEventHandler(string s)
        {
            try
            {
                _namedNode.AddSynonym(s);
                SetDataContext();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion

        #region Изменить

        public static RoutedUICommand ChangeSynonym { get; private set; }

        public void ChangeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var f = new AddChangeSynWindow("Изменение синонима", "Синоним:", _word, _selectedSynonym, false);
            if (f.ShowDialog() != true)
                return;
            try
            {
                _namedNode.ChangeSynonym(_selectedSynonym, f.NewWord);
                SetDataContext();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ChangeAndDeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lbSyn.SelectedItem != null && (string)lbSyn.SelectedItem != _word;
        }

        #endregion

        #region Удалить

        public static RoutedUICommand DeleteSynonym { get; private set; }

        public void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _namedNode.DeleteSynonym(_selectedSynonym);
                SetDataContext();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion


        #endregion

        #region Word Forms

        #region Добавить

        public static RoutedUICommand AddWordForm { get; private set; }

        public void AddFormExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var f = new AddChangeSynWindow("Добавление формы слова", "Форма:", _selectedSynonym, string.Empty, true);
            f.EventAddAgain += WordEventHandler;
            f.ShowDialog();
            f.EventAddAgain -= WordEventHandler;
        }

        private void WordEventHandler(string s)
        {
            try
            {
                _namedNode.AddWordForm(_selectedSynonym, s);
                SetDataContext();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddFormCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _selectedSynonym != null;
        }

        #endregion

        #region Изменить

        public static RoutedUICommand ChangeWordForm { get; private set; }

        public void ChangeFormExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var f = new AddChangeSynWindow("Изменение формы слова", "Форма:", _selectedSynonym, _selectedWordForm, false);
            if (f.ShowDialog() != true)
                return;
            try
            {
                _namedNode.ChangeWordForm(_selectedSynonym, _selectedWordForm, f.NewWord);
                SetDataContext();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ChangeAndDeleteFormCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lbWordForms.SelectedItem != null;
        }

        #endregion

        #region Удалить

        public static RoutedUICommand DeleteWordForm { get; private set; }

        public void DeleteFormExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _namedNode.DelWordForm(_selectedSynonym, _selectedWordForm);
                SetDataContext();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void LbWordFormsSelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (lbWordForms.SelectedItem != null)
                _selectedWordForm = (string)lbWordForms.SelectedItem;
        }
        #endregion
   
    }
}
