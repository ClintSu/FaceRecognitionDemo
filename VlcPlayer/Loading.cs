using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VlcPlayer
{
    using System.ComponentModel;
    public class LoadingWait : Control
    {
        static LoadingWait()
        {
            #region 样式说明

            //此控件的内容和样式都是在文件Generic.xaml进行定义的，即可视为UI设计
            // 经验证，必须有,RelativeSource={RelativeSource TemplatedParent} 否则绑定失效
            // <SolidColorBrush x:Key = "ParticleColor" Color = "{Binding Path=FillColor,RelativeSource={RelativeSource TemplatedParent}}" />

            #endregion 样式说明

            //重载默认样式
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingWait), new FrameworkPropertyMetadata(typeof(LoadingWait)));
            //DependencyProperty 注册  FillColor
            FillColorProperty = DependencyProperty.Register("FillColor",
                typeof(Color),
                typeof(LoadingWait),
                new UIPropertyMetadata(Colors.DarkBlue,
                new PropertyChangedCallback(OnUriChanged))
                );
            //Colors.DarkBlue为控件初始化默认值
        }

        //VS设计器属性支持
        [Description("背景色"), Category("个性配置"), DefaultValue("#FFBBBBBB")]
        public Color FillColor
        {
            //GetValue,SetValue为固定写法，此处一般不建议处理其他逻辑
            get { return (Color)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        //属性变更回调函数
        private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Border b = (Border)d;
            //MessageBox.Show(e.NewValue.ToString());
        }

        #region 自定义Fields

        // DependencyProperty属性定义   FillColorProperty=FillColor+Property组成
        public static readonly DependencyProperty FillColorProperty;

        #endregion 自定义Fields
    }
}