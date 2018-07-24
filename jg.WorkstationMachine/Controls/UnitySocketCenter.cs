using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace jg.WorkstationMachine.Controls
{
    public class UnitySocketCenter
    {
        public int DbRecordId = 0;
        private static readonly object locker = new object();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static UnitySocketCenter uspCenter;
        private UnityModeEnum mode = UnityModeEnum.Teaching;

        private int taskType = 0;

        public UnitySocketCenter()
        {

        }

        public static UnitySocketCenter Instance
        {
            get
            {
                lock (locker)
                {
                    if (uspCenter == null)
                    {
                        uspCenter = new UnitySocketCenter();
                    }
                }
                return uspCenter;
            }
        }

        /// <summary>
        /// 启动指定Unity,演示、练习
        /// </summary>
        /// <param name="taskItem"></param>
        public void StartUnity(string toolPath, string taskId, UnityModeEnum unityMode)
        {
            this.mode = unityMode; //Teaching\Practice
            try
            {

                //🐶准备启动参数

                var args = new string[2] { Globals.SocketPort.ToString(), Globals.SocketIP };

                //🐶准备启动初始化消息
                UnitySocketToInitModel socketToUnityInit = new UnitySocketToInitModel();
                socketToUnityInit.Mode = Enum.GetName(typeof(UnityModeEnum), mode);
                socketToUnityInit.UserID = "";
                socketToUnityInit.UserName = "";
                socketToUnityInit.TaskID = taskId;
                socketToUnityInit.ExamTime = "0";

                var msg = JsonConvert.SerializeObject(socketToUnityInit);

                if (!System.IO.File.Exists(toolPath))
                {
                    MessageBox.Show("无对应教具可用。");
                    return;
                }


                //😄😄启动Unity，监控连接，发送消息😄😄
                Task task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Common.StartProcess(toolPath, args); //启动Unity
                        }
                    catch (Exception)
                    {
                    }
                    while (Globals.UnitySocket.UnityTag == 0)
                    {
                        System.Threading.Thread.Sleep(200);//确认Socket连接成功
                        }
                    Globals.UnitySocket.SendMsgToAllClient(msg);//广播消息
                        Globals.UnitySocket.UnityTag = 0;
                });
                logger.Debug(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}