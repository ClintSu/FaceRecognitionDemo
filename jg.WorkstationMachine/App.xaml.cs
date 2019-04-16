using jg.WorkstationMachine.Controls;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace jg.WorkstationMachine
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();// 获取控制台句柄

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private static string Unique = "jg.WorkstationMachine";


        [STAThread]
        static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();
                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            ActivateWindow();
            return true;
        }

        public static void ActivateWindow(Process process = null)
        {
            if (process == null)
            {
                process = Process.GetCurrentProcess();
            }
            SingleInstance<App>.ActivateWindow(process);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Model.Face face = new Model.Face();

            //for (int i = 2; i < 51; i++)
            //{
            //    var tt = "group" + i.ToString().PadLeft(2,'0');
            //    face.GroupAdd(tt);
            //    System.Threading.Thread.Sleep(100);
            //}
           
            Globals.UnitySocket = new SocketHelper(Globals.SocketIP, Globals.SocketPort);
            Globals.UnitySocket.StartServer();
        }
    }
}
