using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jg.WorkstationMachine.Model
{
    public class FaceModel
    {
        public string face_token { get; set; }
        public FaceLocation location { get; set; }
        public double face_probability { get; set; }
        public FaceAngle angle { get; set; }
        public double age { get; set; }
        public double beauty { get; set; }
        public FaceExpression expression { get; set; }
        public FaceShape face_shape { get; set; }
        public FaceGender gender { get; set; }
        public FaceGlasses glasses { get; set; }
        public FaceRace race { get; set; }
        public FaceFacetype face_type { get; set; }
        public List<FaceLandmark> landmark { get; set; }
        public List<FaceLandmark> landmark72 { get; set; }
        public FaceQuality quality { get; set; }
    }

    /// <summary>
    /// 人脸在图片中的位置
    /// </summary>
    public class FaceLocation
    {
        /// <summary>
        /// 人脸区域离左边界的距离
        /// </summary>
        public double left { get; set; }
        /// <summary>
        /// 人脸区域离上边界的距离
        /// </summary>
        public double top { get; set; }
        /// <summary>
        /// 人脸区域的宽度
        /// </summary>
        public double width { get; set; }
        /// <summary>
        /// 人脸区域的高度
        /// </summary>
        public double height { get; set; }
        /// <summary>
        /// 人脸框相对于竖直方向的顺时针旋转角，[-180,180]
        /// </summary>
        public long rotation { get; set; }
    }
    /// <summary>
    /// 人脸旋转角度参数
    /// </summary>
    public class FaceAngle
    {
        /// <summary>
        /// 三维旋转之左右旋转角[-90(左), 90(右)]
        /// </summary>
        public double yaw { get; set; }
        /// <summary>
        /// 三维旋转之俯仰角度[-90(上), 90(下)]
        /// </summary>
        public double pitch { get; set; }
        /// <summary>
        /// 平面内旋转角[-180(逆时针), 180(顺时针)]
        /// </summary>
        public double roll { get; set; }
    }
    /// <summary>
    /// 表情，当 face_field包含expression时返回
    /// </summary>
    public class FaceExpression
    {
        /// <summary>
        /// none:不笑；smile:微笑；laugh:大笑
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 表情置信度，范围【0~1】，0最小、1最大。
        /// </summary>
        public double probability { get; set; }
    }
    /// <summary>
    /// 脸型，当face_field包含faceshape时返回
    /// </summary>
    public class FaceShape
    {
        /// <summary>
        /// square: 正方形 triangle:三角形 oval: 椭圆 heart: 心形 round: 圆形
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 置信度，范围【0~1】，代表这是人脸形状判断正确的概率，0最小、1最大。
        /// </summary>
        public double probability { get; set; }
    }
    /// <summary>
    /// 性别，face_field包含gender时返回
    /// </summary>
    public class FaceGender
    {
        /// <summary>
        /// male:男性 female:女性
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 性别置信度，范围【0~1】，0代表概率最小、1代表最大。
        /// </summary>
        public double probability { get; set; }
    }
    /// <summary>
    /// 是否带眼镜，face_field包含glasses时返回
    /// </summary>
    public class FaceGlasses
    {
        /// <summary>
        /// none:无眼镜，common:普通眼镜，sun:墨镜
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 眼镜置信度，范围【0~1】，0代表概率最小、1代表最大。
        /// </summary>
        public double probability { get; set; }
    }
    /// <summary>
    /// 人种 face_field包含race时返回
    /// </summary>
    public class FaceRace
    {
        /// <summary>
        /// yellow: 黄种人 white: 白种人 black:黑种人 arabs: 阿拉伯人
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 人种置信度，范围【0~1】，0代表概率最小、1代表最大。
        /// </summary>
        public double probability { get; set; }
    }
    /// <summary>
    /// 真实人脸/卡通人脸 face_field包含facetype时返回
    /// </summary>
    public class FaceFacetype
    {
        /// <summary>
        /// human: 真实人脸 cartoon: 卡通人脸
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 人脸类型判断正确的置信度，范围【0~1】，0代表概率最小、1代表最大。
        /// </summary>
        public double probability { get; set; }
    }
    /// <summary>
    /// 人脸质量信息。face_field包含quality时返回
    /// </summary>
    public class FaceQuality
    {
        /// <summary>
        ///	人脸各部分遮挡的概率，范围[0~1]，0表示完整，1表示不完整
        /// </summary>
        public FaceOcclusion occlusion { get; set; }
        /// <summary>
        /// 人脸模糊程度，范围[0~1]，0表示清晰，1表示模糊
        /// </summary>
        public double blur { get; set; }
        //取值范围在[0~255], 表示脸部区域的光照程度 越大表示光照越好
        public double illumination { get; set; }
        /// <summary>
        /// 人脸完整度，0或1, 0为人脸溢出图像边界，1为人脸都在图像边界内
        /// </summary>
        public long completeness { get; set; }
    }
    /// <summary>
    /// 人脸各部分遮挡的概率，范围[0~1]，0表示完整，1表示不完整
    /// </summary>
    public class FaceOcclusion
    {
        /// <summary>
        /// 左眼遮挡比例，[0-1] ，1表示完全遮挡
        /// </summary>
        public double left_eye { get; set; }
        /// <summary>
        /// 右眼遮挡比例，[0-1] ， 1表示完全遮挡
        /// </summary>
        public double right_eye { get; set; }
        /// <summary>
        /// 鼻子遮挡比例，[0-1] ， 1表示完全遮挡
        /// </summary>
        public double nose { get; set; }
        /// <summary>
        /// 嘴巴遮挡比例，[0-1] ， 1表示完全遮挡
        /// </summary>
        public long mouth { get; set; }
        /// <summary>
        /// 左脸颊遮挡比例，[0-1] ， 1表示完全遮挡
        /// </summary>
        public double left_cheek { get; set; }
        /// <summary>
        /// 右脸颊遮挡比例，[0-1] ， 1表示完全遮挡
        /// </summary>
        public double right_cheek { get; set; }
        /// <summary>
        /// 下巴遮挡比例，，[0-1] ， 1表示完全遮挡
        /// </summary>
        public long chin { get; set; }
    }
    /// <summary>
    /// 脸部特征
    /// </summary>
    public class FaceLandmark
    {
        public double x { get; set; }
        public double y { get; set; }
    }
}
