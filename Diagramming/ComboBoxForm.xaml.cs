﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DiagramControls
{
    /// <summary>
    /// Interaction logic for ComboBoxForm.xaml
    /// </summary>
    public partial class ComboBoxForm : Window
    {
        public ComboBoxForm()
        {
            InitializeComponent();
            this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        //public void RefreshValue(List<Node> items)
        //{
        //    ComboBox1.Items.Clear();
        //    for (int i = 0;i<items.Count;i++)
        //    {
        //        ComboBox1.Items.Add(items[i]);
        //    }
        //    if (items.Count>0)
        //        ComboBox1.SelectedIndex = 0;
        //}
        /// <summary>
        /// string подразумевается для дуг
        /// </summary>
        /// <param name="items"></param>
        public void RefreshValue(IEnumerable<string> items)
        {
            ComboBox1.Items.Clear();
            for (int i = 0; i < items.Count(); i++)
            {
                if (!items.ElementAt(i).Contains("_#"))
                    ComboBox1.Items.Add(items.ElementAt(i));
            }
            ComboBox1.SelectedIndex= 0;
        }
        public object ReturnValue()   
        {
            return (ComboBox1.SelectedItem);
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

   }
}
