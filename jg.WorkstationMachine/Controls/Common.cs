using NLog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Windows;
using System.Windows.Media.Imaging;

namespace jg.WorkstationMachine.Controls
{
    public class Common
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr ptr);

        /// <summary>
        /// Bitmap转换过成Image Source
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            IntPtr ptr = bitmap.GetHbitmap();

            BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ptr,
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ptr);
            return source;
        }
        /// <summary>
        /// Bitmap转换为字符串     
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static String BitmapToBase64(Bitmap bmp)
        {
            // 要返回的字符串
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static System.Windows.Controls.Image BitmapToImage(Bitmap Bi)
        {
            MemoryStream ms = new MemoryStream();
            Bi.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bImage = new BitmapImage();
            bImage.BeginInit();
            bImage.StreamSource = new MemoryStream(ms.ToArray());
            bImage.EndInit();
            ms.Dispose();
            Bi.Dispose();
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = bImage;
            return image;
        }


        /// <summary>
        /// 工位机Mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (var mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac.Trim();
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }


        /// <summary>
        /// 格式化日期【2017-04-11 15:20:00】
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns></returns>
        public static string FormatDate(DateTime dt)
        {
           /* return dt.ToString("yyyy-MM-dd HH:mm:ss")*/;
            return dt.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <param name="len">长度，默认13位</param>
        /// <returns></returns>
        public static string GetTimeStamp(int len = 13)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string gts = Convert.ToInt64(ts.TotalMilliseconds).ToString();
            return gts.Substring(0, len);
        }

        /// <summary>
        /// 启动应用程序
        /// </summary>
        /// <param name="filename">文件名(带路径)</param>
        /// <param name="args">参数</param>
        /// <param name="wait">是否等待完成</param>
        /// <returns></returns>
        public static bool StartProcess(string filename, string[] args, bool wait = false)
        {
            try
            {
               
                string s = "";

                if (args != null)
                {
                    foreach (string arg in args)
                    {
                        s = s + arg + ",";
                    }
                    s = s.Trim();
                    s = s.Substring(0, s.Length - 1);
                }

                System.Diagnostics.Process myprocess = new System.Diagnostics.Process();
                ProcessStartInfo startInfo = new ProcessStartInfo(filename, s);
                myprocess.StartInfo = startInfo;
                myprocess.StartInfo.UseShellExecute = false;
                myprocess.Start();
                if (wait)
                {
                    myprocess.WaitForExit(); //等待执行完成
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("当前教具包出现异常！原因：" + ex.Message);
                throw ex;// ("当前教具包出现异常！原因：" + ex.Message);
            }
            //return false;
        }
    }
}