using AForge.Video.DirectShow;
using jg.WorkstationMachine.Controls;
using jg.WorkstationMachine.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace jg.WorkstationMachine
{
    /// <summary>
    /// PracticeViewer.xaml 的交互逻辑
    /// </summary>
    public partial class PageViewer : Window
    { 

        private string pageName;
        private Model.PageUserModel pageModel;

        public PageViewer(string pageName)
        {
            InitializeComponent();
            this.pageName = pageName;
            this.Loaded += PageViewer_Loaded;
            this.Closing += PageViewer_Closing;  
        }
        private void PageViewer_Loaded(object sender, RoutedEventArgs e)
        {
            this.imageHeader.Source = Globals.BitSource;
            this.userID.Text = Globals.UserID;
            this.userName.Text = Globals.UserName;
            this.workTime.Text = Globals.UseTime;
            this.machineID.Text = Globals.MachineID;


            pageModel = new PageUserModel();
            pageModel.PageName = pageName;

            moonPdfPanel.OpenFile(System.IO.Directory.GetCurrentDirectory() + "\\data\\" + pageName + "_学习工作页.pdf");
            this.PageTitle.Text = pageName;
            moonPdfPanel.ZoomToHeight();

        }
        private void PageViewer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            moonPdfPanel = null;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private delegate void EnableButtonMethod();
        private Timer m_timerToEnableButton;
        private delegate void DoPrintMethod(PrintDialog pdlg, DocumentPaginator paginator);
        private void DoPrint(PrintDialog pdlg, DocumentPaginator paginator)
        {
            pdlg.PrintDocument(paginator, "Page Document");
        }
        private void EnableButton()
        {
            BtnPrint.IsEnabled = true;
        }

        private void BtnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            PrintPreviewWindow previewWnd = new PrintPreviewWindow("PageDocument.xaml", pageModel, new OrderDocumentRenderer());
            previewWnd.Owner = this;
            previewWnd.ShowInTaskbar = false;
            previewWnd.ShowDialog();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            //var filePath = System.IO.Directory.GetCurrentDirectory() + "\\data\\page\\" + pageName + "_学习工作页.pdf";
            //PrintDocument pd = new PrintDocument();
            //Process p = new Process();
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.CreateNoWindow = true;
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.UseShellExecute = true;
            //startInfo.FileName = filePath;
            //startInfo.Verb = "print";
            //startInfo.Arguments = @"/p /h \" + filePath + "\"\"" + pd.PrinterSettings.PrinterName + "\"";
            //p.StartInfo = startInfo;
            //p.Start();
            //p.WaitForExit();

            MessageBox.Show("请先连接打印机。");
            return;

            BtnPrint.IsEnabled = false;
            PrintDialog pdlg = new PrintDialog();
            FlowDocument doc = PrintPreviewWindow.LoadDocumentAndRender("PageDocument.xaml", pageModel, new OrderDocumentRenderer());
            Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint), DispatcherPriority.ApplicationIdle, pdlg, ((IDocumentPaginatorSource)doc).DocumentPaginator);
            m_timerToEnableButton = new Timer(TestTimerCallback, null, 3000, Timeout.Infinite);

        }

        public void TestTimerCallback(Object state)
        {
            m_timerToEnableButton.Dispose();
            Dispatcher.BeginInvoke(new EnableButtonMethod(EnableButton));
        }
    }
}