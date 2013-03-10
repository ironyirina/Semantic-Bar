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
using MouseButton = MindFusion.Diagramming.Wpf.MouseButton;
using System.Drawing;
using System.Windows.Media;


namespace textMindFusion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int countSelectedNode = 0;
        List<DiagramNode> SelectNode = new List<DiagramNode>();
        
        int nodeHeight = 50;
        int nodeWidth = 50;
        public string path = "MyWeb.xml";
        bool load = true;
        bool reload = false;
        SemanticWeb _myWeb;
        int countNotNamed = 0;// кол-во неименованный вершин для создания
        //список коррекции. ключ - id в диаграмме, элемент - полноценная вершина из списка
        //Dictionary<object, Node> convertList = new Dictionary<object, Node>();    

       // Rect bounds = new Rect(0, 0, 50, 50); //размер узла по умолчанию

        public MainWindow()
        {
            InitializeComponent();

            DD.DefaultShape = Shapes.Ellipse;
            DD.LinkHeadShape = ArrowHeads.PointerArrow;
            DD.LinkHeadShapeSize = 10;
            DD.IsEnabled = false;
            //TopStackPanel.IsEnabled = false;
            
        }

        #region  служебные: отправка письма, проверка имени, вывод графа

        void SendMessage(string message)
        {
            if (!load && !reload)
            {
                ListBoxLog.Items.Add(message);
            }
        }


        void PrintGraph(SemanticWeb web)
        {
            load = true;
            reload = true;
            DD.ClearAll();
            Dictionary<int, DiagramNode> nodeMap = new Dictionary<int, DiagramNode>();
            foreach (var node in web.Nodes)
            {
                var Bounds = new Rect(new System.Windows.Point(node.X, node.Y), new System.Windows.Point(node.X + nodeWidth, node.Y + nodeHeight));
                var diagramNode = DD.Factory.CreateShapeNode(Bounds);
                //diagramNode.Brush = new LinearGradientBrush(new GradientStopCollection
                nodeMap[node.ID] = diagramNode;
                diagramNode.Text = node.Name;
                diagramNode.Tag = node;
            }

            foreach (var arc in web.Arcs)
            {
                if (!arc.Name.Contains("_#"))
                {
                    var diagramArc =
                        DD.Factory.CreateDiagramLink(nodeMap[arc.From], nodeMap[arc.To]);
                    diagramArc.Text = arc.Name;
                    diagramArc.Tag = true;
                }
            }
            load = false;
            reload = false;
            // arrange the graph(расстановка)
            //sbState.Style = (Style)FindResource("ReadySBStyle");
            if (!Verification())
                MessageBox.Show("Обнаружены ошибки в сети.\n Для просмотра ошибок загляните в отчеты", "внимание!");
           
   
        }
        #endregion

        #region сохранить/отменить
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Verification())
            {
                if (MessageBox.Show("Обнаружены ошибки в сети.\n Для просмотра ошибок загляните в отчеты", "внимание!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }
            for (int i = 0; i < DD.Nodes.Count; i++)
            {
                _myWeb.ChangeNodeCoordinates(((Node)DD.Nodes[i].Tag).ID, DD.Nodes[i].Bounds.X, DD.Nodes[i].Bounds.Y);
            }
                SemanticWeb.WriteToXml(path, _myWeb);
            SendMessage("изменения сохранены");
        }

        public static RoutedUICommand Cancel { get; private set; }

        private void CancelCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            reload = true; load = true;
            _myWeb = SemanticWeb.ReadFromXml(path);
            PrintGraph(_myWeb);
            reload = false; load = false;
            SendMessage("изменения отменены");
            _somethingChanged = false;
        }

        private void CancelCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _somethingChanged;
        }
        #endregion

        #region Zoom-Panel
        private void zoomInButton_Click(object sender, RoutedEventArgs e)
        {
            DD.ZoomFactor = Math.Min(1000, DD.ZoomFactor + 10);
        }

        private void zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            DD.ZoomFactor = Math.Max(20, DD.ZoomFactor - 10);
        }

        private void fitButton_Click(object sender, RoutedEventArgs e)
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

        private void noZoomButton_Click(object sender, RoutedEventArgs e)
        {
            DD.ZoomFactor = 100;
        }
        #endregion 
        
        #region вершины

        private void DD_NodeDeleting(object sender, NodeValidationEventArgs e)
        {
            if (load)
            {
                e.Cancel = true; return;
            }
            if (isEdit.IsChecked == false)
            {
                e.Cancel = true; return;
            }
            if (((Node)e.Node.Tag).IsSystem || ((Node)e.Node.Tag).Name.Contains("#"))

            {
                _somethingChanged = true;
                e.Cancel = true;
            }
        }

        private void DD_NodeDeleted(object sender, NodeEventArgs e)
        {

            try
            {
                SendMessage("удаление вершины" + ((Node)e.Node.Tag).Name);
                if (!reload && !load )
                    _myWeb.DeleteNode((Node)e.Node.Tag);
                _somethingChanged = true;
            }
            catch (ArgumentException e1)
            {
                SendMessage("Вершина " + ((Node)e.Node.Tag).Name + "не может быть удалена");
                MessageBox.Show(e1.Message);
            }
        }


        private void DD_NodeTextEditing(object sender, NodeValidationEventArgs e)
        {
            if (isEdit.IsChecked == false)
            {
                e.Cancel = true;
                return;
            }
            if (((Node)e.Node.Tag).IsSystem)
                e.Cancel = true;
            if (((Node)e.Node.Tag).Name.Contains("#"))
            {
                e.Cancel = true;
                return;
            }

            string newName; //новое имя вершины
            TextBoxForm formName = new TextBoxForm(_myWeb, e.Node.Text);
            //formName.RefreshValue();
            formName.ShowDialog();
            try
            {
                if (formName.DialogResult == true)
                {
                    newName = formName.ReturnValue();
                    _myWeb.ChangeNodeName(((Node)e.Node.Tag).ID, newName);
                    e.Node.Text = newName;
                    SendMessage("Изменение имени вершины на " + e.Node.Text + "  завершилось");
                    _somethingChanged = true;
                    //formName.Close();
                    //e.Cancel = true;
                }
                else
                {
                    SendMessage("отмена изменения имени вершины " + e.Node.Text);
                    //formName.Close();
                    //e.Cancel = true;
                }
            }
            catch (ArgumentException e1)
            {
                SendMessage("отмена изменения имени вершины: " + e1.Message);
                //e.Cancel = true;
            }
            formName.Close();
            e.Cancel = true;
           


        }

        private void DD_NodeCreated(object sender, NodeEventArgs e)
        {
            if (!load)
            {
                try
                {
                    Node webNode = _myWeb.AddNode(""); //неименованная
                    e.Node.Text = (webNode.Name);
                    e.Node.Tag = webNode;
                    countNotNamed++;
                    SendMessage("создана вершина " + ((Node)e.Node.Tag).Name);
                    _somethingChanged = true;
                }
                catch (ArgumentException e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }
        }

        #endregion

        #region дуги
        private void DD_LinkTextEditing(object sender, LinkValidationEventArgs e)
        {

            if (isEdit.IsChecked==false)
            {
                e.Cancel = true;
                return;
            }
            string newT; //новое имя вершины выбранное из комбобоксика
            SendMessage("изменение типа дуги: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
            ComboBoxForm formLink = new ComboBoxForm();
            var listarc = _myWeb.GetAllowedArcNames(((Node)e.Link.Origin.Tag).ID);
            formLink.RefreshValue(listarc);
            formLink.ShowDialog();
            try
            {
                if (formLink.DialogResult == true)
                {
                    newT = formLink.ReturnValue().ToString();
                    _myWeb.ChangeArcName(((Node)e.Link.Origin.Tag).ID, e.Link.Text, newT, ((Node)e.Destination.Tag).ID);
                    e.Link.Text = newT;
                    SendMessage("изменение дуги завершилось: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                    _somethingChanged = true;
                }
                else
                {
                    SendMessage("отмена изменения типа дуги: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                    e.Cancel = true;
                }
            }
            catch (ArgumentException e1)
            {
                SendMessage("отмена изменения типа дуги с ошибкой: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                MessageBox.Show(e1.Message);
                e.Cancel = true;
            }
            formLink.Close();
            e.Cancel = true;
        }

        private void DD_LinkCreated(object sender, LinkEventArgs e)
        {

            try
            {
                ComboBoxForm formLink = new ComboBoxForm();
                formLink.RefreshValue(_myWeb.GetAllowedArcNames(((Node)e.Link.Origin.Tag).ID));
                formLink.ShowDialog();
                SendMessage("создание дуги " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                if (formLink.DialogResult == true)
                {
                    //throw new ArgumentException();
                    e.Link.Text = formLink.ReturnValue().ToString();
                    _myWeb.AddArc(((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                    e.Link.Tag = true;
                    SendMessage("создание дуги завершилось: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                    _somethingChanged = true;
                }
                else
                {
                    //throw new ArgumentException("отмена выбора имени дуги");
                    DD.Links.Remove(e.Link);
                    SendMessage("создание дуги отменено: " + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
            }
            catch (ArgumentException e1)
            {
                MessageBox.Show(e1.Message);
                /*произошла ошибка вставки, удаляем дугу из графа*/
                e.Link.Tag = false;
                DD.Links.Remove(e.Link);
                SendMessage("создание дуги отменено: " + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
            }
        }

        private void DD_LinkDeleting(object sender, LinkValidationEventArgs e)
        {
            if (!load)
            {
                e.Cancel = true; return;
            }
            if (isEdit.IsChecked == true)
            {
                e.Cancel = true;
                return;
            }
        }
        
        private void DD_LinkDeleted(object sender, LinkEventArgs e)
        {

            if (!load)
            {
                return;
            }
            if (isEdit.IsEnabled == false)
                return;

            // DD.Links.Remove()
            if (e.Link.Text != "")
            {
                if ((bool)e.Link.Tag)
                {
                   if (!load || !reload) 
                     _myWeb.DeleteArc(((Node) e.Link.Origin.Tag).ID, e.Link.Text, ((Node) e.Link.Destination.Tag).ID);
                }
                SendMessage("дуга удалена: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                _somethingChanged = true;
            }
            // e.Link.Destination - цель
            // e.Link.Origin//от куда
        }
        #region FromID-To дуги
        bool change = false; //=true, когда уже сохранили дугу
        DiagramLink oldLink = new DiagramLink();

      

        private void DD_LinkModified(object sender, LinkEventArgs e)
        {
            if (!load)
            {
                return;
            }
            if (isEdit.IsChecked==false)
                return;
            SendMessage("изменение дуги: " + oldLink.Text + " от " + oldLink.Origin.Text + " к " + oldLink.Destination.Text);
            try
            {
                if (oldLink.Origin.Text != e.Link.Origin.Text)
                {
                    _myWeb.ChangeArcDirectionFrom(((Node)oldLink.Origin.Tag).ID, ((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                    SendMessage("изменение дуги заврешено: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                    _somethingChanged = true;
                }
                if (oldLink.Destination.Text != e.Link.Destination.Text)
                {
                    _myWeb.ChangeArcDirectionFrom(((Node)oldLink.Origin.Tag).ID, ((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                
                    //_myWeb.ChangeArcDirectionTo(((Node)e.Link.Origin.Tag).ID, oldLink.Destination.Text, ((Node)e.Link.Tag).ID, ((Node)e.Link.Destination.Tag).ID);
                    SendMessage("изменение дуги заврешено: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                    _somethingChanged = true;
                }


            }
            catch (ArgumentException e1)
            {
                e.Link.Text = oldLink.Text;
                e.Link.Origin = oldLink.Origin;
                e.Link.Destination = oldLink.Destination;
                MessageBox.Show(e1.Message);
                SendMessage("изменение дуги отменено: " + oldLink.Text + " от " + oldLink.Origin.Text + " к " + oldLink.Destination.Text);
            }
            catch(NullReferenceException e2)
            {
                
            }
        }

        private void DD_LinkModifying(object sender, LinkValidationEventArgs e)
        {
            if (!load)
            {
                e.Cancel = true; return;
            }
            if (isEdit.IsChecked == false)
            {
                e.Cancel = true;
                return;
            }

            if (!change)
            {
                oldLink.Destination = e.Link.Destination;
                oldLink.Origin = e.Link.Origin;
                oldLink.Text = e.Link.Text;
                change = true;
            }
        }
#endregion
#endregion

        #region загрузка/закрытие окна
        //private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    MessageBoxResult result = MessageBox.Show("Сохранить изменения и выйти?", "Изменения не сохранены?",
        //                  MessageBoxButton.YesNoCancel);// == System.Windows.Forms.DialogResult.No)
        //    switch (result)
        //    {
        //        case MessageBoxResult.Yes: /*SemanticWeb.WriteToXml(path, _myWeb)*/saveButton_Click(new object(),new RoutedEventArgs()); break;//сохр. изменения  
        //        case MessageBoxResult.No: break;  //закрыть
        //        default: e.Cancel = true; break; //остановить закрытие
        //    } 
        //}

        //private void Window_Loaded_1(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        _myWeb = SemanticWeb.ReadFromXml(path);
        //        //myViewWeb = new SemanticWebUsersLevel(_myWeb);
        //        PrintGraph(_myWeb);
        //        SendMessage("Сем. сеть загружена");
        //    }
        //    catch (Exception e1)
        //    {
        //        load = false;
        //        SendMessage("Сем. сеть пуста");

        //    }
        //}
#endregion

     

       
        
        private void isEdit_Click(object sender, RoutedEventArgs e)
        {
            if (isEdit.IsChecked == true)
                DD.Behavior = Behavior.DrawLinks;
            else
                DD.Behavior = Behavior.Pan;
        }

        #region контекстное для вершины

        DiagramNode rightNode; //вершина по которой щелкнули правой кнопкой

        private void DD_NodeClicked(object sender, NodeEventArgs e)
        {
            myContextMenu.IsEnabled = false;
            myContextMenu.Visibility = Visibility.Collapsed;
            if (e.MouseButton == MouseButton.Right && (isEdit.IsChecked == true))
            {
                myContextMenu.Visibility = Visibility.Visible;
                myContextMenu.IsEnabled = true;
                myContextMenu.DataContext = MenuOptionsNode;
                rightNode = e.Node;
            }
        }

        public List<MenuItem> MenuOptionsNode
        {
            get
            {
                var items = new List<MenuItem>();
                var del = new MenuItem { Header = "Удалить вершину" };
                del.Click += delOnClick;
                items.Add(del);
                var change = new MenuItem { Header = "Изменить вершину" };
                change.Click += changeOnClick;
                items.Add(change);
                return items;
            }
        }

        private void delOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            DD.Nodes.Remove(rightNode);
        }
        private void changeOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            DD_NodeTextEditing(new object(), new NodeValidationEventArgs(rightNode));
            
        }
        #endregion
        #region контекстное для связи
        DiagramLink rightLink; //связь по которой щелкнули правой кнопкой
        private void DD_LinkClicked(object sender, LinkEventArgs e)
        {
            myContextMenu.IsEnabled = false;
            myContextMenu.Visibility = Visibility.Collapsed;

            if (e.MouseButton == MouseButton.Right && isEdit.IsChecked==true)
            {
                rightLink = e.Link;
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
                change.Click += changeLinkOnClick;
                items.Add(change);
                return items;
            }
        }

        private void DelLinkOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            DD.Links.Remove(rightLink);
        }
        private void changeLinkOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            DD_LinkTextEditing(new object(),new LinkValidationEventArgs(rightLink));
        }

#endregion
        #region контекстное для графа
        System.Windows.Point  rightClick; //позиция правого клика мыши по полю
        private void DD_Clicked(object sender, DiagramEventArgs e)
        {
           
            myContextMenu.Visibility = Visibility.Collapsed;
            if (e.MouseButton == MouseButton.Right &&   (isEdit.IsChecked==true))
            {
                rightClick = e.MousePosition;
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
                var addn = new MenuItem { Header = "добавить веришну" };
                addn.Click += addnOnClick;
                items.Add(addn);

                
                var arrange = new MenuItem { Header = "авторасстановка" };
                arrange.Click += arrangeOnClick;
                items.Add(arrange);
                
                return items;
            }
        }

        private void arrangeOnClick(object sender, RoutedEventArgs routedEventArgs)
        {

            var layout = new LayeredLayout();
            layout.Arrange(DD);
        }
        private void addnOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Node tagNode =  _myWeb.AddNode("");
            var Bounds = new Rect(new System.Windows.Point(rightClick.X, rightClick.Y), new System.Windows.Point(rightClick.X + nodeWidth, rightClick.Y + nodeHeight));
            var diagramNode = DD.Factory.CreateShapeNode(Bounds);
            diagramNode.Text = "";
            diagramNode.Tag = tagNode;
            DD.Nodes.Add(diagramNode);
            SendMessage("Создание вершины");
            _somethingChanged = true;

        }
        #endregion

        #region веривикация/расставить

        private void VerificationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Verification())
                MessageBox.Show("Обнаружены ошибки в сети.\n Для просмотра ошибок загляните в отчеты", "внимание!");
            else
                MessageBox.Show("Ошибок в сети не обнаружено");
        }
        bool Verification()
        {
            ListBoxVerification.Items.Clear();
            var verefication = new Verification(_myWeb);
            verefication.Verificate();
            if (verefication.NoErros)
            {
                ListBoxVerification.Items.Add("Ошибок не обнаружено");
                return true;
            }
            foreach (var i in verefication.Errors)
                ListBoxVerification.Items.Add(i);
            return false;
        }
        #endregion

        #region Главное Меню

        #region Переменные
        private string _fileName = string.Empty;
        private bool _somethingChanged;
        private bool _isOpen;
        private const string DefaultExtension = ".xml";
        #endregion

        #region Инициализация

        static MainWindow()
        {
            EditWithForms = new RoutedUICommand("EditWithForms", "EditWithForms", typeof(MainWindow));
            //EditVisual = new RoutedUICommand("EditVisual", "EditVisual", typeof(MainWindow));
            Consult = new RoutedUICommand("Consult", "Consult", typeof(MainWindow));
            Cancel = new RoutedUICommand("Cancel", "Cancel", typeof(MainWindow));
        }
        #endregion

        #region Файл
        #region Создать

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //если в данный момент что-то открыто, это надо закрыть
            if (_isOpen)
                ApplicationCommands.Close.Execute(null, null);
            _myWeb = new SemanticWeb();
            _isOpen = true;
            PrintGraph(_myWeb);
            DD.IsEnabled = true;
            ChangeTopMenuNode();
            VerificationButton.IsEnabled = ArrangeButton.IsEnabled = true;
            zoomInButton.IsEnabled = zoomOutButton.IsEnabled = fitButton.IsEnabled = noZoomButton.IsEnabled = true;
        }

        #endregion

        #region Открыть

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (_isOpen)
                ApplicationCommands.Close.Execute(null, null);
            //var ofd = new OpenFileDialog { Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*" };
            //if (ofd.ShowDialog() != true) return;
            //_fileName = ofd.FileName;
            _fileName = @"C:\Users\Ирина\Documents\Чуприна\1 — копия.xml";
            _myWeb = SemanticWeb.ReadFromXml(_fileName);
            PrintGraph(_myWeb);
            DD.IsEnabled = true;
            _isOpen = true;
            ChangeTopMenuNode();
            VerificationButton.IsEnabled = ArrangeButton.IsEnabled = true;
            zoomInButton.IsEnabled = zoomOutButton.IsEnabled = fitButton.IsEnabled = noZoomButton.IsEnabled = true;
        }

        #endregion

        #region Сохранить

        private void BeforeSaving()
        {
            if (!Verification())
            {
                if (MessageBox.Show("Обнаружены ошибки в сети.\n Для просмотра ошибок загляните в отчеты", "Внимание!", 
                    MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }
            foreach (DiagramNode t in DD.Nodes)
            {
                _myWeb.ChangeNodeCoordinates(((Node)t.Tag).ID, t.Bounds.X, t.Bounds.Y);
            }
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            BeforeSaving();
            //Если мы ещё ни разу не сохранялись, то идём в SaveAs
            if (_fileName == string.Empty)
                ApplicationCommands.SaveAs.Execute(null, null);
            else
            {
                SemanticWeb.WriteToXml(_fileName, _myWeb);
                _somethingChanged = false;
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
            var saveDialog = new SaveFileDialog { FileName = _fileName, AddExtension = true, DefaultExt = DefaultExtension };
            if (saveDialog.ShowDialog() != true) return;
            _fileName = saveDialog.FileName;
            SemanticWeb.WriteToXml(_fileName, _myWeb);
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
            DD.IsEnabled = false;
            DD.ClearAll();
            _isOpen = false;
            _fileName = string.Empty;
            _somethingChanged = false;
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

        #region Редактирование через формочки

        public static RoutedUICommand EditWithForms { get; private set; }

        public void EditWithFormsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //InProgerssStatusBar("Редактирование");
            //var f = new EditWindow(_filePath + _copyFileName);
            //f.ShowDialog();
            //_somethingChanged = true;
            //NormalizeStatusBar();
            var f = new KacWindow {SW = _myWeb};
            f.ShowDialog();
            _myWeb = f.SW;
            SemanticWeb.WriteToXml(_fileName, _myWeb);
            //_myWeb = SemanticWeb.ReadFromXml(_filePath + _copyFileName);
            PrintGraph(_myWeb);
            _somethingChanged = true;
        }

        #endregion

        #region Консультация

        public static RoutedUICommand Consult { get; private set; }

        private void ConsultExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //проверка сем. сети
            //_sw = SemanticWeb.ReadFromXml(_filePath + _copyFileName);
            var checker = new Verification(_myWeb);
            checker.Verificate();
            if (!checker.NoErros)
            {
                //ErrorStatusBar("Сем. сеть содержит ошибки. Консультация невозможна.");
                return;
            }
            var w = new ConsultingWindow.MainWindow { Sw = _myWeb };
            w.Show();
        }

        private void ConsultCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _isOpen;
        }

        #endregion

        private void DD_LinkActivated(object sender, LinkEventArgs e)
        {
            //e.Link.Style.Setters.Add(new Setter(
        }

        #endregion

        #region выделение отмеченных вершин/дуг
        private void DD_NodeSelected(object sender, NodeEventArgs e)
        {
            //var stroke = new SolidColorBrush (System.Windows.Media.Color.FromRgb(255,0,0));
            //Style shapeNodeStyle = new Style();

            //shapeNodeStyle.Setters.Add(new Setter(ShapeNode.StrokeProperty, stroke));
            //e.Node.Style = shapeNodeStyle;
            SelectNode.Add(e.Node);
            e.Node.StrokeThickness = 5;

            ChangeTopMenuNode();
            
        }

        private void DD_NodeDeselected(object sender, NodeEventArgs e)
        {
            //var stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            //Style shapeNodeStyle = new Style();
            //shapeNodeStyle.Setters.Add(new Setter(ShapeNode.StrokeProperty, stroke));
            //e.Node.Style = shapeNodeStyle;
            e.Node.StrokeThickness = 1;
            SelectNode.Remove(e.Node);
            ChangeTopMenuNode();
        }

        private void DD_LinkSelected(object sender, LinkEventArgs e)
        {
            e.Link.StrokeThickness = 5;
        }

        private void DD_LinkDeselected(object sender, LinkEventArgs e)
        {
            e.Link.StrokeThickness = 1;
        }
        #endregion


        #region верхняя менюшка по вершинам:добавить/изменить/удалить
        private void AddNodeButton_Click(object sender, RoutedEventArgs e)
        {
            Node tagNode = _myWeb.AddNode("");
            var Bounds = new Rect(new System.Windows.Point(0, 0), new System.Windows.Point(0 + nodeWidth, 0 + nodeHeight));
            var diagramNode = DD.Factory.CreateShapeNode(Bounds);
            diagramNode.Text = "";
            diagramNode.Tag = tagNode;
            diagramNode.Selected = true;
            DD.Nodes.Add(diagramNode);
            SendMessage("Создание вершины");
            _somethingChanged = true;
        }

        private void ChangeNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectNode.Count==1)
                DD_NodeTextEditing(new object(), new NodeValidationEventArgs(SelectNode[0]));
        }

        private void DeleteNodeButton_Click(object sender, RoutedEventArgs e)
        {
            while (SelectNode.Count > 0)
            {
                DD.Nodes.Remove(SelectNode[0]);
            }
        }
#endregion

        void ChangeTopMenuNode()
        {
            AddNodeButton.IsEnabled = true;
            if (SelectNode.Count == 0)
            {
                ChangeNodeButton.IsEnabled = false;
                DeleteNodeButton.IsEnabled = false;
            }
            if (SelectNode.Count == 1)
            {
                ChangeNodeButton.IsEnabled = true;
                DeleteNodeButton.IsEnabled = true;
            }
            if (SelectNode.Count > 1)
            {
                ChangeNodeButton.IsEnabled = false;
                DeleteNodeButton.IsEnabled = true;
            }
        }

        private void isEdit_Checked(object sender, RoutedEventArgs e)
        {

        }

    }
}

