using Meta.Vlc.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace VlcPlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        bool MAX = true;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ResizeMode = ResizeMode.NoResize;
                Player.Loaded += Player_Loaded;

                if (string.IsNullOrEmpty(App.VideoUri))
                {
                    return;
                }
                Player.LoadMedia(App.VideoUri);
                var ext = System.IO.Path.GetExtension(App.VideoUri);
                var array = App.VideoUri.Split('\\');
                this.VideoTitle.Text = array[array.Length - 1].Replace(ext, "");
                Player.Play();
            }
            catch (Exception)
            {
                this.Close();
            }     
        }

        private void Player_Loaded(object sender, RoutedEventArgs e)
        {
            this.loadingWait.Visibility = Visibility.Collapsed;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Player.Stop();
            Player.Dispose();
            ApiManager.ReleaseAll();
            base.OnClosing(e);
        }


        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            Player.PauseOrResume();
        }

        private void BtnPush_Click(object sender, RoutedEventArgs e)
        {
            MAX = !MAX;//循环。点一次全屏，再点还原。 
            if (MAX)
            {
                //this.WindowState = WindowState.Maximized;//全屏 
                //this.WindowState = WindowState.Normal;//还原  
                this.WindowState = WindowState.Maximized;//全屏     
                //this.AllowsTransparency = false;
            }
            else
            {
                this.WindowState = WindowState.Normal;//还原 
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //this.AllowsTransparency = true;
            }
        }
        private void PlayProgressSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var value = (float)(e.GetPosition(ProgressSlider).X / ProgressSlider.ActualWidth);
            ProgressSlider.Value = value;
            e.Handled = true;
        }

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GridTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void LayoutParent_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BtnPlay.IsChecked = !BtnPlay.IsChecked;
            Player.PauseOrResume();
        }
    }
}
