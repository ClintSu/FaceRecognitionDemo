﻿using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jg.WorkstationMachine.Controls
{
    public class Video
    {
        public static VideoCaptureDevice GetVideoSource()
        {
            try
            {
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count == 0)//没有检测到摄像头
                    return null;

                var videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);//连接第一个摄像头
                if (videoSource.VideoCapabilities.Count() == 0) return null;

                var videoResolution = videoSource.VideoCapabilities[0];//.First(ii => ii.FrameSize.Width == p.VideoSource.VideoCapabilities.Max(jj => jj.FrameSize.Width)); //获取摄像头最高的分辨率

                //p.ByteCount = videoResolution.BitCount / 8;
                videoSource.VideoResolution = videoResolution;
                return videoSource;
            }
            catch (Exception)
            {
                return null;
            }
          
        }

    }
}
