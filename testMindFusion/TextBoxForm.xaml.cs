using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Kernel;

namespace textMindFusion
{
    /// <summary>
    /// Interaction logic for TextBoxForm.xaml
    /// </summary>
    public partial class TextBoxForm : Window
    {
        SemanticWeb myWeb;
        public TextBoxForm(SemanticWeb myWeb, string Name)
        {
            InitializeComponent();
            this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            TextboxBox1.Text = Name;
            TextboxBox1.Focus();
            this.myWeb = myWeb;
        }

        public void RefreshValue()
        {
            TextboxBox1.Text="";
        }
        public string ReturnValue()
        {
            return (TextboxBox1.Text);
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (NameExist(TextboxBox1.Text))
                MessageBox.Show("такое имя уже существует");
            else
                this.DialogResult = true;
        }
        bool NameExist(string name)
        {
            for (int i = 0; i < myWeb.Nodes.Count; i++)
            {
                if (name == myWeb.Nodes[i].Name)
                    return true;
            }
            return false;

        }
    }
}
