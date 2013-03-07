using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Kernel;

namespace EditingForms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class EditWindow
    {
        #region Переменные
        private SemanticWeb _myWeb;
        private readonly string _fileName;
        private bool _somethingChanged;
        private Node _selectedNode;
        private Arc _selectedArc;

        #endregion

        #region Инициализация
        public EditWindow(string fileName)
        {
            _fileName = fileName;
            InitializeComponent();
            NormalizeStatusBar();
            try
            {
                _myWeb = SemanticWeb.ReadFromXml(_fileName);
            }
            catch
            {
                _myWeb = new SemanticWeb();
            }
            SetDataContext();
        }

        private void SetDataContext()
        {
            lvNodes.DataContext = _myWeb.Nodes;
            lvArcs.DataContext = _myWeb.Arcs;
        }

        private void RefreshAll()
        {
            lvNodes.Items.Refresh();
            lvArcs.Items.Refresh();
            lvAllNodes.Items.Refresh();
            lvAllArcs.Items.Refresh();
        }

        static EditWindow()
        {
            Apply = new RoutedUICommand("Apply", "Apply", typeof(EditWindow));
            Cancel = new RoutedUICommand("Cancel", "Cancel", typeof(EditWindow));
            AddNode = new RoutedUICommand("AddNode", "AddNode", typeof(EditWindow));
            ChangeNode = new RoutedUICommand("ChangeNode", "ChangeNode", typeof(EditWindow));
            DeleteNode = new RoutedUICommand("DeleteNode", "DeleteNode", typeof(EditWindow));
            AddArc = new RoutedUICommand("AddArc", "AddArc", typeof(EditWindow));
            ChangeArcFrom = new RoutedUICommand("ChangeArcFrom", "ChangeArcFrom", typeof(EditWindow));
            ChangeArcName = new RoutedUICommand("ChangeArcName", "ChangeArcName", typeof(EditWindow));
            ChangeArcTo = new RoutedUICommand("ChangeArcTo", "ChangeArcTo", typeof(EditWindow));
            DeleteArc = new RoutedUICommand("DeleteArc", "DeleteArc", typeof(EditWindow));
        }
        #endregion

        #region Применить/Отменить/Закрыть

        #region Применить
        public static RoutedUICommand Apply { get; private set; }

        private void ApplyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //сначала проверяем правильность сем. сети
            var checker = new Verification(_myWeb);
            Cursor = Cursors.Wait;
            checker.Verificate();
            lbErrors.DataContext = checker.Errors;
            Cursor = Cursors.Arrow;
            if (!checker.NoErros)
            {
                var dRes = MessageBox.Show("Сеть содержит ошибки. Всё равно сохранить?", "",
                    MessageBoxButton.YesNo);
                if (dRes == MessageBoxResult.No) return;
            }
            SemanticWeb.WriteToXml(_fileName, _myWeb);
            SetDataContext();
            _somethingChanged = false;
            NormalizeStatusBar();
        }

        private void ApplyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _somethingChanged;
        }
        #endregion

        #region Отменить
        public static RoutedUICommand Cancel { get; private set; }
        private void CancelExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _myWeb = SemanticWeb.ReadFromXml(_fileName);
            SetDataContext();
            var checker = new Verification(_myWeb);
            Cursor = Cursors.Wait;
            checker.Verificate();
            lbErrors.DataContext = checker.Errors;
            Cursor = Cursors.Arrow;
            _somethingChanged = false;
        }
        private void CancelCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _somethingChanged;
        }
        #endregion

        #region Закрыть
        private void WindowClosing1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_somethingChanged) return;
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

        private void BtnCloseClick1(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #endregion

        #region Вершины

        #region Добавить

        public static RoutedUICommand AddNode { get; private set; }

        public void AddExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            InProgerssStatusBar("Добавление вершины");
            var f = new AddChangeNode("Добавление вершины", "Имя вершины", "") {AllowManyAddings = true};
            f.EventAddAgain += NodeEventHandler;
            f.ShowDialog();
            f.EventAddAgain -= NodeEventHandler;
            NormalizeStatusBar();
        }

        private void NodeEventHandler(string s)
        {
            try
            {
                _myWeb.AddNode(s);
                SetDataContext();
                RefreshAll();
                _somethingChanged = true;
                InProgerssStatusBar("Добавление вершины");
            }
            catch (Exception ex)
            {
                ErrorStatusBar(ex.Message);
            }
        }
        #endregion

        #region Изменить

        public static RoutedUICommand ChangeNode { get; private set; }

        public void ChangeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            InProgerssStatusBar("Изменение вершины");
            var f = new AddChangeNode("Изменение вершины", "Имя вершины:", _selectedNode.Name)
                {AllowManyAddings = false};
            if (f.ShowDialog() != true)
            {
                NormalizeStatusBar();
                return;
            }
            try
            {
                _myWeb.ChangeNodeName(_selectedNode.ID, f.NodeName);
                SetDataContext();
                RefreshAll();
                _somethingChanged = true;
                NormalizeStatusBar();
            }
            catch (ArgumentException ex)
            {
                ErrorStatusBar(ex.Message);
            }
        }

        public void ChangeAndDeleteCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lvNodes.SelectedItem != null;
        }

        #endregion

        #region Удалить

        public static RoutedUICommand DeleteNode { get; private set; }

        public void DeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _myWeb.DeleteNode(_selectedNode);
                SetDataContext();
                RefreshAll();
                _somethingChanged = true;
                NormalizeStatusBar();
            }
            catch (ArgumentException ex)
            {
                ErrorStatusBar(ex.Message);
            }
        }

        #endregion

        private void LvNodesSelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (lvNodes.SelectedItem != null)
                _selectedNode = (Node) lvNodes.SelectedItem;
        }
        #endregion

        #region Дуги

        #region Добавить

        public static RoutedUICommand AddArc { get; private set; }

        public void AddArcExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var f = new AddChangeArc(_myWeb, "Добавление дуги", 0) {AllowManyAddings = true};
            f.EventAddAgain += ArcEventHandler;
            f.ShowDialog();
            f.EventAddAgain -= ArcEventHandler;
        }

        private void ArcEventHandler(int fromID, string arcName, int toID)
        {
            try
            {
                _myWeb.AddArc(fromID, arcName, toID);
                SetDataContext();
                RefreshAll();
                _somethingChanged = true;
                NormalizeStatusBar();
            }
            catch (Exception ex)
            {
                ErrorStatusBar(ex.Message);
            }
        }
        #endregion

        #region Изменить

        public static RoutedUICommand ChangeArcFrom { get; private set; }

        public void ChangeArcFromExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var f = new AddChangeArc(_myWeb, "Изменение вершины, откуда выходит дуга", 1)
                        {
                            AllowManyAddings = false,
                            From = _selectedArc.From,
                            ArcName = _selectedArc.Name,
                            To = _selectedArc.To
                        };
            if (f.ShowDialog() != true) return;
            try
            {
                _myWeb.ChangeArcDirectionFrom(_selectedArc.From.ID, f.From.ID,
                    _selectedArc.Name,
                    _selectedArc.To.ID);
                SetDataContext();
                RefreshAll();
                _somethingChanged = true;
                NormalizeStatusBar();
            }
            catch (ArgumentException ex)
            {
                ErrorStatusBar(ex.Message);
            }
        }

        public static RoutedUICommand ChangeArcName { get; private set; }

        public void ChangeArcNameExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var f = new AddChangeArc(_myWeb, "Изменение имени дуги", 2)
            {
                AllowManyAddings = false,
                From = _selectedArc.From,
                ArcName = _selectedArc.Name,
                To = _selectedArc.To
            };
            if (f.ShowDialog() != true) return;
            try
            {
                _myWeb.ChangeArcName(_selectedArc.From.ID, _selectedArc.Name, 
                    f.ArcName, _selectedArc.To.ID);
                SetDataContext();
                RefreshAll();
                _somethingChanged = true;
                NormalizeStatusBar();
            }
            catch (ArgumentException ex)
            {
                ErrorStatusBar(ex.Message);
            }
        }

        public static RoutedUICommand ChangeArcTo { get; private set; }

        public void ChangeArcToExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var f = new AddChangeArc(_myWeb, "Изменение вершины, откуда выходит дуга", 3)
            {
                AllowManyAddings = false,
                From = _selectedArc.From,
                ArcName = _selectedArc.Name,
                To = _selectedArc.To
            };
            if (f.ShowDialog() != true) return;
            try
            {
                _myWeb.ChangeArcDirectionTo(_selectedArc.From.ID, _selectedArc.Name,
                    _selectedArc.To.ID, f.To.ID);
                SetDataContext();
                RefreshAll();
                _somethingChanged = true;
                NormalizeStatusBar();
            }
            catch (ArgumentException ex)
            {
                ErrorStatusBar(ex.Message);
            }
        }

        public void ChangeAndDeleteArcCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = lvArcs.SelectedItem != null;
        }

        #endregion

        #region Удалить

        public static RoutedUICommand DeleteArc { get; private set; }

        public void DeleteArcExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _myWeb.DeleteArc(_selectedArc.From.ID, _selectedArc.Name, _selectedArc.To.ID);
                SetDataContext();
                RefreshAll();
                _somethingChanged = true;
                NormalizeStatusBar();
            }
            catch (ArgumentException ex)
            {
                ErrorStatusBar(ex.Message);
            }
        }

        #endregion

        private void LvArcsSelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (lvArcs.SelectedItem != null)
                _selectedArc = (Arc) lvArcs.SelectedItem;
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

