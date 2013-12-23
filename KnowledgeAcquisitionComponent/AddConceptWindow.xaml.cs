using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Consulting;
using Kernel;

namespace KnowledgeAcquisitionComponent
{
    /// <summary>
    /// Interaction logic for AddConceptWindow.xaml
    /// </summary>
    public partial class AddConceptWindow
    {
        #region Переменные
        public string ConceptName { get; set; }
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
            var searcher = new Searcher(null, true, true);
            twIsA.Items.Add(MetadataSearch.AddInstancesOfMetaObject((string)cbTypes.SelectedItem));
        }

        private void WindowLoaded1(object sender, RoutedEventArgs e)
        {
            tbName.Text = ConceptName;
            cbTypes.ItemsSource = SemanticWeb.Web().GetAllMetaObjectNames();
            if (cbTypes.Items.Count > 0)
                cbTypes.SelectedIndex = 0;
        } 
        #endregion

        #region Добавление понятия
        public static RoutedUICommand AddConcept { get; set; }

        private void AddConceptExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (rbSubClass.IsChecked == true)
                    AddInstance("#is_a");
                if (rbInstance.IsChecked == true)
                    AddInstance("#is_instance");
                else if (rbSynonym.IsChecked == true)
                    AddSynonym();
                else if (rbWordForm.IsChecked == true)
                    AddWordForm();
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

        private void AddInstance(string arcName)
        {
            //создаём неименованный узел для нового понятия
            var newUnnamedNode = SemanticWeb.Web().AddNode(string.Empty);
            //создаём именованный узел для нового понятия
            var newNamedNode = SemanticWeb.Web().AddNode(tbName.Text);
            //дуга Name
            SemanticWeb.Web().AddArc(newUnnamedNode.ID, "#Name", newNamedNode.ID);
            //ищем неименованную вершину для родительской сущности
            var parentUnnamedNode = SemanticWeb.Web().GetUnnamedNodesForName(((TreeViewItem)twIsA.SelectedItem).Header.ToString());
            //проводим дугу arcName
            SemanticWeb.Web().AddArc(newUnnamedNode.ID, arcName, parentUnnamedNode.ID);
        }

        private void AddSynonym()
        {
            var parentNamedNode = SemanticWeb.Web().Mota(SemanticWeb.Web().Atom(((TreeViewItem)twIsA.SelectedItem).Header.ToString()));
            parentNamedNode.AddSynonym(tbName.Text);
        }

        private void AddWordForm()
        {
            var name = (((TreeViewItem) twIsA.SelectedItem).Header.ToString());
            var parentNamedNode = SemanticWeb.Web().Mota(SemanticWeb.Web().Atom(name));
            parentNamedNode.AddWordForm(name, tbName.Text);
        }
        #endregion
    }
}
