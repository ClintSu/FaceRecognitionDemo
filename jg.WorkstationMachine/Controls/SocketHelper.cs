using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace jg.WorkstationMachine.Controls
{
    public class QueuePool
    {
        //消息队列
        public static Queue<string> messages = new Queue<string>();
    }

    public class SocketHelper
    {
        public AsyncCallback pfnWorkerCallBack;
        public int UnityTag;
        private static byte[] result = new byte[1024];
        private int clientCount = 0;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private Socket serverSocket;

        public SocketHelper(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            //DispatcherHelper.Initialize();
        }

        private string ip { get; set; }
        private int port { get; set; }

        /// <summary>
        /// 关闭Socket服务
        /// </summary>
        public void QuitServer()
        {
            if (serverSocket != null)
                serverSocket.Close();

            Socket workerSocket = null;
            for (int i = 0; i < Globals.WorkerSocketList.Count; i++)
            {
                workerSocket = (Socket)Globals.WorkerSocketList[i];
                if (workerSocket != null)
                {
                    workerSocket.Close();
                    workerSocket = null;
                }
            }
        }

        /// <summary>
        /// 发送消息给指定客户端
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clientNumber"></param>
        public void SendMsgToClient(string msg, int clientNumber)
        {
            try
            {
                byte[] byData = System.Text.Encoding.UTF8.GetBytes(msg);
                Socket workerSocket = (Socket)Globals.WorkerSocketList[clientNumber - 1];
                if (workerSocket != null)
                {
                    if (workerSocket.Connected)
                        workerSocket.Send(byData);
                }
            }
            catch (SocketException se)
            {
                logger.Error(se.Message);
            }
        }

        /// <summary>
        /// 广播消息给客户端
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clientNumber"></param>
        public void SendMsgToAllClient(string msg)
        {
            try
            {
                byte[] byData = System.Text.Encoding.UTF8.GetBytes(msg);
                foreach (Socket workerSocket in Globals.WorkerSocketList)
                {
                    if (workerSocket != null)
                    {
                        if (workerSocket.Connected)
                            workerSocket.Send(byData);
                    }
                }  
            }
            catch (SocketException se)
            {
                logger.Error(se.Message);
            }
        }

        /// <summary>
        /// 广播给所有客户端
        /// </summary>
        /// <param name="msg"></param>
        public void SendToClient(string msg)
        {
            try
            {
                byte[] byData = System.Text.Encoding.UTF8.GetBytes(msg);
                Socket workerSocket = null;
                for (int i = 0; i < Globals.WorkerSocketList.Count; i++)
                {
                    workerSocket = (Socket)Globals.WorkerSocketList[i];
                    if (workerSocket != null)
                    {
                        if (workerSocket.Connected)
                            workerSocket.Send(byData);
                    }
                }
            }
            catch (SocketException se)
            {
                logger.Error(se.Message);
            }
        }

        /// <summary>
        /// 开启Socket服务
        /// </summary>
        public void StartServer()
        {
            IPAddress ipa = IPAddress.Parse(ip);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ipa, port));
            serverSocket.Listen(2);

            //创建Call Back为任意客户端连接
            serverSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);

            //启动消息队列监控
            Thread thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 处理返回信息
        /// </summary>
        /// <param name="number"></param>
        /// <param name="msg"></param>
        private void DealReceivedMsg(int number, string msg)
        {
            if (msg == "\0")
            {
                logger.Info("客户端" + number + ",已停止。");
                Socket socket = (Socket)Globals.WorkerSocketList[number - 1];
                socket.Close();
                socket.Dispose();
                return;
            }
            QueuePool.messages.Enqueue(msg); //消息放入队列中
            logger.Info("客户端" + number + ",接收到数据：" + msg + "\n");
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                //创建一个新Socket
                Socket workerSocket = serverSocket.EndAccept(asyn);
                //递增客户端数目，用作标记客户端
                Interlocked.Increment(ref clientCount);
                //添加客户端到数组中记录
                Globals.WorkerSocketList.Add(workerSocket);
                //用于标记客户端已连接
                UnityTag = clientCount;
                //记录客户端变化
                RefreshConnectedClientList();
                //指定这个Socket处理接收到的数据
                WaitForData(workerSocket, clientCount);
                // Main Socket继续等待客户端的连接
                serverSocket.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (ObjectDisposedException)
            {
                logger.Info("客户端连接: Socket 已关闭。");
                //Messenger.Default.Send<int>(95, "MagicControl");
            }
            catch (SocketException se)
            {
                logger.Error(se.Message);
            }
        }

        //Call Back,Socket检测客户端发送数据
        private void OnDataReceived(IAsyncResult asyn)
        {
            SocketPacket sp = (SocketPacket)asyn.AsyncState;
            try
            {
                int receive = sp.CurrentSocket.EndReceive(asyn);
                char[] chars = new char[receive + 1];
                Decoder decoder = Encoding.UTF8.GetDecoder();
                int charLen = decoder.GetChars(sp.dataBuffer, 0, receive, chars, 0);
                System.String data = new System.String(chars);
                //接受客户端数据

                //处理客户端数据！！！！！！！
                DealReceivedMsg(sp.ClientNO, data);

                //回复客户端
                //string replyMsg = sp.ClientNO.ToString();
                //byte[] byData = Encoding.UTF8.GetBytes(replyMsg);

                //Socket workerSocket = (Socket)sp.CurrentSocket;
                //workerSocket.Send(byData);

                WaitForData(sp.CurrentSocket, sp.ClientNO);
            }
            catch (ObjectDisposedException)
            {

                while (QueuePool.messages.Count > 0)
                { //提交未完成前，延迟处理🐶
                    System.Threading.Thread.Sleep(50);
                }

                logger.Error("数据接收时: Socket 已关闭。");
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10054) // 连接被管道重置(客户端主动退出连接)
                {
                    string msg = "Client " + sp.ClientNO + " 断开连接" + "\n";
                    logger.Info(msg);

                    while (QueuePool.messages.Count > 0)
                    { //提交未完成前，延迟处理🐶
                        System.Threading.Thread.Sleep(50);
                    }

                    Globals.WorkerSocketList[sp.ClientNO - 1] = null;
                    RefreshConnectedClientList();
                }
                else
                    logger.Error("数据接收时错误: " + se.Message);
            }
        }

        private void RefreshConnectedClientList()
        {
            for (int i = 0; i < Globals.WorkerSocketList.Count; i++)
            {
                string clientKey = Convert.ToString(i + 1);
                Socket workerSocket = (Socket)Globals.WorkerSocketList[i];
                if (workerSocket != null)
                {
                    if (workerSocket.Connected)
                        logger.Info("Client [" + clientKey + "] IP:" + (workerSocket.RemoteEndPoint as IPEndPoint).Address.ToString());
                }
            }
        }

        private void ScanQueue()
        {
            while (QueuePool.messages.Count > 0)
            {
                string msg = QueuePool.messages.Dequeue();

                //if (msg.ToUpper() == "OK\0")
                //{
                //    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                //    {
                //        Messenger.Default.Send<int>(94, "MagicControl");
                //    });
                //}
                //if (msg.StartsWith("{\"MessageType\":"))
                //{
                //    Messenger.Default.Send<string>(msg, "SocketMsg");
                //}
                Thread.Sleep(20);
            }
        }

        private void threadStart()
        {
            while (true)
            {
                if (QueuePool.messages.Count > 0)
                {
                    try
                    {
                        ScanQueue();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                    }
                }
                else
                {
                    //没有任务，休息2秒钟
                    Thread.Sleep(1000);
                }
            }
        }

        //等待客户端的数据
        private void WaitForData(Socket skt, int clientNumber)
        {
            try
            {
                if (pfnWorkerCallBack == null)
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);

                SocketPacket socketPacket = new SocketPacket(skt, clientNumber);
                skt.BeginReceive(socketPacket.dataBuffer, 0, socketPacket.dataBuffer.Length, SocketFlags.None, pfnWorkerCallBack, socketPacket);
            }
            catch (SocketException se)
            {
                logger.Warn("WaitForData has warn:" + se.Message);
            }
        }
    }

    internal class SocketPacket
    {
        public int ClientNO;
        public Socket CurrentSocket;
        public byte[] dataBuffer = new byte[1024 * 1024 * 1];

        public SocketPacket(Socket socket, int clientNumber)
        {
            CurrentSocket = socket;
            ClientNO = clientNumber;
        }
    }
}