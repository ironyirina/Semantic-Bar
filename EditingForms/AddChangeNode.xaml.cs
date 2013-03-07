using System;
using System.Windows;
using System.Windows.Input;

namespace EditingForms
{
    /// <summary>
    /// Interaction logic for AddChangeNode.xaml
    /// </summary>
    public partial class AddChangeNode : Window
    {
        public string NodeName { get; private set; }

        public bool AllowManyAddings { get; set; }

        #region Инициализация
        public AddChangeNode(string title, string label, string tbText)
        {
            InitializeComponent();
            btnAdd.Command = Add;
            
            Title = title;
            lblName.Content = label;
            NodeName = tbText;
            tbNodeName.Text = tbText;
        }

        static AddChangeNode()
        {
            Add = new RoutedUICommand("AddNode", "AddNode", typeof(AddChangeNode));
        }
        
        #endregion

        #region Добавление
        public static RoutedUICommand Add { get; private set; }

        public void AddExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            NodeName = tbNodeName.Text.Trim();
            OnEventAddAgain(tbNodeName.Text.Trim());
            tbNodeName.Text = string.Empty;
            tbNodeName.Focus();
        }

        public void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = tbNodeName.Text.Trim() != string.Empty;
            e.CanExecute = true;
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

        private void BtnAddClick1(object sender, RoutedEventArgs e)
        {
            if (!AllowManyAddings)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
