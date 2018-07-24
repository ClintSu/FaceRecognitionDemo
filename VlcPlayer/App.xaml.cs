using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VlcPlayer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string VideoUri;

        [STAThread]
        static void Main(string[] args)
        {

            if (args == null)
            {
                return;
            }
            VideoUri = args[0];

            var application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }
}
