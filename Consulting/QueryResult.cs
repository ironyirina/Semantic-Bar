using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Kernel;

namespace Consulting
{
    /// <summary>
    /// Результат запроса
    /// </summary>
    public class QueryResult
    {
        /// <summary>
        /// Запрос
        /// </summary>
        public string Query;

        /// <summary>
        /// Слова из запроса
        /// </summary>
        public List<string> Names;

        /// <summary>
        /// неименованные вершины сем. сети, соответствующие словам из запроса
        /// </summary>
        public List<Node> UnnamedNodes;

        /// <summary>
        /// Список результатов для каждого из слов
        /// </summary>
        public List<WordResult> EveryWordResult { get; set; }

        /// <summary>
        /// Связи между понятиями
        /// </summary>
        public List<string> Connections { get; set; }

        /// <summary>
        /// Результат совместного поиска слов
        /// </summary>
        public List<string> JointResult { get; set; }

        /// <summary>
        /// Обобщённый результат
        /// </summary>
        public List<string> GeneralResult { get; set; }

        /// <summary>
        /// Сообщение о выполнении запроса
        /// </summary>
        public string Message { get; set; }

        public Action<string> ExecuteSimilarQuery { get; set; }

        public QueryResult()
        {
            Names = new List<string>();
            UnnamedNodes = new List<Node>();
            EveryWordResult = new List<WordResult>();
            Connections = new List<string>();
            JointResult = new List<string>();
            GeneralResult = new List<string>();
        }

        public StackPanel Print()
        {
            var stackPanel = new StackPanel();
            Expander exp;
            //Сообщение
            if (!string.IsNullOrEmpty(Message))
                stackPanel.Children.Add(new Label {Content = Message});
            //Связи
            if (Connections.Count > 0)
            {
                var lb = new ListBox {ItemsSource = Connections};
                lb.MouseDoubleClick += (sender, args) => ExecuteSimilarQuery(lb.SelectedItem.ToString());
                exp = new Expander {Header = "Связи между понятиями", Content = lb};
                stackPanel.Children.Add(exp);
            }
            //Совместные результаты
            if (JointResult.Count > 0)
            {
                var lb = new ListBox { ItemsSource = JointResult };
                lb.MouseDoubleClick += (sender, args) => ExecuteSimilarQuery(lb.SelectedItem.ToString());
                exp = new Expander { Header = "Результат поиска", Content = lb };
                stackPanel.Children.Add(exp);
            }
            else 
            {
                //Результаты для отдельных слов
                for (int i = EveryWordResult.Count - 1; i >= 0; i--)
                {
                    if (EveryWordResult[i] != null)
                        stackPanel.Children.Add(EveryWordResult[i].Print());
                }
            }
            //Обобщённые результаты
            if (GeneralResult.Count > 0)
            {
                var lb = new ListBox { ItemsSource = GeneralResult };
                lb.MouseDoubleClick += (sender, args) => ExecuteSimilarQuery(lb.SelectedItem.ToString());
                exp = new Expander { Header = "Обобщённые результаты", Content = lb };
                stackPanel.Children.Add(exp);
            }
            return stackPanel;
        }
    }
}
