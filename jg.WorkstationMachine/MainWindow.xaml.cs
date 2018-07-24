using AForge.Video.DirectShow;
using jg.WorkstationMachine.Controls;
using jg.WorkstationMachine.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace jg.WorkstationMachine
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private Model.Course course;

        private Model.CourseItem courseItem;

        SRecognition sr;

        private LoginWindow login;
        public MainWindow(LoginWindow login)
        {
            InitializeComponent();
            this.login = login;
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            if (Globals.VRShow)
            {
                this.BtnVirtual.Visibility = Visibility.Visible;
            }
            else
            {
                this.BtnVirtual.Visibility = Visibility.Collapsed;
            }

            if (!Globals.IsOnline)
            {
                this.VideoPlayer.MouseMove += VideoPlayer_MouseMove;
            }


        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancellationTokenSource.Cancel();

            if (Globals.IsSpeech && sr != null)
            {
                sr.StopRec();
            }

            if (_VideoSource != null)
            {
                this.VideoPlayer.Stop();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Globals.IsSpeech)
            {
                sr = new SRecognition();
                sr.Recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
                sr.BeginRec();
            }
            VideoLoaded();
        }

        private void LoadData()
        {
            this.grid1.Visibility = Visibility.Visible;


            var sourceContent = System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "\\course.json");
            course = JsonConvert.DeserializeObject<Model.Course>(sourceContent);

            courseTitle.Text = course.courseName;

            foreach (var ci in course.courseItems)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = ci.itemName;

                Thread.Sleep(20);
                this.ListCourse.Items.Add(item);
            }

            this.ListCourse.SelectedIndex = 0;
            //courseItem = course.courseItems[1];

            grid2.Visibility = Visibility.Visible;

        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //识别出的关键词
            string text = e.Result.Text;
            switch (text)
            {
                case "打开视频":
                    //sy.Speak(e.Result.Text);
                    BtnVideo_Click(null, null);
                    break;
                case "打开教具":
                    //sy.Speak(e.Result.Text);
                    BtnVirtual_Click(null, null);
                    break;
                case "打开手册":
                    //sy.Speak(e.Result.Text);
                    BtnManual_Click(null, null);
                    break;
                case "打开工作页":
                    //sy.Speak(e.Result.Text);
                    BtnPage_Click(null, null);
                    break;
                default:
                    break;
            }
            //txtVideo.Text = text;

        }

        private void ListCourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBoxItem = this.ListCourse.SelectedItem as ListBoxItem;
            courseItem = course.courseItems.First(x => x.itemName == listBoxItem.Content.ToString());
            itemDesp.Text = courseItem.description;
        }

        /// <summary>
        /// 视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] args = new string[] { System.AppDomain.CurrentDomain.BaseDirectory + "data\\" + courseItem.playName + ".mp4" };
                Common.StartProcess(System.AppDomain.CurrentDomain.BaseDirectory + "VlcPlayer.exe", args, true);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
          
        }
        /// <summary>
        /// 教具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnVirtual_Click(object sender, RoutedEventArgs e)
        {
            var filePath = System.AppDomain.CurrentDomain.BaseDirectory + "data\\unity\\" + courseItem.unityName + ".exe";
            UnitySocketCenter.Instance.StartUnity(filePath, courseItem.itemCode, UnityModeEnum.Teaching);
        }
        /// <summary>
        /// 手册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnManual_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] args = new string[] { System.AppDomain.CurrentDomain.BaseDirectory + "data\\" + courseItem.playName + "_维修手册.pdf" };
                Common.StartProcess(System.AppDomain.CurrentDomain.BaseDirectory + "PdfViewer.exe", args, true);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
        /// <summary>
        /// 工作页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            PageViewer viewer = new PageViewer(courseItem.playName);
            viewer.ShowDialog();
        }


        #region 人脸识别模块
        /// <summary>
        /// 视频源
        /// </summary>
        VideoCaptureDevice _VideoSource = null;
        /// <summary>
        /// 线程控制
        /// </summary>
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        FrameworkElement frameworkElement = new FrameworkElement();

        //人脸识别数据
        FaceDetectResultModel faceDetect = new FaceDetectResultModel();
        //1:N 检索人脸数据
        FaceSearchResultModel faceSearch = new FaceSearchResultModel();

        private static object lockObj = new object();

        private System.Drawing.Bitmap bitmap;
        string imageBase64;
        Face face = new Face();
        int i = 5000;

        #region 离线识别

        /// <summary>
        /// 虹软SDK的AppId
        /// </summary>
        const string AppId = "8vmqWrE33nXsUyvvY1pstRsAGZeWEm9Bhwba1MCHddjP";
        /// <summary>
        /// 虹软SDK人脸检测的Key
        /// </summary>
        const string DKey = "6CcVU4nb4iwqYv8i5WoKSo7Ad3y9qs7SD8ttmQjiZm2w";
        /// <summary>
        /// 虹软SDK人脸比对的Key
        /// </summary>
        const string RKey = "6CcVU4nb4iwqYv8i5WoKSo7fGf1oo6viuV5QQ2gmYNx6";

        /// <summary>
        /// 人脸图片存放路径
        /// </summary>
        const string FaceDataPath = "d:\\FeatureData";

        /// <summary>
        /// 准备注册的人脸的序号
        /// </summary>
        int _RegisterIndex = -1;
        /// <summary>
        /// 准备注册的人脸特征值
        /// </summary>
        byte[] _RegisterFeatureData = null;

        private void PictureCapture(bool result)
        {
            try
            {
                if (result == false)
                {
                    this.imageHeader.Source = Common.BitmapToBitmapSource(Properties.Resources.noimage);
                }
                else
                {
                    if (_RegisterIndex != -1)
                    {
                        var id = ArcFace.Api.CacheFaceResults[_RegisterIndex].ID;
                        if (!string.IsNullOrEmpty(id))
                        {
                            var path = FaceDataPath + "\\image\\" + ArcFace.Api.CacheFaceResults[_RegisterIndex].ID + ".bmp";
                            if (!System.IO.File.Exists(path)) return;
                            System.Drawing.Bitmap bm = new System.Drawing.Bitmap(path);
                            
                            bm.Dispose();
                            //var first = faceSearch.user_list.First();

                            var time = DateTime.Now;
                            Globals.UserID = id;
                            Globals.UserName = "体验用户";
                            Globals.UseTime = Common.FormatDate(time);
                            Globals.BitSource = Common.BitmapToBitmapSource(bm);

                            this.imageHeader.Source = Globals.BitSource;
                            this.userID.Text = Globals.UserID;
                            this.userName.Text = Globals.UserName;
                            this.workTime.Text = Globals.UseTime;
                            this.machineID.Text = Globals.MachineID;

                            this.imageMessage.Visibility = Visibility.Visible;
                            this.backButton.Visibility = Visibility.Visible;
                            this.gridAnimation.Visibility = Visibility.Collapsed;
                            //this.backButton.Visibility = Visibility.Visible;

                            if (_VideoSource != null)
                            {
                                this.VideoPlayer.Stop();
                            }
                            this.GridVideoPlayer.Visibility = Visibility.Hidden;

                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
            }
        }

        private void FaceRegister()
        {
            if (bitmap == null) return;

            this._RegisterFeatureData = ArcFace.Api.CacheFaceResults[_RegisterIndex].GetFeatureData();
            var r = ArcFace.Api.CacheFaceResults[_RegisterIndex].Rectangle;
            r.Inflate((int)(r.Width * 0.5), (int)(r.Height * 0.5));
            if (r.X < 0)
            {
                r.Width += r.X;
                r.X = 0;
            }
            if (r.Y < 0)
            {
                r.Height += r.Y;
                r.Y = 0;
            }
            var nImg = new System.Drawing.Bitmap(r.Width, r.Height);

            using (var g = System.Drawing.Graphics.FromImage(nImg))
            {
                g.DrawImage(bitmap, new System.Drawing.Rectangle(0, 0, r.Width, r.Height), r, System.Drawing.GraphicsUnit.Pixel);
            }
            Thread.Sleep(100);
            this.imageHeader.Source = Common.BitmapToBitmapSource(nImg);
            if (_RegisterFeatureData == null)
            {
                //MessageBox.Show("没有人脸数据，请面对摄像头并点击视频");
                return;
            }
            var userId = "ID" + Common.GetTimeStamp(10); //DateTime.Now.ToString("MMddHHmmss");
            ArcFace.Api.AddFace(userId, _RegisterFeatureData, (System.Drawing.Image)(nImg));
            bitmap.Dispose();
        }

        private void VideoPlayer_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                if (ArcFace.Api.CacheFaceResults.FaceNumber == 1)
                {
                    _RegisterIndex = 0;
                    this.VideoPlayer.Cursor = System.Windows.Forms.Cursors.Hand;
                    return;
                }

                float _RateH = 1.0F * this.VideoPlayer.Height / this._VideoSource.VideoResolution.FrameSize.Height;
                float _RateW = 1.0F * this.VideoPlayer.Width / this._VideoSource.VideoResolution.FrameSize.Width;

                var x = e.X / _RateW;
                var y = e.Y / _RateH;
                _RegisterIndex = ArcFace.Api.CacheFaceResults.Items.IndexOf(ArcFace.Api.CacheFaceResults.Items.Take(ArcFace.Api.CacheFaceResults.FaceNumber).FirstOrDefault(ii => x >= ii.Rectangle.Left && x <= ii.Rectangle.Right && y >= ii.Rectangle.Top && y <= ii.Rectangle.Bottom));


                this.VideoPlayer.Cursor = _RegisterIndex == -1 ? System.Windows.Forms.Cursors.Default : System.Windows.Forms.Cursors.Hand;

            }
            catch (Exception ex)
            {
                //logger.Error(ex);
            }
        }
        #endregion

        /// <summary>
        /// 摄像头初始化
        /// </summary>
        private void VideoLoaded()
        {
            try
            {
                this.imageHeader.Source = Common.BitmapToBitmapSource(Properties.Resources.noimage);
                this.imageAni.Source = Common.BitmapToBitmapSource(Properties.Resources.face);
                this.userID.Text = "";
                this.userName.Text = "";
                this.workTime.Text = "";
                this.machineID.Text = "";

                _VideoSource = Video.GetVideoSource();
                //获取摄像头参数
                if (null == _VideoSource)
                {
                    System.Windows.MessageBox.Show("没有检测到摄像头");
                    this.Close();
                    return;
                }
                this.VideoPlayer.VideoSource = _VideoSource;
                this.VideoPlayer.Start();

                //线程控制人脸采集
                Task.Factory.StartNew(() =>
                {
                    Task.Delay(500).Wait();
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            if (_VideoSource == null|| !_VideoSource.IsRunning)
                            { //usb接口异常，重新连接摄像头
                                this.Dispatcher.BeginInvoke(new Action(()=> {
                                    _VideoSource = Video.GetVideoSource();
                                    if (_VideoSource != null)
                                    {
                                        _VideoSource.Stop();
                                        _VideoSource.WaitForStop();
                                        this.VideoPlayer.Stop();

                                        this.VideoPlayer.VideoSource = _VideoSource;
                                        this.VideoPlayer.Start();
                                    }
                                    
                                }), null); 
                            }
                            if (Globals.IsOnline)
                            {
                                bitmap = this.VideoPlayer.GetCurrentVideoFrame();
                                if (bitmap == null) continue;
                                lock (lockObj)
                                {
                                    imageBase64 = Common.BitmapToBase64(bitmap);
                                    i = 200;
                                    var result = FaceSearch();
                                    if (result)
                                    {
                                        FaceDetect();
                                        this.Dispatcher.BeginInvoke(new Action(PictureControl), null);
                                        cancellationTokenSource.Cancel();
                                    }
                                    else
                                    {

                                        faceDetect = null;
                                        if (!FaceCollect())
                                        {
                                            bitmap.Dispose();
                                            imageBase64 = null;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                Thread.Sleep(200);


                                if (!ArcFace.Api.Init(out string msg, AppId, DKey, RKey, 50, 500, FaceDataPath))
                                {
                                    System.Windows.MessageBox.Show(msg);
                                    this.Close();
                                    return;
                                }

                                var img = this.VideoPlayer.GetCurrentVideoFrame();
                                if (img == null)
                                {
                                    continue;
                                }

                                bool result = ArcFace.Api.FaceMatch(img);

                                this.Dispatcher.BeginInvoke(new Action<bool>(PictureCapture), new object[] { result });

                                if (_RegisterIndex != -1 && ArcFace.Api.CacheFaceResults.Items.Count > 0)
                                {

                                    if (result && ArcFace.Api.CacheFaceResults[_RegisterIndex].Score != 0)
                                    {
                                        img.Dispose();
                                        cancellationTokenSource.Cancel();
                                    }

                                    if (string.IsNullOrEmpty(ArcFace.Api.CacheFaceResults[_RegisterIndex].ID)
                                    && ArcFace.Api.CacheFaceResults[_RegisterIndex].Score == 0
                                    && ArcFace.Api.CacheFaceResults[_RegisterIndex].Rectangle != null)
                                    {
                                        try
                                        {
                                            if (bitmap == null)
                                            {
                                                bitmap = img.Clone(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), img.PixelFormat);
                                                img.Dispose();
                                                this.Dispatcher.BeginInvoke(new Action(FaceRegister), null);
                                                //cancellationTokenSource.Cancel();
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            //throw ex;    
                                        }
                                    }
                                    else
                                    {
                                        img.Dispose();
                                    }
                                }
                                else
                                {
                                    img.Dispose();
                                }
                            }


                            //检测到用户，使用UserGet接口
                            //未检测到用户，使用FaceDetect接口,并注册用户
                            Thread.Sleep(100);
                        }
                        catch (Exception ex)
                        {
                            //throw;

                        }
                    }
                }, cancellationTokenSource.Token);

            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// 人脸查找
        /// </summary>
        private bool FaceSearch()
        {
            try
            {
                faceSearch = null;
                var jObject = face.Search(imageBase64, "BASE64", Globals.FaceGroupId);
                var error_code = int.Parse(jObject["error_code"].ToString());

                if (error_code > 0)
                {
                    faceSearch = null;
                    return false;
                }
                else
                {
                    var result = jObject["result"];
                    faceSearch = JsonConvert.DeserializeObject<FaceSearchResultModel>(result.ToString());

                    foreach (var item in faceSearch.user_list)
                    {
                        if (item.score >= 80)
                        {
                            return true;
                        }
                    }
                    faceSearch = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 人脸检测
        /// </summary>
        private void FaceDetect()
        {
            Dictionary<string, object> options = new Dictionary<string, object>();
            //options["face_field"] = "age,beauty,expression,faceshape,gender,glasses,landmark,race,quality,facetype";

            // var imageBase64 = Common.BitmapToBase64(bitmap);

            var jObject = face.Detect(imageBase64, "BASE64", options);

            var error_code = int.Parse(jObject["error_code"].ToString());

            if (error_code > 0)
            {
                faceSearch = null;
            }
            else
            {
                var result = jObject["result"];
                faceDetect = JsonConvert.DeserializeObject<FaceDetectResultModel>(result.ToString());
            }
        }
        /// <summary>
        /// 人脸采集
        /// </summary>
        /// <returns></returns>
        private bool FaceCollect()
        {
            var faces = new JArray
            {
                new JObject
                {
                    { "image", imageBase64},
                    { "image_type", "BASE64"},
                    { "face_field", "quality"}
                }};

            var jObject = face.Faceverify(faces);
            if (jObject == null) return false;

            var error_code = int.Parse(jObject["error_code"].ToString());
            if (error_code > 0)
            {
                return false;
            }
            var result = jObject["result"];
            var verifyResult = JsonConvert.DeserializeObject<FaceVerifyResultModel>(result.ToString());
            if (verifyResult.face_liveness <= 0.933801)
            {
                return false;
            }

            var faceModel = verifyResult.face_list.First();

            //质量检测
            bool boolValue = false;
            boolValue =
                faceModel.quality.occlusion.left_eye >= 0.6
               && faceModel.quality.occlusion.right_eye >= 0.6
               && faceModel.quality.occlusion.nose >= 0.7
               && faceModel.quality.occlusion.mouth >= 0.7
               && faceModel.quality.occlusion.right_cheek >= 0.8
               && faceModel.quality.completeness == 0
               && faceModel.quality.blur >= 0.7
               && faceModel.quality.illumination <= 40
               && faceModel.angle.yaw >= 20
               && faceModel.angle.pitch >= 20
               && faceModel.angle.roll >= 20;

            if (boolValue)
            {
                return false;
            }

            var userId = "ID" + Common.GetTimeStamp(10); //DateTime.Now.ToString("MMddHHmmss");
            jObject = face.UserAdd(imageBase64, "BASE64", Globals.FaceGroupId, userId);
            error_code = int.Parse(jObject["error_code"].ToString());

            if (error_code > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 人脸截图
        /// </summary>
        private void PictureControl()
        {
            //Thread.Sleep(2000);
            try
            {
                var location = faceDetect.face_list.First().location;
                var r = new System.Drawing.Rectangle((int)location.left, (int)location.top, (int)location.width, (int)location.height);

                r.Inflate((int)(r.Width * 0.5), (int)(r.Height * 0.5));
                if (r.X < 0)
                {
                    r.Width += r.X;
                    r.X = 0;
                }
                if (r.Y < 0)
                {
                    r.Height += r.Y;
                    r.Y = 0;
                }
                var nImg = new System.Drawing.Bitmap(r.Width, r.Height);
                using (var g = System.Drawing.Graphics.FromImage(nImg))
                {
                    g.DrawImage(bitmap, new System.Drawing.Rectangle(0, 0, r.Width, r.Height), r, System.Drawing.GraphicsUnit.Pixel);
                }
                

                var first = faceSearch.user_list.First();
                var time = DateTime.Now;
                Globals.UserID = first.user_id; 
                Globals.UserName = "体验用户";
                Globals.UseTime = Common.FormatDate(time);
                Globals.BitSource = Common.BitmapToBitmapSource(nImg);
                this.imageHeader.Source = Globals.BitSource;

                this.userID.Text = Globals.UserID;
                this.userName.Text = Globals.UserName;
                this.workTime.Text = Globals.UseTime;
                this.machineID.Text = Globals.MachineID;


                this.imageMessage.Visibility = Visibility.Visible;
                this.backButton.Visibility = Visibility.Visible;
                this.gridAnimation.Visibility = Visibility.Collapsed;
                //this.backButton.Visibility = Visibility.Visible;

                bitmap.Dispose();
                imageBase64 = null;
                if (_VideoSource != null)
                {
                    this.VideoPlayer.Stop();
                }
                this.GridVideoPlayer.Visibility = Visibility.Hidden;

                Task.Factory.StartNew(()=>
                {
                    Thread.Sleep(200);
                    this.Dispatcher.BeginInvoke(new Action(LoadData), null);
                });
               
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
            }
        }


        /// <summary>
        /// 绘制扫描线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoPlayer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
           
            if (Globals.IsOnline)
            {
                //int h = this.VideoPlayer.Height;
                //int w = this.VideoPlayer.Width;

                //System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.DeepSkyBlue);
                //pen.Width =1.5;

                //for (; i < h - 100;)
                //{
                //    e.Graphics.DrawLine(pen, w / 4, i, w - w / 4, i);
                //    i += 15;
                //}
            }
            else
            {
                try
                {
                    System.Drawing.Pen _PenFace;
                    _PenFace = new System.Drawing.Pen(System.Drawing.Color.Red, 1);
                    _PenFace.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                    _PenFace.DashPattern = new float[] { 5, 5 };


                    float _RateH = 1.0F * this.VideoPlayer.Height / this._VideoSource.VideoResolution.FrameSize.Height;
                    float _RateW = 1.0F * this.VideoPlayer.Width / this._VideoSource.VideoResolution.FrameSize.Width;

                    e.Graphics.ScaleTransform(_RateW, _RateH);
                    for (int i = 0; i < ArcFace.Api.CacheFaceResults.FaceNumber; i++)
                    {
                        if (string.IsNullOrEmpty(ArcFace.Api.CacheFaceResults[i].ID))
                        {
                            //e.Graphics.DrawRectangle(_PenFace, ArcFace.Api.CacheFaceResults[i].Rectangle);
                        }
                        else
                        {
                            e.Graphics.DrawRectangle(System.Drawing.Pens.Green, ArcFace.Api.CacheFaceResults[i].Rectangle);

                        }
                    }
                }
                catch (Exception ex)
                {

                    //logger.Error(ex);
                }
            }
        }

        #endregion

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            login.Show();
        }

    }
}
