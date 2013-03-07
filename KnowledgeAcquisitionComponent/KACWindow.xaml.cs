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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kernel;
using Microsoft.Win32;

namespace KnowledgeAcquisitionComponent
{
    /// <summary>
    /// Interaction logic for KacWindow.xaml
    /// </summary>
    public partial class KacWindow : Window
    {
        private string _fileName;// = @"C:\Users\Ирина\Documents\Чуприна\SemanticWeb\SemanticWeb\KnowledgeAcquisitionComponent\Bellini.txt";

        public SemanticWeb SW { get; set; }

        #region Инициализация
        public KacWindow()
        {
            InitializeComponent();
            tbPath.Text = _fileName;
        }

        static KacWindow()
        {
            Select = new RoutedUICommand("Select", "Select", typeof(KacWindow));
            Load = new RoutedUICommand("Load", "Load", typeof(KacWindow));
        } 
        #endregion

        #region Select

        public static RoutedUICommand Select { get; private set; }

        private void SelectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var d = new OpenFileDialog {Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"};
            if (d.ShowDialog() != true) return;
            _fileName = d.FileName;
            tbPath.Text = _fileName;
        }

        #endregion

        #region Load

        public static RoutedUICommand Load { get; private set; }

        private void LoadExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var loader = new Loader(_fileName, SendReport) {SW = SW};
            loader.Load();
            //Close();
        }

        private void SendReport(string s)
        {
            tblockLog.Text += s + "\n";
        }

        private void LoadCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = tbPath.Text != string.Empty;
        }

        #endregion
    }
}
