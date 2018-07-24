using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PdfViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string PfdUri;

        [STAThread]
        static void Main(string[] args)
        {
            if (args == null)
            {
                return;
            }
            PfdUri = args[0];

            //var name = "百度";
            //PfdUri = @"D:\项目管控\工位机项目\2\jg.WorkstationMachine\jg.WorkstationMachine\bin\Debug\data\pdf\" + name + ".pdf" ;


            var application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }
}
