using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace jg.WorkstationMachine.Controls
{
    public class SRecognition
    {
        public SpeechRecognitionEngine Recognizer = null;//语音识别引擎  
        private DictationGrammar dictationGrammar = null; //自然语法  

        //重载构造函数
        public SRecognition() : this(new string[] {"打开视频","打开教具","打开工作页","打开工作页"})
        {

        }

        public SRecognition(string[] fg) //创建关键词语列表  
        {
            CultureInfo cultureInfo = new CultureInfo("zh-CN");
            foreach (RecognizerInfo config in SpeechRecognitionEngine.InstalledRecognizers())//获取所有语音引擎  
            {
                if (config.Culture.Equals(cultureInfo) && config.Id == "MS-2052-80-DESK")

                {
                    Recognizer = new SpeechRecognitionEngine(config);
                    break;
                }//选择识别引擎
            }
            if (Recognizer != null)
            {
                InitializeSpeechRecognitionEngine(fg);//初始化语音识别引擎  
                dictationGrammar = new DictationGrammar();
            }
            else
            {
                MessageBox.Show("创建语音识别失败");
            }
        }
      
        private void InitializeSpeechRecognitionEngine(string[] fg)
        {
            Recognizer.SetInputToDefaultAudioDevice();//选择默认的音频输入设备  
            Grammar customGrammar = CreateCustomGrammar(fg);
            //根据关键字数组建立语法  
            Recognizer.UnloadAllGrammars();
            Recognizer.LoadGrammar(customGrammar);
            //加载语法    
        }
        public void BeginRec()//关联窗口控件  
        {
            TurnSpeechRecognitionOn();
            TurnDictationOn();
        }
        public void StopRec()//停止语音识别引擎  
        {
            TurnDictationOff();
            TurnSpeechRecognitionOff();  
        }
        public virtual Grammar CreateCustomGrammar(string[] fg) //创造自定义语法  
        {
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(new Choices(fg));
            return new Grammar(grammarBuilder);
        }
        private void TurnSpeechRecognitionOn()//启动语音识别函数  
        {
            if (Recognizer != null)
            {
                Recognizer.RecognizeAsync(RecognizeMode.Multiple);
                //识别模式为连续识别  
            }
            else
            {
                MessageBox.Show("创建语音识别失败");
            }
        }
        private void TurnSpeechRecognitionOff()//关闭语音识别函数  
        {
            if (Recognizer != null)
            {
                Recognizer.RecognizeAsyncStop();
            }
            else
            {
                MessageBox.Show("关闭语音识别失败");
            }
        }

        private void TurnDictationOn()
        {
            if (Recognizer != null)
            {
                Recognizer.LoadGrammar(dictationGrammar);
                //加载自然语法  
            }
            else
            {
                MessageBox.Show("创建语音识别失败");
            }
        }
        private void TurnDictationOff()
        {
            if (dictationGrammar != null)
            {
                Recognizer.UnloadGrammar(dictationGrammar);
                //卸载自然语法  
            }
            else
            {
                MessageBox.Show("关闭语音识别失败");
            }
        }
    }
}
