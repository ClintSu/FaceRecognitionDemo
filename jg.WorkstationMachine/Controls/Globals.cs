using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace jg.WorkstationMachine.Controls
{
    public class Globals
    {
        public static readonly string AppName = ConfigurationManager.AppSettings["AppName"];
        public static readonly string SocketIP = ConfigurationManager.AppSettings["SocketIP"];
        public static readonly int SocketPort = int.Parse(ConfigurationManager.AppSettings["SocketPort"]);
        public static readonly string FaceGroupId = ConfigurationManager.AppSettings["FaceGroupId"];
        public static readonly string MachineID = ConfigurationManager.AppSettings["MachineID"];
        public static readonly bool IsOnline = bool.Parse(ConfigurationManager.AppSettings["IsOnline"]);
        public static readonly bool IsSpeech = bool.Parse(ConfigurationManager.AppSettings["IsSpeech"]);
        public static readonly bool VRShow = bool.Parse(ConfigurationManager.AppSettings["VRShow"]);

        /// <summary>
        /// Socket管理
        /// </summary>
        public static SocketHelper UnitySocket;
        /// <summary>
        /// Socket客户端
        /// </summary>
        public static System.Collections.ArrayList WorkerSocketList = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());


        public static string UserID { get; set; }
        public static string UserName { get; set; }
        public static string UseTime { get; set; }

        public static BitmapSource BitSource { get; set; }

    }
}
