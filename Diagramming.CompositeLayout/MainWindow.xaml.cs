//
// Copyright (c) 2012, MindFusion LLC - Bulgaria.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Win32;
using MindFusion.Diagramming.Wpf.Layout;
using System.Windows.Input;

using Kernel;
using textMindFusion;

namespace MindFusion.Diagramming.Wpf.Samples.CS.CompositeLayout
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
        public int zoomvalue = 75;
        string path;
        bool load = true;
        bool reload = false;
        SemanticWeb myWeb;
        SemanticWebUsersLevel myViewWeb;
        int countNotNamed = 0;// кол-во неименованный вершин для создания
        //список коррекции. ключ - id в диаграмме, элемент - полноценная вершина из списка
        //Dictionary<object, Node> convertList = new Dictionary<object, Node>();
        Rect bounds = new Rect(0, 0, 100, 100); //размер узла по умолчанию
        
        public MainWindow( string path)
		{
			InitializeComponent();

			DD.DefaultShape = Shapes.Ellipse;
			DD.LinkHeadShape = ArrowHeads.PointerArrow;
			DD.LinkHeadShapeSize = 10;

            this.path = path;
		}
        #region Zoom
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
        #region Сохранить/отменить
        /*применить*/
        private void saveButton_Click(object sender, RoutedEventArgs e)
		{
            SemanticWeb.WriteToXml(path, myWeb);
            SendMessage("изменения сохранены");
		}

        /*отменить*/
		private void loadButton_Click(object sender, RoutedEventArgs e)
		{
            reload = true;
            myWeb = SemanticWeb.ReadFromXml(path);
            PrintGraph(myViewWeb);
            reload = false;
            SendMessage("изменения отменены");
        }
        #endregion
        #region контекстное меню
        /// <summary>
        /// собственный класс для параметров пункта меню
        /// </summary>
        public class MenuItem
        {
            public string Text { get; set; }
            public List<MenuItem> Children { get; private set; }
            public ICommand Command { get; set; }

            public MenuItem(string item)
            {
                Text = item;
                Children = new List<MenuItem>();
            }
        }
        //само меню
        public List<MenuItem> MenuOptions = new List<MenuItem>();
        //навес события для вызова контестного меню. Общее контекстное меню
        private void diagram_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuOptions.Clear();
            MenuOptions.Add(new MenuItem("проверка"));// { Command = new DelegatingCommand(messg, () => "привет") });
            
            //return menu;
        }

        void messg(string STRING)
        {
            MessageBox.Show(STRING);
        }
        #endregion

        /// <summary>
        /// Загрузка из XML файла с десериализацией
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                myWeb = SemanticWeb.ReadFromXml(path);
                myViewWeb = new SemanticWebUsersLevel(myWeb);
                PrintGraph(myViewWeb);
                SendMessage("Сем. сеть загружена");
            }
            catch (Exception e1)
            {
                load = false;
                SendMessage("Сем. сеть пуста");

            }
        }

        void PrintGraph(SemanticWebUsersLevel web)
        {

            DD.ClearAll();
            Dictionary<int, DiagramNode> nodeMap = new Dictionary<int, DiagramNode>();
            foreach (var node in web.Nodes)
            {
                var diagramNode = DD.Factory.CreateShapeNode(bounds);
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
                        DD.Factory.CreateDiagramLink(nodeMap[arc.From.ID], nodeMap[arc.To.ID]);
                    diagramArc.Text = arc.Name;
                }
            }
            load = false;
            // arrange the graph(расстановка)
            var layout = new LayeredLayout();
            layout.Arrange(DD);
            //sbState.Style = (Style)FindResource("ReadySBStyle");

        }

        #region вершины
        private void DD_NodeDeleting(object sender, NodeValidationEventArgs e)
        {
            if (((Node)e.Node.Tag).IsSystem || ((Node)e.Node.Tag).Name.Contains("#") )
                e.Cancel = true;
        }

        private void DD_NodeDeselected(object sender, NodeEventArgs e)
        {
            try
            {
                SendMessage("удаление вершины" + ((Node)e.Node.Tag).Name);
                if (!reload)
                    myWeb.DeleteNode((Node)e.Node.Tag);
            }
            catch (ArgumentException e1)
            {
                SendMessage("Вершина " + ((Node)e.Node.Tag).Name + "не может быть удалена");
                MessageBox.Show(e1.Message);
            }
        }

        private void DD_NodeTextEditing(object sender, NodeValidationEventArgs e)
        {
            if (((Node)e.Node.Tag).IsSystem)
                e.Cancel = true;
        }

        private void DD_NodeTextEdited(object sender, EditNodeTextEventArgs e)
        {
            if (((Node)e.Node.Tag).Name.Contains("#"))
            {
                return;
            }
            try
            {
                SendMessage("изменение имени вершины " + e.OldText + " на " + e.NewText);
                if (e.OldText != e.NewText)
                {
                    NameExist(e.NewText);
                    myWeb.ChangeNodeName(((Node)e.Node.Tag).ID, e.NewText);

                }

                e.Node.Text = e.NewText;
                //изменить словарь
                ((Node)e.Node.Tag).Name = e.NewText;
                //говорим, что была изменена неименованная вершина
                if (e.OldText.Contains("new"))
                    countNotNamed--;
            }
            catch (ArgumentException e1)
            {
                SendMessage("изменение имени вершины " + e.OldText + " на " + e.NewText + " было отменено");
                MessageBox.Show(e1.Message);
                e.Node.Text = e.OldText;
                ((Node)e.Node.Tag).Name = e.OldText;
                return;
            }
        }

        private void DD_NodeCreated(object sender, NodeEventArgs e)
        {
            if (!load)
            {
                try
                {
                    Node webNode = myWeb.AddNode("new" + countNotNamed.ToString()); //неименованная
                    e.Node.Text = (webNode.Name);
                    e.Node.Tag = webNode;
                    countNotNamed++;
                    SendMessage("создана вершина " + ((Node)e.Node.Tag).Name);
                }
                catch (ArgumentException e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }
            //формируем предупреждение что есть несоединенная вершина со значением по умолчанию
        }
        #endregion
        #region ДУГИ
        private void DD_LinkTextEditing(object sender, LinkValidationEventArgs e)
        {
            string newT; //новое имя вершины выбранное из комбобоксика
            SendMessage("изменение типа дуги: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
            ComboBoxForm formLink = new ComboBoxForm();
            var listarc = myWeb.GetAllowedArcNames(((Node)e.Link.Origin.Tag).ID);
            formLink.RefreshValue(listarc);
            formLink.ShowDialog();
            try
            {
                if (formLink.DialogResult == true)
                {
                    newT = formLink.ReturnValue().ToString();
                    //myWeb.ChangeArcName(e.Link.Text, newT, e.Link.Origin.Text);
                    myWeb.ChangeArcName(((Node)e.Link.Origin.Tag).ID, e.Link.Text, newT, ((Node)e.Destination.Tag).ID);
                    e.Link.Text = newT;
                    SendMessage("изменение дуги завершилось: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
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
                formLink.RefreshValue(SemanticWeb.SystemArcs);
                formLink.ShowDialog();
                SendMessage("создание дуги " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                if (formLink.DialogResult == true)
                {
                    //throw new ArgumentException();
                    e.Link.Text = formLink.ReturnValue().ToString();
                    myWeb.AddArc(e.Link.Origin.Text, e.Link.Text, e.Link.Destination.Text);
                    SendMessage("создание дуги завершилось: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
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
                DD.Links.Remove(e.Link);
                SendMessage("создание дуги отменено: " + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
            }
        }

        private void DD_LinkDeleted(object sender, LinkEventArgs e)
        {
            // DD.Links.Remove()
            myWeb.DeleteArc(((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
            SendMessage("дуга удалена: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
            // e.Link.Destination - цель
            // e.Link.Origin//от куда
        }
           #region From-To дуги
        /// служебные пеерменные
        bool change = false; //=true, когда уже сохранили дугу
        DiagramLink oldLink = new DiagramLink();
        private void DD_LinkModifying(object sender, LinkValidationEventArgs e)
        {
            if (!change)
            {
                oldLink.Destination = e.Link.Destination;
                oldLink.Origin = e.Link.Origin;
                oldLink.Text = e.Link.Text;
                change = true;
            }
        }

        private void DD_LinkModified(object sender, LinkEventArgs e)
        {
            SendMessage("изменение дуги: " + oldLink.Text + " от " + oldLink.Origin.Text + " к " + oldLink.Destination.Text);
            try
            {
                if (oldLink.Origin.Text != e.Link.Origin.Text)
                {
                    myWeb.ChangeArcDirectionFrom(((Node)oldLink.Origin.Tag).ID, ((Node)e.Link.Origin.Tag).ID, e.Link.Text, ((Node)e.Link.Destination.Tag).ID);
                    SendMessage("изменение дуги заврешено: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
                }
                if (oldLink.Destination.Text != e.Link.Destination.Text)
                {
                    myWeb.ChangeArcDirectionTo(((Node)e.Link.Origin.Tag).ID, oldLink.Destination.Text,((Node)e.Link.Tag).ID,  ((Node)e.Link.Destination.Tag).ID);
                    SendMessage("изменение дуги заврешено: " + e.Link.Text + " от " + e.Link.Origin.Text + " к " + e.Link.Destination.Text);
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
        }
           #endregion  
#endregion
      
#region  служебные: отправка письма, проверка имени
    
        void SendMessage(string message)
        {
            if (!load && !reload)
            {
                ListBoxLog.Items.Add(message);
            }
        }

        void NameExist(string name)
        {
            for (int i = 0; i < myViewWeb.Nodes.Count; i++)
                if (name == myViewWeb.Nodes[i].Name)
                    throw new ArgumentException("такое имя уже существует");
        }
#endregion

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Изменения не сохранены.Сохранить изменения ?", "?",
                           MessageBoxButton.YesNoCancel);// == System.Windows.Forms.DialogResult.No)
            switch (result)
            {
                case MessageBoxResult.Yes: SemanticWeb.WriteToXml(path, myWeb); break;//сохр. изменения  
                case MessageBoxResult.No: break;  //закрыть
                default: e.Cancel = true; break; //остановить закрытие
            } 
        }

    }

}
