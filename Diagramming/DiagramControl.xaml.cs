using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Kernel;
using MindFusion.Diagramming.Wpf;
using SynonymEditor;

namespace DiagramControls
{
    /// <summary>
    /// Interaction logic for DiagramControl.xaml
    /// </summary>
    public partial class DiagramControl
    {
        #region Public Properties
        /// <summary>
        /// Messages for Log
        /// </summary>
        public List<string> Messages { get; set; }
        /// <summary>
        /// View or Edit Behavior (see MindFusion Behavior)
        /// </summary>
        public Behavior Behavior { 
            get { return DD.Behavior;}
            set { DD.Behavior = value; } }
        /// <summary>
        /// True if CanEdit checkbox is checked
        /// </summary>
        public bool CanEdit { get; set; }
        /// <summary>
        /// File with the semantic web (used for implementing apply and cancel commnads)
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Zoom
        /// </summary>
        public double ZoomFactor
        {
            get { return DD.ZoomFactor; }
            set { DD.ZoomFactor = value; }
        }

        public new bool IsEnabled
        {
            get { return DD.IsEnabled; }
            set { DD.IsEnabled = value; }
        }

        public bool Load { get; set; }
        public bool Reload { get; set; }

        #endregion

        #region Private fields

        private const int NodeHeight = 50;
        private const int NodeWidth = 50;

        #endregion

        #region Ctor
        public DiagramControl()
        {
            InitializeComponent();
            DD.DefaultShape = Shapes.Ellipse;
            DD.LinkHeadShape = ArrowHeads.PointerArrow;
            DD.LinkHeadShapeSize = 10;
            DD.IsEnabled = false;
            Load = true;
            Messages = new List<string>();
            SelectedNodes = new List<DiagramNode>();
        } 
        #endregion

        #region FitSize

        public void FitSize()
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

        #endregion

        #region Saving

        public void SaveCoordinates()
        {
            foreach (DiagramNode t in DD.Nodes)
            {
                SemanticWeb.Web().ChangeNodeCoordinates(((Node)t.Tag).ID, t.Bounds.X, t.Bounds.Y);
            }
        }

        public void ClearAll()
        {
            DD.ClearAll();
        }

        #endregion

        #region Nodes
        /// <summary>
        /// Node deleting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdNodeDeleted(object sender, NodeEventArgs e)
        {
            try
            {
                Messages.Add("Удаление вершины" + ((Node)e.Node.Tag).Name);
                if (!Reload && !Load)
                    SemanticWeb.Web().DeleteNode((Node)e.Node.Tag);
            }
            catch (ArgumentException e1)
            {
                Messages.Add("Вершина " + ((Node)e.Node.Tag).Name + "не может быть удалена");
                MessageBox.Show(e1.Message);
            }
        }

        public void DeleteSelectedNodes()
        {
            while (SelectedNodes.Count > 0)
            {
                DD.Nodes.Remove(SelectedNodes[0]);
            }
        }

        /// <summary>
        /// Изменение имени вершины
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DdNodeTextEditing(object sender, NodeValidationEventArgs e)
        {
            if (!CanEdit)
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
                    Messages.Add("Изменение имени вершины на " + e.Node.Text + "  завершилось");
                    SemanticWeb.Web().ChangeNodeName(((Node)e.Node.Tag).ID, newName);
                }
                else
                {
                    Messages.Add("отмена изменения имени вершины " + e.Node.Text);
                }
            }
            catch (ArgumentException e1)
            {
                Messages.Add("отмена изменения имени вершины: " + e1.Message);
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
            if (!Load)
            {
                CreateNode("", e.MousePosition.X, e.MousePosition.Y);
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

        public void CreateNode(string name, double x, double y)
        {
            try
            {
                Node tagNode = SemanticWeb.Web().AddNode("");
                var bounds = new Rect(new Point(x, y), new Point(x + NodeWidth, y + NodeHeight));
                var diagramNode = DD.Factory.CreateShapeNode(bounds);
                diagramNode.Text = name;
                diagramNode.Tag = tagNode;
                DD.Nodes.Add(diagramNode);
                Messages.Add("Создание вершины " + name);
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
            }
        } 
        #endregion

        #region Arcs
        /// <summary>
        /// Изменение имени дуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdLinkTextEditing(object sender, LinkValidationEventArgs e)
        {

            if (!CanEdit)
            {
                e.Cancel = true;
                return;
            }
            Messages.Add("Изменение типа дуги: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
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
                    Messages.Add("Изменение дуги завершилось: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
                else
                {
                    Messages.Add("Отмена изменения типа дуги: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                    e.Cancel = true;
                }
            }
            catch (ArgumentException e1)
            {
                Messages.Add("Отмена изменения типа дуги с ошибкой: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
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
                Messages.Add("Создание дуги " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                if (formLink.DialogResult == true)
                {
                    e.Link.Text = formLink.ReturnValue().ToString();
                    SemanticWeb.Web().AddArc(((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                    e.Link.Tag = true;
                    Messages.Add("Создание дуги завершилось: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
                else
                {
                    DD.Links.Remove(e.Link);
                    Messages.Add("Создание дуги отменено: " + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
            }
            catch (ArgumentException e1)
            {
                MessageBox.Show(e1.Message);
                /*произошла ошибка вставки, удаляем дугу из графа*/
                e.Link.Tag = false;
                DD.Links.Remove(e.Link);
                Messages.Add("Создание дуги отменено: " + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
            }
        }

        /// <summary>
        /// Удаление дуги
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DdLinkDeleting(object sender, LinkValidationEventArgs e)
        {
            if (Load)
            {
                e.Cancel = true; return;
            }
            if (!CanEdit)
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
            if (Load)
            {
                return;
            }
            if (!CanEdit)
                return;
            // DD.Links.Remove()
            if (e.Link.Text != "")
            {
                if ((bool)e.Link.Tag)
                {
                    if (!Load || !Reload)
                        SemanticWeb.Web().DeleteArc(((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                }
                Messages.Add("дуга удалена: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
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
            if (!Load)
            {
                return;
            }
            if (!CanEdit)
                return;
            Messages.Add("изменение дуги: " + _oldLink.Text + " от " + _oldLink.Origin.Text + " к " + _oldLink.Destination.Text);
            try
            {
                if (_oldLink.Origin.Text != e.Link.Origin.Text)
                {
                    SemanticWeb.Web().ChangeArcDirectionFrom(((Node)_oldLink.Origin.Tag).ID, ((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                    Messages.Add("изменение дуги заврешено: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
                if (_oldLink.Destination.Text != e.Link.Destination.Text)
                {
                    SemanticWeb.Web().ChangeArcDirectionFrom(((Node)_oldLink.Origin.Tag).ID, ((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                    Messages.Add("изменение дуги заврешено: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
            }
            catch (ArgumentException e1)
            {
                e.Link.Text = _oldLink.Text;
                e.Link.Origin = _oldLink.Origin;
                e.Link.Destination = _oldLink.Destination;
                MessageBox.Show(e1.Message);
                Messages.Add("изменение дуги отменено: " + _oldLink.Text + " от " + _oldLink.Origin.Text + " к " + _oldLink.Destination.Text);
            }
            catch (NullReferenceException e2)
            {

            }
        }

        private void DdLinkModifying(object sender, LinkValidationEventArgs e)
        {
            if (!Load)
            {
                e.Cancel = true; return;
            }
            if (!CanEdit)
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

        #region PrintGraph

        public void PrintGraph(SemanticWeb web)
        {
            Load = true;
            Reload = true;
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
            Load = false;
            Reload = false;
        } 
        #endregion

        #region Контекстное меню
        #region Контекстное меню для вершины

        DiagramNode _rightNode; //вершина по которой щелкнули правой кнопкой

        private void DdNodeClicked(object sender, NodeEventArgs e)
        {
            myContextMenu.IsEnabled = false;
            myContextMenu.Visibility = Visibility.Collapsed;
            if (e.MouseButton == MouseButton.Right && CanEdit)
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
            var w = new SynWindow(FileName, ((Node)_rightNode.Tag).Name);
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

            if (e.MouseButton == MouseButton.Right && CanEdit)
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
        public List<DiagramNode> SelectedNodes { get; set; }

        private void DdClicked(object sender, DiagramEventArgs e)
        {
            myContextMenu.Visibility = Visibility.Collapsed;
            if (e.MouseButton == MouseButton.Right && (CanEdit))
            {
                _rightClick = e.MousePosition;
                myContextMenu.Visibility = Visibility.Visible;
                myContextMenu.IsEnabled = true;
                myContextMenu.DataContext = MenuOptions;
                SelectedNodes = new List<DiagramNode>();
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
                return items;
            }
        }

        #endregion
        #endregion

        #region Выделение отмеченных вершин/дуг
        private void DdNodeSelected(object sender, NodeEventArgs e)
        {
            SelectedNodes.Add(e.Node);
            e.Node.StrokeThickness = 5;
           // ChangeTopMenuNode();

        }

        private void DdNodeDeselected(object sender, NodeEventArgs e)
        {
            e.Node.StrokeThickness = 1;
            SelectedNodes.Remove(e.Node);
           // ChangeTopMenuNode();
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
