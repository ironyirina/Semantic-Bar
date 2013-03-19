using System.Windows.Input;
using Kernel;
using Microsoft.Win32;

namespace KnowledgeAcquisitionComponent
{
    /// <summary>
    /// Interaction logic for KacWindow.xaml
    /// </summary>
    public partial class KacWindow
    {
        private string _fileName;// = @"C:\Users\Ирина\Documents\Чуприна\SemanticWeb\SemanticWeb\KnowledgeAcquisitionComponent\Bellini.txt";

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
            var loader = new Loader(_fileName, SendReport);
            loader.Load();
            Close();
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
