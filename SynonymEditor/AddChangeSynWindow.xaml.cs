using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SynonymEditor
{
    /// <summary>
    /// Interaction logic for AddChangeSynWindow.xaml
    /// </summary>
    public partial class AddChangeSynWindow : Window
    {
        #region Переменные
        private readonly bool _allowManyAddings;
        public string NewWord { get; private set; } 
        #endregion

        #region Инициализация
        public AddChangeSynWindow(string title, string label, string tbText, string tb2Text, bool allowManyAddings)
        {
            _allowManyAddings = allowManyAddings;
            InitializeComponent();
            Title = title;
            lblItemType.Content = label;
            tbWord.Text = tbText;
            tbNewItem.Text = tb2Text;
        }

        static AddChangeSynWindow()
        {
            Add = new RoutedUICommand("Add", "Add", typeof(AddChangeSynWindow));
        } 
        #endregion

        #region Добавление
        public static RoutedUICommand Add { get; private set; } 

        public void AddExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            NewWord = tbNewItem.Text.Trim();
            OnEventAddAgain(NewWord);
            tbNewItem.Text = string.Empty;
            tbNewItem.Focus();
        }

        public void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tbNewItem.Text.Trim() != string.Empty;
        }
        #endregion

        #region Событие при добавлении
        public event Action<string> EventAddAgain;

        public void OnEventAddAgain(string obj)
        {
            var handler = EventAddAgain;
            if (handler != null)
                handler(obj);
        } 
        #endregion

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnAddClick(object sender, RoutedEventArgs e)
        {
            if (!_allowManyAddings)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
