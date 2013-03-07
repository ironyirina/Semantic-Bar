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
using Consulting;
using Kernel;

namespace KnowledgeAcquisitionComponent
{
    /// <summary>
    /// Interaction logic for AddConceptWindow.xaml
    /// </summary>
    public partial class AddConceptWindow : Window
    {
        #region Переменные
        public string ConceptName { get; set; }

        public SemanticWeb SW { get; set; } 
        #endregion

        #region Инициализация + интерфейсик
        public AddConceptWindow()
        {
            InitializeComponent();
        }

        static AddConceptWindow()
        {
            AddConcept = new RoutedUICommand("AddConcept", "AddConcept", typeof(AddConceptWindow));
        }

        private void BtnCloseClick1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CbTypesSelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            twIsA.Items.Clear();
            var searcher = new Searcher(SW);
            twIsA.Items.Add(searcher.AddSubClassesOfMetaObject((string)cbTypes.SelectedItem));
        }

        private void WindowLoaded1(object sender, RoutedEventArgs e)
        {
            tbName.Text = ConceptName;
            cbTypes.ItemsSource = SW.GetAllMetaObjectNames();
            if (cbTypes.Items.Count > 0)
                cbTypes.SelectedIndex = 0;
        } 
        #endregion

        public static RoutedUICommand AddConcept { get; set; }
        
        private void AddConceptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //создаём неименованный узел для нового понятия
                var newUnnamedNode = SW.AddNode(string.Empty);
                //создаём именованный узел для нового понятия
                var newNamedNode = SW.AddNode(tbName.Text);
                //дуга Name
                SW.AddArc(newUnnamedNode.ID, "#Name", newNamedNode.ID);
                //ищем неименованную вершину для родительской сущности
                var parentUnnamedNode = SW.GetUnnamedNodeForName(((TreeViewItem) twIsA.SelectedItem).Header.ToString());
                //проводим дугу is_instance
                SW.AddArc(newUnnamedNode.ID, "#is_instance", parentUnnamedNode.ID);
                Close();
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void AddConceptCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tbName.Text != string.Empty && twIsA.SelectedItem != null;
        }
    }
}
