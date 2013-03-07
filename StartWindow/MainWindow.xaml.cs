using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using EditingForms;
using Kernel;
using Microsoft.Win32;
using textMindFusion;
namespace StartWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Переменные
        /// <summary>
        /// Имя файла без пути, но с расширением
        /// </summary>
        private string _fileName = string.Empty;
        /// <summary>
        /// Имя файла без пути, с расширением, имеет вид "~" + _fileName
        /// </summary>
        private string _copyFileName = string.Empty;
        /// <summary>
        /// Путь к файлу
        /// </summary>
        private string _filePath = string.Empty;
        private bool _somethingChanged;
        /// <summary>
        /// При создании файл имеет такое имя
        /// </summary>
        private const string NewFileName = "NewSW.xml";
        /// <summary>
        /// Расширение файла по умолчанию
        /// </summary>
        private const string DefaultExtension = ".xml";
        private SemanticWeb _sw; 

        #endregion

        #region Инициализация
        public MainWindow()
        {
            InitializeComponent();
        }

        static MainWindow()
        {
            EditWithForms = new RoutedUICommand("EditWithForms", "EditWithForms", typeof(MainWindow));
           // EditVisual = new RoutedUICommand("EditVisual", "EditVisual", typeof(MainWindow));
            Consult = new RoutedUICommand("Consult", "Consult", typeof(MainWindow));
        }

        public void ChangeInformation(string name, string path)
        {
            tbName.Text = name;
            tbPath.Text = path;
        } 
        #endregion

        #region Файл
        #region Создать

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //если в данный момент что-то открыто, это надо закрыть
            if (_copyFileName != string.Empty)
                ApplicationCommands.Close.Execute(null, null);
            //создаём файл NewSW.xml
            _copyFileName = NewFileName;
            _fileName = NewFileName;
           File.Create(_copyFileName);
            ChangeInformation(Path.GetFileNameWithoutExtension(_copyFileName),
                Path.GetPathRoot(_copyFileName));
            NormalizeStatusBar();
        }

        #endregion

        #region Открыть

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //если в данный момент что-то открыто, это надо закрыть
            if (_copyFileName != string.Empty)
                ApplicationCommands.Close.Execute(null, null);
            var ofd = new OpenFileDialog { Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" };
            if (ofd.ShowDialog() != true) return;
            var fullName = ofd.FileName;
            _fileName = Path.GetFileName(fullName);
            _copyFileName = "~" + _fileName;
            _filePath = Path.GetDirectoryName(fullName) + "\\";
            File.Copy(fullName, _filePath + _copyFileName, true);
            NormalizeStatusBar();
            ChangeInformation(Path.GetFileNameWithoutExtension(_fileName), _filePath);
        }

        #endregion

        #region Сохранить

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Если мы ещё ни разу не сохранялись, то идём в SaveAs
            if (_fileName == NewFileName)
                ApplicationCommands.SaveAs.Execute(null, null);
            else
            {
                File.Copy(_filePath + _copyFileName,
                    _filePath + _fileName, true);
                _somethingChanged = false;
            }
        }

        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Сохранять можно, если открыт какой-нибудь файл и что-то менялось
            e.CanExecute = _copyFileName != string.Empty && _somethingChanged;
        }

        #endregion

        #region Сохранить как
        private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog { FileName = _fileName, AddExtension = true, DefaultExt = DefaultExtension };
            if (saveDialog.ShowDialog() != true) return;

            //копируем файл копии бд из директории по умолчанию в выбранную директорию
            string oldPath = _filePath, oldName = _fileName, copyOldName = _copyFileName;
            _filePath = string.Format("{0}\\", Path.GetDirectoryName(saveDialog.FileName));
            _fileName = Path.GetFileName(saveDialog.FileName);
            File.Copy(string.Format("{0}{1}", oldPath, copyOldName),
                string.Format("{0}{1}", _filePath, _fileName), true);

            //создаем еще одну копию
            _copyFileName = "~" + _fileName;
            File.Copy(_filePath + _fileName, _filePath + _copyFileName, true);
            //удаляем все файлы из директории по умолчанию
            if (File.Exists(oldName))
                File.Delete(oldName);
            if (File.Exists(copyOldName))
                File.Delete(copyOldName);
            ChangeInformation(_fileName, _filePath);
            _somethingChanged = false;

        }
        #endregion

        #region Закрыть
        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_somethingChanged)
            {
                var res = MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Yes)
                    ApplicationCommands.Save.Execute(null, null);
                if (res == MessageBoxResult.Cancel) return;
            }
            
            //удаляем файл копии
            if (File.Exists(string.Format("{0}{1}", _filePath, _copyFileName)))
                File.Delete(string.Format("{0}{1}", _filePath, _copyFileName));
            _fileName = string.Empty;
            _copyFileName = string.Empty;
            _filePath = string.Empty;
            _somethingChanged = false;
            ChangeInformation(_fileName, _filePath);
        }

        private void CanExecuteIfIsOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            //Можно выполнять команду, если открыт файл
            e.CanExecute = _copyFileName != string.Empty;
        }


        private void WindowClosing1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplicationCommands.Close.Execute(null, null);
        }
        #endregion 
        #endregion

        #region Редактирование
        #region Редактирование через формочки

        public static RoutedUICommand EditWithForms { get; private set; }

        public void EditWithFormsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            InProgerssStatusBar("Редактирование");
            var f = new EditWindow(_filePath + _copyFileName);
            f.ShowDialog();
            _somethingChanged = true;
            NormalizeStatusBar();
        }

        #endregion

        #region Визуальное редактирование

        //public static RoutedUICommand EditVisual { get; private set; }

        //public void EditVisualExecuted(object sender, ExecutedRoutedEventArgs e)
        //{
        //    var f = new textMindFusion.MainWindow(_filePath + _copyFileName);
        //    f.ShowDialog();
        //    _somethingChanged = true;
        //}

        #endregion 
        #endregion

        #region Консультация

        public static RoutedUICommand Consult { get; private set; }

        private void ConsultExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //проверка сем. сети
            _sw = SemanticWeb.ReadFromXml(_filePath + _copyFileName);
            var checker = new Verification(_sw);
            checker.Verificate();
            if (!checker.NoErros)
            {
                ErrorStatusBar("Сем. сеть содержит ошибки. Консультация невозможна.");
                return;
            }
            var w = new ConsultingWindow.MainWindow {Sw = _sw};
            w.ShowDialog();
        }

        private void ConsultCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _copyFileName != string.Empty;
        }

        #endregion

        #region StatusBar

        private void NormalizeStatusBar()
        {
            sbState.Background =
               new LinearGradientBrush(new GradientStopCollection
                                            {
                                                new GradientStop(Colors.LightCyan, 0),
                                                new GradientStop(Colors.White, 0.7),
                                                new GradientStop(Colors.LightCyan, 1)
                                            });
            lblState.Content = "Готово";
        }

        private void InProgerssStatusBar(string text)
        {
            sbState.Background =
                new LinearGradientBrush(new GradientStopCollection
                                            {
                                                new GradientStop(Colors.LightGreen, 0),
                                                new GradientStop(Colors.White, 0.7),
                                                new GradientStop(Colors.LightGreen, 1)
                                            });
            lblState.Content = text;
        }

        private void ErrorStatusBar(string text)
        {
            sbState.Background =
               new LinearGradientBrush(new GradientStopCollection
                                            {
                                                new GradientStop(Colors.Pink, 0),
                                                new GradientStop(Colors.White, 0.7),
                                                new GradientStop(Colors.Pink, 1)
                                            });
            lblState.Content = text;
        }

        #endregion
    }
}
