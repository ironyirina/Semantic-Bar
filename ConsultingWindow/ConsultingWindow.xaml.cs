﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Consulting;
using Kernel;

namespace ConsultingWindow
{
    /// <summary>
    /// Interaction logic for ConsultingWindow.xaml
    /// </summary>
    public partial class ConsWindow
    {
        #region Variables
        /// <summary>
        /// Текст запроса
        /// </summary>
        public string Query
        {
            get { return tbQuery.Text; }
            set { tbQuery.Text = value; }
        }
        /// <summary>
        /// Список всех запросов
        /// </summary>
        private static readonly List<string> QueryConsequence;
        /// <summary>
        /// Указатель на текущий запрос в списке
        /// </summary>
        private static int _currentQueryIndex;

        private bool _newQuery;
        #endregion

        #region Initialization
        public ConsWindow()
        {
            InitializeComponent();
        }

        static ConsWindow()
        {
            _currentQueryIndex = -1;
            QueryConsequence = new List<string>();
            Search = new RoutedUICommand("Search", "Search", typeof(ConsWindow));
            Prev = new RoutedUICommand("Предыдущий запрос", "Prev", typeof(ConsWindow));
            Prev.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Control));
            Next = new RoutedUICommand("Следующий запрос", "Next", typeof(ConsWindow));
            Next.InputGestures.Add(new KeyGesture(Key.Y, ModifierKeys.Control));
        } 
        #endregion

        #region Search
        private void PrintResult(QueryResult res)
        {
            Query = res.Query;
            panelRes.Children.Clear();
            var ss = res.Print().Children;
            for (int i = ss.Count - 1; i >= 0; i--)
            {
                UIElement child = ss[i];
                ss.RemoveAt(i);
                panelRes.Children.Add(child);
            }
        }

        public static RoutedUICommand Search { get; private set; }

        private void SearchExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var searcher = new Searcher(Query, SearchAnotherQuery, cbSyn.IsChecked == true, cbParent.IsChecked == true);
            QueryResult res = searcher.Search();
            PrintResult(res);

            if (_newQuery)
                for (int i = QueryConsequence.Count - 1; i > _currentQueryIndex; i--)
                    QueryConsequence.RemoveAt(i);

            if (_currentQueryIndex >= QueryConsequence.Count - 1)
            {
                QueryConsequence.Add(Query);
                _currentQueryIndex++;
            }
        }

        private void SearchAnotherQuery(string query)
        {
            Query = query;
            SearchExecuted(null, null);
        }
        #endregion

        #region Prev/Next

        #region Prev
        public static RoutedUICommand Prev { get; private set; }

        private void PrevExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _currentQueryIndex--;
            _newQuery = false;
            SearchAnotherQuery(QueryConsequence[_currentQueryIndex]);
            _newQuery = true;
        }

        private void PrevCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _currentQueryIndex > 0 && QueryConsequence.Count >= 2;
        } 
        #endregion

        #region Next
        public static RoutedUICommand Next { get; private set; }

        private void NextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            _currentQueryIndex++;
            _newQuery = false;
            SearchAnotherQuery(QueryConsequence[_currentQueryIndex]);
            _newQuery = true;
        }

        private void NextCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _currentQueryIndex < QueryConsequence.Count - 1 && QueryConsequence.Count >= 2;
        }
        #endregion

        #endregion
    }
}
