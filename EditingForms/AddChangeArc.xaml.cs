using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Kernel;

namespace EditingForms
{
    /// <summary>
    /// Interaction logic for AddChangeArc.xaml
    /// </summary>
    public partial class AddChangeArc : Window
    {
        private readonly SemanticWeb _sw;

        #region Свойства
        public Node From
        {
            get
            {
                if (cbFrom.SelectedItem != null)
                    return (Node)cbFrom.SelectedItem;
                throw new ArgumentException("Переменных пока нет");
            }
            set { cbFrom.SelectedItem = value; }
        }

        public string ArcName
        {
            get
            {
                if (cbName.SelectedItem != null)
                    return cbName.SelectedItem.ToString();
                throw new ArgumentException("Нет возможных имён для дуг");
            }
            set { cbName.SelectedItem = value; }
        }

        public Node To
        {
            get
            {
                if (cbTo.SelectedItem != null)
                    return (Node)cbTo.SelectedItem;
                throw new ArgumentException("Переменных пока нет");
            }
            set { cbTo.SelectedItem = value; }
        }

        public bool AllowManyAddings { get; set; } 
        #endregion

        #region Инициализация

        /// <summary>
        /// Окно для создания или изменения дуги
        /// </summary>
        /// <param name="sw">Сем. сеть</param>
        /// <param name="title">Заголовок окна</param>
        /// <param name="changeType">тип: 0 - добавление, 1 - изм. from,
        /// 2 - изм. name, 3 - изм. to</param>
        /// <param name="fromID"> </param>
        /// <param name="arcName"> </param>
        /// <param name="toID"> </param>
        public AddChangeArc(SemanticWeb sw, string title, int changeType,
            int fromID = 0, string arcName = "#MetaRelations", int toID = 0)
        {
            _sw = sw;
            InitializeComponent();

            cbFrom.ItemsSource = _sw.Nodes;

            cbTo.ItemsSource = _sw.Nodes;

            //if (cbFrom.Items.Count > 0) cbFrom.SelectedIndex = 0;
            //if (cbName.Items.Count > 0) cbName.SelectedIndex = 0;
            //if (cbTo.Items.Count > 0) cbTo.SelectedIndex = 0;

            Title = title;

            switch (changeType)
            {
                case 1:
                    cbName.IsEnabled = false;
                    cbTo.IsEnabled = false;
                    break;
                case 2: 
                    cbFrom.IsEnabled = false;
                    cbTo.IsEnabled = false;
                    break;
                case 3: 
                    cbFrom.IsEnabled = false;
                    cbName.IsEnabled = false;
                    break;
            }
        } 

        static AddChangeArc()
        {
            Add = new RoutedUICommand("Add", "Add", typeof(AddChangeArc));
        }

        
        #endregion

        #region Добавление
        public static RoutedUICommand Add { get; private set; }

        public void AddExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OnEventAddAgain(((Node)cbFrom.SelectedItem).ID, cbName.SelectedItem.ToString(),
               ((Node) cbTo.SelectedItem).ID);
        } 
        #endregion

        #region Событие при добавлении

        public event Action<int, string, int> EventAddAgain;

        public void OnEventAddAgain(int arg1, string arg2, int arg3)
        {
            Action<int, string, int> handler = EventAddAgain;
            if (handler != null) handler(arg1, arg2, arg3);
        }

        #endregion

        #region ОК/Отмена
        private void BtnCloseClick1(object sender, RoutedEventArgs e)
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
        #endregion

        private void CbFromSelected1(object sender, RoutedEventArgs e)
        {
            cbName.ItemsSource = _sw.GetAllowedArcNames(((Node) cbFrom.SelectedItem).ID);
            if (cbName.Items.Count > 0) cbName.SelectedIndex = 0;
        }
    }
}
