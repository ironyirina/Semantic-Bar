using System.Windows;
using Kernel;

namespace DiagramControls
{
    /// <summary>
    /// Interaction logic for TextBoxForm.xaml
    /// </summary>
    public partial class TextBoxForm : Window
    {
        public TextBoxForm(string Name)
        {
            InitializeComponent();
            this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            TextboxBox1.Text = Name;
            TextboxBox1.Focus();
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
            if (TextboxBox1.Text != string.Empty && NameExist(TextboxBox1.Text))
                MessageBox.Show("такое имя уже существует");
            else
                this.DialogResult = true;
        }
        bool NameExist(string name)
        {
            for (int i = 0; i < SemanticWeb.Web().Nodes.Count; i++)
            {
                if (name == SemanticWeb.Web().Nodes[i].Name)
                    return true;
            }
            return false;

        }
    }
}
