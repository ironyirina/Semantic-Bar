using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using KnowledgeAcquisitionComponent;
using Microsoft.Win32;
using MindFusion.Diagramming.Wpf;
using MindFusion.Diagramming.Wpf.Layout;
using Kernel;
using System.Windows.Controls;
using System;
using SynonymEditor;
using MouseButton = MindFusion.Diagramming.Wpf.MouseButton;
using ConsultingWindow;
using Validation = Kernel.Validation;

namespace textMindFusion
{
    /// <summary>
    /// Interaction logic for ConsultingWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Private Variables
        readonly List<DiagramNode> _selectNode = new List<DiagramNode>();

        private const int NodeHeight = 50;
        private const int NodeWidth = 50;
        bool _load = true;
        bool _reload;

        private readonly Validation _validation;
        #endregion

        #region Initialization
        #region Ctor
        public MainWindow()
        {
            InitializeComponent();

            DD.DefaultShape = Shapes.Ellipse;
            DD.LinkHeadShape = ArrowHeads.PointerArrow;
            DD.LinkHeadShapeSize = 10;
            DD.IsEnabled = false;
            _validation = new Validation();
            _validation.ValidationFinished += OnValidationFinished;
            ListBoxValidation.ItemsSource = _validation.Errors;
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

        #region  Служебные: отправка письма, проверка имени, вывод графа

        private void SendMessage(string message)
        {
            if (!_load && !_reload)
            {
                ListBoxLog.Items.Add(message);
            }
        }

        private void PrintGraph(SemanticWeb web)
        {
            _load = true;
            _reload = true;
            DD.ClearAll();
            var nodeMap = new Dictionary<int, DiagramNode>();
            foreach (var node in web.Nodes)
            {
                var bounds = new Rect(new Point(node.X, node.Y), new Point(node.X + NodeWidth, node.Y + NodeHeight));
                var diagramNode = DD.Factory.CreateShapeNode(bounds);
                //diagramNode.Brush = new LinearGradientBrush(new GradientStopCollection
                nodeMap[node.ID] = diagramNode;
                diagramNode.Text = node.Name;
                diagramNode.Tag = node;
            }
            foreach (var arc in web.Arcs)
            {
                var diagramArc =
                    DD.Factory.CreateDiagramLink(nodeMap[arc.From], nodeMap[arc.To]);
                diagramArc.Text = arc.Name;
                diagramArc.Tag = true;
            }
            _load = false;
            _reload = false;
            if (!_validation.NoErrors)
                MessageBox.Show("Обнаружены ошибки в сети.\n Для просмотра ошибок загляните в отчеты", "Внимание!");
        }
        #endregion

        #region Cancel Command
        /// <summary>
        /// Отмена изменений
        /// </summary>
        public static RoutedUICommand Cancel { get; private set; }

        private void CancelCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _reload = true; _load = true;
            SemanticWeb.ReadFromXml(_fileName);
            PrintGraph(SemanticWeb.Web());
            _reload = false; _load = false;
            SendMessage("изменения отменены");
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
            DD.ZoomFactor = Math.Min(1000, DD.ZoomFactor + 10);
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
            DD.ZoomFactor = Math.Max(20, DD.ZoomFactor - 10);
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
            Rect union = Rect.Empty;
            foreach (DiagramNode node in DD.Nodes)
            {
                if (union.IsEmpty)
                    union = node.Bounds;
                else
                    union.Union(node.Bounds);
            }

            union.Inflate(50, 50);

            DD.ZoomToRect(union);
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
            DD.ZoomFactor = 100;
        }

        private void NoZoomCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        #endregion
        #endregion 
        
        #region Вершины
        /// <summary>
        /// Удаление вершины
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdNodeDeleted(object sender, NodeEventArgs e)
        {
            try
            {
                SendMessage("Удаление вершины" + ((Node)e.Node.Tag).Name);
                if (!_reload && !_load )
                    SemanticWeb.Web().DeleteNode((Node)e.Node.Tag);
            }
            catch (ArgumentException e1)
            {
                SendMessage("Вершина " + ((Node)e.Node.Tag).Name + "не может быть удалена");
                MessageBox.Show(e1.Message);
            }
        }

        /// <summary>
        /// Изменение имени вершины
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdNodeTextEditing(object sender, NodeValidationEventArgs e)
        {
            if (checkBoxIsEdit.IsChecked == false)
            {
                e.Cancel = true;
                return;
            }
            if (((Node)e.Node.Tag).IsSystem)
                e.Cancel = true;

            var formName = new TextBoxForm(e.Node.Text);
            formName.ShowDialog();
            try
            {
                if (formName.DialogResult == true)
                {
                    string newName = formName.ReturnValue(); //новое имя вершины
                   /*УЖАСНЫЙ КОСЯК ИЗМЕНЕНИЕ СЕТИ Д.Б. ЗДЕСЬ, А НЕ В КОНЦЕ*/
                    e.Node.Text = newName;
                    SendMessage("Изменение имени вершины на " + e.Node.Text + "  завершилось");
                    SemanticWeb.Web().ChangeNodeName(((Node)e.Node.Tag).ID, newName);
                }
                else
                {
                    SendMessage("отмена изменения имени вершины " + e.Node.Text);
                }
            }
            catch (ArgumentException e1)
            {
                SendMessage("отмена изменения имени вершины: " + e1.Message);
            }
            formName.Close();
            e.Cancel = true;
        }

        /// <summary>
        /// Создание вершины
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdNodeCreated(object sender, NodeEventArgs e)
        {
            if (!_load)
            {
                CreateNode("", e.MousePosition.X, e.MousePosition.Y);
                //Запилила отдельной функцией, ибо было ваще ужасно: одна функция повторялась много раз в коде
            }
        }

        private void CreateNode(string name, double x, double y)
        {
            try
            {
                Node tagNode = SemanticWeb.Web().AddNode("");
                var bounds = new Rect(new Point(x, y), new Point(x + NodeWidth, y + NodeHeight));
                var diagramNode = DD.Factory.CreateShapeNode(bounds);
                diagramNode.Text = name;
                diagramNode.Tag = tagNode;
                DD.Nodes.Add(diagramNode);
                SendMessage("Создание вершины " + name);
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// По двойному клику на пустом месте добавляем вершину
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdDoubleClicked(object sender, DiagramEventArgs e)
        {
            CreateNode("", e.MousePosition.X, e.MousePosition.Y);
        }

        #endregion

        #region Дуги
        /// <summary>
        /// Изменение имени дуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdLinkTextEditing(object sender, LinkValidationEventArgs e)
        {

            if (checkBoxIsEdit.IsChecked==false)
            {
                e.Cancel = true;
                return;
            }
            SendMessage("Изменение типа дуги: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
            var formLink = new ComboBoxForm();
            var listarc = SemanticWeb.Web().GetAllowedArcNames(((Node)e.Link.Origin.Tag).ID);
            formLink.RefreshValue(listarc);
            formLink.ShowDialog();
            try
            {
                if (formLink.DialogResult == true)
                {
                    string newT = formLink.ReturnValue().ToString(); //новое имя вершины выбранное из комбобоксика
                    SemanticWeb.Web().ChangeArcName(((Node)e.Link.Origin.Tag).ID, e.Link.Text, newT, ((Node)e.Destination.Tag).ID);
                    e.Link.Text = newT;
                    SendMessage("Изменение дуги завершилось: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
                else
                {
                    SendMessage("Отмена изменения типа дуги: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                    e.Cancel = true;
                }
            }
            catch (ArgumentException e1)
            {
                SendMessage("Отмена изменения типа дуги с ошибкой: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                MessageBox.Show(e1.Message);
                e.Cancel = true;
            }
            formLink.Close();
            e.Cancel = true;
        }
        
        /// <summary>
        /// Создание дуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdLinkCreated(object sender, LinkEventArgs e)
        {
            try
            {
                var formLink = new ComboBoxForm();
                formLink.RefreshValue(SemanticWeb.Web().GetAllowedArcNames(((Node)e.Link.Origin.Tag).ID));
                formLink.ShowDialog();
                SendMessage("Создание дуги " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                if (formLink.DialogResult == true)
                {
                    e.Link.Text = formLink.ReturnValue().ToString();
                    SemanticWeb.Web().AddArc(((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                    e.Link.Tag = true;
                    SendMessage("Создание дуги завершилось: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
                else
                {
                    DD.Links.Remove(e.Link);
                    SendMessage("Создание дуги отменено: " + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
            }
            catch (ArgumentException e1)
            {
                MessageBox.Show(e1.Message);
                /*произошла ошибка вставки, удаляем дугу из графа*/
                e.Link.Tag = false;
                DD.Links.Remove(e.Link);
                SendMessage("Создание дуги отменено: " + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
            }
        }

        /// <summary>
        /// Удаление дуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdLinkDeleting(object sender, LinkValidationEventArgs e)
        {
            if (_load)
            {
                e.Cancel = true; return;
            }
            if (checkBoxIsEdit.IsChecked == true)
            {
                e.Cancel = true;
            }
        }
        
        /// <summary>
        /// Удаление дуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdLinkDeleted(object sender, LinkEventArgs e)
        {
            //не работает: всегда _load == false
            if (_load)
            {
                return;
            }
            if (checkBoxIsEdit.IsEnabled == false)
                return;
            // DD.Links.Remove()
            if (e.Link.Text != "")
            {
                if ((bool)e.Link.Tag)
                {
                   if (!_load || !_reload)
                       SemanticWeb.Web().DeleteArc(((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                }
                SendMessage("дуга удалена: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
               // _somethingChanged = true;
            }
            // e.Link.Destination - цель
            // e.Link.Origin//откуда
        }

        #region FromID-To дуги
        bool _change; //=true, когда уже сохранили дугу
        readonly DiagramLink _oldLink = new DiagramLink();

        /// <summary>
        /// Изменение дуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdLinkModified(object sender, LinkEventArgs e)
        {
            if (!_load)
            {
                return;
            }
            if (checkBoxIsEdit.IsChecked==false)
                return;
            SendMessage("изменение дуги: " + _oldLink.Text + " от " + _oldLink.Origin.Text + " к " + _oldLink.Destination.Text);
            try
            {
                if (_oldLink.Origin.Text != e.Link.Origin.Text)
                {
                    SemanticWeb.Web().ChangeArcDirectionFrom(((Node)_oldLink.Origin.Tag).ID, ((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                    SendMessage("изменение дуги заврешено: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
                if (_oldLink.Destination.Text != e.Link.Destination.Text)
                {
                    SemanticWeb.Web().ChangeArcDirectionFrom(((Node)_oldLink.Origin.Tag).ID, ((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                    SendMessage("изменение дуги заврешено: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
            }
            catch (ArgumentException e1)
            {
                e.Link.Text = _oldLink.Text;
                e.Link.Origin = _oldLink.Origin;
                e.Link.Destination = _oldLink.Destination;
                MessageBox.Show(e1.Message);
                SendMessage("изменение дуги отменено: " + _oldLink.Text + " от " + _oldLink.Origin.Text + " к " + _oldLink.Destination.Text);
            }
            catch(NullReferenceException e2)
            {
                
            }
        }

        private void DdLinkModifying(object sender, LinkValidationEventArgs e)
        {
            if (!_load)
            {
                e.Cancel = true; return;
            }
            if (checkBoxIsEdit.IsChecked == false)
            {
                e.Cancel = true;
                return;
            }

            if (!_change)
            {
                _oldLink.Destination = e.Link.Destination;
                _oldLink.Origin = e.Link.Origin;
                _oldLink.Text = e.Link.Text;
                _change = true;
            }
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
            DD.Behavior = checkBoxIsEdit.IsChecked == true ? Behavior.DrawLinks : Behavior.Pan;
        } 
        #endregion

        #region Контекстное меню
        #region Контекстное меню для вершины

        DiagramNode _rightNode; //вершина по которой щелкнули правой кнопкой

        private void DdNodeClicked(object sender, NodeEventArgs e)
        {
            myContextMenu.IsEnabled = false;
            myContextMenu.Visibility = Visibility.Collapsed;
            if (e.MouseButton == MouseButton.Right && (checkBoxIsEdit.IsChecked == true))
            {
                myContextMenu.Visibility = Visibility.Visible;
                myContextMenu.IsEnabled = true;
                _rightNode = e.Node;
                myContextMenu.DataContext = MenuOptionsNode;
            }
        }

        public List<MenuItem> MenuOptionsNode
        {
            get
            {
                var items = new List<MenuItem>();
                var del = new MenuItem { Header = "Удалить вершину" };
                del.Click += DelOnClick;
                items.Add(del);
                var change = new MenuItem { Header = "Изменить вершину" };
                change.Click += ChangeOnClick;
                items.Add(change);
                //Изменить синонимы
                //проверка на пустое имя вершины
                if (_rightNode != null && _rightNode.Text != "")
                {
                    var synEdit = new MenuItem { Header = "Синонимы" };
                    synEdit.Click += SynEditOnClick;
                    items.Add(synEdit);
                }
                return items;
            }
        }

        private void SynEditOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var w = new SynWindow(_fileName, ((Node)_rightNode.Tag).Name);
            w.ShowDialog();
            PrintGraph(SemanticWeb.Web());
        }

        private void DelOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            DD.Nodes.Remove(_rightNode);
        }

        private void ChangeOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            DdNodeTextEditing(new object(), new NodeValidationEventArgs(_rightNode));
        }

        #endregion

        #region Контекстное меню для связи
        DiagramLink _rightLink; //связь по которой щелкнули правой кнопкой
        private void DdLinkClicked(object sender, LinkEventArgs e)
        {
            myContextMenu.IsEnabled = false;
            myContextMenu.Visibility = Visibility.Collapsed;

            if (e.MouseButton == MouseButton.Right && checkBoxIsEdit.IsChecked == true)
            {
                _rightLink = e.Link;
                myContextMenu.IsEnabled = true;
                myContextMenu.Visibility = Visibility.Visible;
                myContextMenu.DataContext = MenuOptionsLink;
            }
        }

        public List<MenuItem> MenuOptionsLink
        {
            get
            {
                var items = new List<MenuItem>();
                var del = new MenuItem { Header = "Удалить связь" };
                del.Click += DelLinkOnClick;
                items.Add(del);
                var change = new MenuItem { Header = "Переименовать связь" };
                change.Click += ChangeLinkOnClick;
                items.Add(change);
                return items;
            }
        }

        private void DelLinkOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            DD.Links.Remove(_rightLink);
        }
        private void ChangeLinkOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            DdLinkTextEditing(new object(), new LinkValidationEventArgs(_rightLink));
        }

        #endregion

        #region Контекстное меню для графа
        Point _rightClick; //позиция правого клика мыши по полю
        private void DdClicked(object sender, DiagramEventArgs e)
        {
            myContextMenu.Visibility = Visibility.Collapsed;
            if (e.MouseButton == MouseButton.Right && (checkBoxIsEdit.IsChecked == true))
            {
                _rightClick = e.MousePosition;
                myContextMenu.Visibility = Visibility.Visible;
                myContextMenu.IsEnabled = true;
                myContextMenu.DataContext = MenuOptions;
            }
        }

        public List<MenuItem> MenuOptions
        {
            get
            {
                var items = new List<MenuItem>();
                var addn = new MenuItem { Header = "Добавить веришну" };
                addn.Click += (sender, args) => CreateNode("", _rightClick.X, _rightClick.Y);
                items.Add(addn);

                //var arrange = new MenuItem { Header = "Авторасстановка" };
                //items.Add(arrange);
                return items;
            }
        }

        //private void ArrangeOnClick(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    //НАХЕР ЭТУ ФИГНЮ, АДОВО ТОРМОЗИТ
        //    //НУ ИЛИ ПИЛИ В ОТДЕЛЬНОМ ПОТОКЕ С КНОПОЧКОЙ "ОТМЕНА"
        //    var layout = new LayeredLayout();
        //    layout.Arrange(DD);
        //}

        #endregion 
        #endregion

        #region Главное Меню

        #region Переменные
        private string _fileName = string.Empty;
        private bool _isOpen;
        private const string DefaultExtension = ".xml";
        #endregion

        #region Файл
        #region Создать

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //если в данный момент что-то открыто, это надо закрыть
            if (_isOpen)
                ApplicationCommands.Close.Execute(null, null);
            _isOpen = true;
            PrintGraph(SemanticWeb.Web());
            DD.IsEnabled = true;
            ChangeTopMenuNode();
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
            _fileName = ofd.FileName;
            SemanticWeb.ReadFromXml(_fileName);
            PrintGraph(SemanticWeb.Web());
            DD.IsEnabled = true;
            _isOpen = true;
            ChangeTopMenuNode();
            zoomInButton.IsEnabled = zoomOutButton.IsEnabled = fitButton.IsEnabled = noZoomButton.IsEnabled = true;
        }

        public static RoutedUICommand LoadDemo { get; private set; }

        public void LoadDemoExecuted(object sender, RoutedEventArgs e)
        {
            if (_isOpen)
                ApplicationCommands.Close.Execute(null, null);
            _fileName = @"demo.xml";
            SemanticWeb.ReadFromXml(_fileName);
            PrintGraph(SemanticWeb.Web());
            DD.IsEnabled = true;
            _isOpen = true;
            ChangeTopMenuNode();
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
            foreach (DiagramNode t in DD.Nodes)
            {
                SemanticWeb.Web().ChangeNodeCoordinates(((Node)t.Tag).ID, t.Bounds.X, t.Bounds.Y);
            }
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Если мы ещё ни разу не сохранялись, то идём в SaveAs
            if (_fileName == string.Empty)
                ApplicationCommands.SaveAs.Execute(null, null);
            else
            {
                BeforeSaving();
                SemanticWeb.WriteToXml(_fileName);
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
            var saveDialog = new SaveFileDialog { FileName = _fileName, AddExtension = true, DefaultExt = DefaultExtension,
            Filter = "xml documents|.xml"};
            if (saveDialog.ShowDialog() != true) return;
            _fileName = saveDialog.FileName;
            SemanticWeb.WriteToXml(_fileName);
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
            DD.IsEnabled = false;
            DD.ClearAll();
            _isOpen = false;
            _fileName = string.Empty;
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
            PrintGraph(SemanticWeb.Web());
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

        #region Выделение отмеченных вершин/дуг
        private void DdNodeSelected(object sender, NodeEventArgs e)
        {
            _selectNode.Add(e.Node);
            e.Node.StrokeThickness = 5;

            ChangeTopMenuNode();
            
        }

        private void DdNodeDeselected(object sender, NodeEventArgs e)
        {
            e.Node.StrokeThickness = 1;
            _selectNode.Remove(e.Node);
            ChangeTopMenuNode();
        }

        private void DdLinkSelected(object sender, LinkEventArgs e)
        {
            e.Link.StrokeThickness = 5;
        }

        private void DdLinkDeselected(object sender, LinkEventArgs e)
        {
            e.Link.StrokeThickness = 1;
        }
        #endregion

        #region Верхняя менюшка по вершинам:добавить/изменить/удалить
        private void AddNodeButtonClick(object sender, RoutedEventArgs e)
        {
            CreateNode("", 0, 0);
        }

        private void ChangeNodeButtonClick(object sender, RoutedEventArgs e)
        {
            if (_selectNode.Count==1)
                DdNodeTextEditing(new object(), new NodeValidationEventArgs(_selectNode[0]));
        }

        private void DeleteNodeButtonClick(object sender, RoutedEventArgs e)
        {
            while (_selectNode.Count > 0)
            {
                DD.Nodes.Remove(_selectNode[0]);
            }
        }
        #endregion

        #region Вспомогательная штучка, которая зажигает кнопочки для редактирования вершин
        /// <summary>
        /// Вспомогательная штучка, которая зажигает кнопочки для редактирования вершин
        /// </summary>
        void ChangeTopMenuNode()
        {
            AddNodeButton.IsEnabled = true;
            if (_selectNode.Count == 0)
            {
                ChangeNodeButton.IsEnabled = false;
                DeleteNodeButton.IsEnabled = false;
            }
            if (_selectNode.Count == 1)
            {
                ChangeNodeButton.IsEnabled = true;
                DeleteNodeButton.IsEnabled = true;
            }
            if (_selectNode.Count > 1)
            {
                ChangeNodeButton.IsEnabled = false;
                DeleteNodeButton.IsEnabled = true;
            }
        } 
        #endregion

        #region Closing
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            _validation.Dispose();
        } 
        #endregion

        #region комопнента объяснения (I семестр магистратуры)

        #region выделение контура указанных дуг и вершин

        public void AddStrokeThickness(List<Node> listNode, List<Arc> listArc)
        {


            #region вершины
            foreach (var node in listNode) //для каждой вершины из заданного списка
                foreach (DiagramNode t in DD.Nodes)
                {
                    if (((Node)t.Tag).ID == node.ID)
                    {
                        t.StrokeThickness = 5;
                        break;
                    }
                }

            #endregion
            #region дуги

            foreach (var arc in listArc) //для каждой вершины из заданного списка
                foreach (DiagramLink t in DD.Links)
                {
                    if (t.Text == arc.Name && ((Node)t.Origin.Tag).ID == arc.From
                        && ((Node)t.Destination.Tag).ID == arc.To)
                    {
                        t.StrokeThickness = 5;
                        break;
                    }
                }

            #endregion
        }

        #endregion

        #endregion
    }
}