using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using KnowledgeAcquisitionComponent;
using Microsoft.Win32;
using Kernel;
using System;
using ConsultingWindow;
using MindFusion.Diagramming.Wpf;
using Validation = Kernel.Validation;

namespace textMindFusion
{
    /// <summary>
    /// Interaction logic for ConsultingWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Private Variables
        private readonly Validation _validation;
        private readonly List<string> _messages = new List<string>();

        private string FileName
        {
            get { return MyDiag.FileName; }
            set { MyDiag.FileName = value; }
        }

        private bool _isOpen;
        private const string DefaultExtension = ".xml";
        #endregion

        #region Initialization
        #region Ctor
        public MainWindow()
        {
            InitializeComponent();
            _validation = new Validation();
            _validation.ValidationFinished += OnValidationFinished;
            ListBoxValidation.ItemsSource = _validation.Errors;

            
            ListBoxLog.ItemsSource = MyDiag.Messages.Union(_messages);
            MyDiag.FileName = string.Empty;
            MyDiag.CanEdit = checkBoxIsEdit.IsChecked == true;
            MyDiag.Load = true;
        }
        
        #endregion

        #region OnValidationFinished event handler

        private void OnValidationFinished(int i)
        {
            if (Dispatcher.CheckAccess())
            {
                expanderErrList.Header = "Ошибок: " + i;
                ListBoxValidation.ItemsSource = _validation.Errors;
            }
            else
            {
                Action<int> d = OnValidationFinished;
                Dispatcher.Invoke(d, new object[] {i});
            }
        }

        #endregion

        #region Commands initialization
        static MainWindow()
        {
            LoadData = new RoutedUICommand("LoadData", "LoadData", typeof(MainWindow));
            LoadData.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            Consult = new RoutedUICommand("Consult", "Consult", typeof(MainWindow));
            Consult.InputGestures.Add(new KeyGesture(Key.F5));
            Cancel = new RoutedUICommand("Cancel", "Cancel", typeof(MainWindow));
            LoadDemo = new RoutedUICommand("Загрузить демо", "LoadDemo", typeof(MainWindow));
            LoadDemo.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));
            ZoomInCommand = new RoutedUICommand("ZoomIn", "ZoomIn", typeof(MainWindow));
            ZoomInCommand.InputGestures.Add(new KeyGesture(Key.Add, ModifierKeys.Control));
            ZoomOutCommand = new RoutedUICommand("ZoomOut", "ZoomOut", typeof(MainWindow));
            ZoomOutCommand.InputGestures.Add(new KeyGesture(Key.Subtract, ModifierKeys.Control));
            FitSizeCommand = new RoutedUICommand("FitSize", "FitSize", typeof(MainWindow));
            NoZoomCommand = new RoutedUICommand("NoZoom", "NoZoom", typeof(MainWindow));
        } 
        #endregion
        #endregion

        #region Cancel Command
        /// <summary>
        /// Отмена изменений
        /// </summary>
        public static RoutedUICommand Cancel { get; private set; }

        private void CancelCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyDiag.Reload= true; MyDiag.Load = true;
            SemanticWeb.ReadFromXml(FileName);
            MyDiag.PrintGraph(SemanticWeb.Web());
            MyDiag.Reload = false; MyDiag.Load = false;
            _messages.Add("изменения отменены");
        }

        private void CancelCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SemanticWeb.IsChanged;
        }
        #endregion

        #region Zoom-Panel
        #region ZoomIn
        public static RoutedUICommand ZoomInCommand { get; private set; }

        private void ZoomInExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyDiag.ZoomFactor = Math.Min(1000, MyDiag.ZoomFactor + 10);
        }

        private void ZoomInCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        } 
        #endregion

        #region ZoomOut
        public static RoutedUICommand ZoomOutCommand { get; private set; }

        private void ZoomOutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyDiag.ZoomFactor = Math.Max(20, MyDiag.ZoomFactor - 10);
        }

        private void ZoomOutCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        #endregion

        #region FitSize
        public static RoutedUICommand FitSizeCommand { get; private set; }

        private void FitSizeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyDiag.FitSize();
        }

        private void FitSizeCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        #endregion

        #region NoZoom
        public static RoutedUICommand NoZoomCommand { get; private set; }

        private void NoZoomExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyDiag.ZoomFactor = 100;
        }

        private void NoZoomCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        #endregion
        #endregion 

        #region Режим редактирования/просмотра
        /// <summary>
        /// Изменение режима редактирования/просмотра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsEditClick(object sender, RoutedEventArgs e)
        {
            MyDiag.Behavior = checkBoxIsEdit.IsChecked == true ? Behavior.DrawLinks : Behavior.Pan;
            MyDiag.CanEdit = checkBoxIsEdit.IsChecked == true;
        } 
        #endregion

        #region Главное Меню

        #region Файл
        #region Создать

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //если в данный момент что-то открыто, это надо закрыть
            if (_isOpen)
                ApplicationCommands.Close.Execute(null, null);
            _isOpen = true;
            MyDiag.PrintGraph(SemanticWeb.Web());
            MyDiag.IsEnabled = true;
            //ChangeTopMenuNode();
            zoomInButton.IsEnabled = zoomOutButton.IsEnabled = fitButton.IsEnabled = noZoomButton.IsEnabled = true;
        }

        #endregion

        #region Открыть

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_isOpen)
                ApplicationCommands.Close.Execute(null, null);
            var ofd = new OpenFileDialog { Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" };
            if (ofd.ShowDialog() != true) return;
            FileName = ofd.FileName;
            SemanticWeb.ReadFromXml(FileName);
            MyDiag.PrintGraph(SemanticWeb.Web());
            MyDiag.IsEnabled = true;
            _isOpen = true;
            //ChangeTopMenuNode();
            zoomInButton.IsEnabled = zoomOutButton.IsEnabled = fitButton.IsEnabled = noZoomButton.IsEnabled = true;
        }

        public static RoutedUICommand LoadDemo { get; private set; }

        public void LoadDemoExecuted(object sender, RoutedEventArgs e)
        {
            if (_isOpen)
                ApplicationCommands.Close.Execute(null, null);
            FileName = @"demo.xml";
            SemanticWeb.ReadFromXml(FileName);
            MyDiag.PrintGraph(SemanticWeb.Web());
            MyDiag.IsEnabled = true;
            _isOpen = true;
            //ChangeTopMenuNode();
            zoomInButton.IsEnabled = zoomOutButton.IsEnabled = fitButton.IsEnabled = noZoomButton.IsEnabled = true;
        }

        #endregion

        #region Сохранить

        private void BeforeSaving()
        {
            if (!_validation.NoErrors)
            {
                if (MessageBox.Show("Обнаружены ошибки в сети.\n Для просмотра ошибок загляните в отчёты", "Внимание!", 
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }
            MyDiag.SaveCoordinates();
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Если мы ещё ни разу не сохранялись, то идём в SaveAs
            if (FileName == string.Empty)
                ApplicationCommands.SaveAs.Execute(null, null);
            else
            {
                BeforeSaving();
                SemanticWeb.WriteToXml(FileName);
            }
        }

        
        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //Сохранять можно, если открыт какой-нибудь файл и что-то менялось
            e.CanExecute = _isOpen /*&& _somethingChanged*/;
        }

        #endregion

        #region Сохранить как
        private void SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            BeforeSaving();
            var saveDialog = new SaveFileDialog { FileName = FileName, AddExtension = true, DefaultExt = DefaultExtension,
            Filter = "xml documents|.xml"};
            if (saveDialog.ShowDialog() != true) return;
            FileName = saveDialog.FileName;
            SemanticWeb.WriteToXml(FileName);
        }
        #endregion

        #region Закрыть
        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (SemanticWeb.IsChanged)
            {
                var res = MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Yes)
                    ApplicationCommands.Save.Execute(null, null);
                if (res == MessageBoxResult.Cancel) return;
            }
            MyDiag.IsEnabled = false;
            MyDiag.ClearAll();
            _isOpen = false;
            FileName = string.Empty;
            SemanticWeb.Close();
        }

        private void CanExecuteIfIsOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            //Можно выполнять команду, если открыт файл
            e.CanExecute = _isOpen;
        }


        private void WindowClosing1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplicationCommands.Close.Execute(null, null);
        }
        #endregion
        #endregion

        #region Загрузка данных

        public static RoutedUICommand LoadData { get; private set; }

        public void EditWithFormsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveExecuted(null, null);
            var f = new KacWindow();
            f.ShowDialog();
            MyDiag.PrintGraph(SemanticWeb.Web());
        }

        #endregion

        #region Консультация

        public static RoutedUICommand Consult { get; private set; }

        private void ConsultExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (!_validation.NoErrors)
            {
                MessageBox.Show("Сем. сеть содержит ошибки. Консультация невозможна.");
                return;
            }
            var w = new ConsWindow();
            w.Show();
        }

        private void ConsultCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _isOpen;
        }

        #endregion

        #endregion

        #region Верхняя менюшка по вершинам:добавить/изменить/удалить
        private void AddNodeButtonClick(object sender, RoutedEventArgs e)
        {
            MyDiag.CreateNode("", 0, 0);
        }

        private void ChangeNodeButtonClick(object sender, RoutedEventArgs e)
        {
            if (MyDiag.SelectedNodes.Count == 1)
                MyDiag.DdNodeTextEditing(new object(), new NodeValidationEventArgs(MyDiag.SelectedNodes[0]));
        }

        private void DeleteNodeButtonClick(object sender, RoutedEventArgs e)
        {
            MyDiag.DeleteSelectedNodes();
        }
        #endregion

        #region Closing
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _validation.Dispose();
        } 
        #endregion

    }
}