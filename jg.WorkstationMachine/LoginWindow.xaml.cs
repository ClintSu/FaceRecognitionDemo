using jg.WorkstationMachine.Controls;
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

namespace jg.WorkstationMachine
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            this.appName.Text = Globals.AppName;
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(this);
            this.Hide();
            main.ShowDialog();
            this.Show();
        }
    }
}
