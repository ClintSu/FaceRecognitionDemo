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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PdfViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isLoaded = false;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.moonPdfPanel.PreviewMouseWheel += MoonPdfPanel_PreviewMouseWheel;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //moonPdfPanel.OpenFile("E:\\百度-人脸活体检测硬件合作标准.pdf");
            try
            {
                if (string.IsNullOrEmpty(App.PfdUri))
                {
                    return;
                }

                moonPdfPanel.OpenFile(App.PfdUri);
                moonPdfPanel.ZoomToHeight();

                this.currentPage.Text = moonPdfPanel.GetCurrentPageNumber().ToString();
                this.totalPage.Text = moonPdfPanel.TotalPages.ToString();
                _isLoaded = true;
            }
            catch (Exception)
            {
                _isLoaded = false;
            }
        }
        private void MoonPdfPanel_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_isLoaded)
            {
                this.currentPage.Text = moonPdfPanel.GetCurrentPageNumber().ToString();
            }
        }
        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.ZoomIn();
            }
        }
        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.ZoomOut();
            }
        }
        private void FacingButton_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ViewType = MoonPdfLib.ViewType.Facing;
        }
        private void SinglePageButton_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel.ViewType = MoonPdfLib.ViewType.SinglePage;
        }  
        private void PageFirst_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.GotoFirstPage();
                this.currentPage.Text = "1";//moonPdfPanel.GetCurrentPageNumber().ToString();
            }
        }
        private void PagePrevious_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.GotoPreviousPage();
                this.currentPage.Text = moonPdfPanel.GetCurrentPageNumber().ToString();
            }
        }
        private void currentPage_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                var pageNumber = int.Parse(this.currentPage.Text);
                if (pageNumber <= moonPdfPanel.TotalPages && pageNumber >= 1)
                {
                    moonPdfPanel.Zoom(pageNumber);
                }
            }
            catch (Exception)
            {

            }
        }
        private void PageNext_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.GotoNextPage();
                this.currentPage.Text = moonPdfPanel.GetCurrentPageNumber().ToString();
            }
        }
        private void PageLast_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
            {
                moonPdfPanel.GotoLastPage();
                this.currentPage.Text = moonPdfPanel.TotalPages.ToString();//moonPdfPanel.GetCurrentPageNumber().ToString();
            }
        }
        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            moonPdfPanel = null;
            this.Close();
        }
    }
}

