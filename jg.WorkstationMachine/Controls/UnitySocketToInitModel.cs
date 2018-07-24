namespace jg.WorkstationMachine.Controls
{
    /// <summary>
    /// 教具初始化消息
    /// </summary>
    public class UnitySocketToInitModel
    {
        public string ExamTime { get; set; }
        public string Mode { get; set; }//"Practice",
        public string TaskID { get; set; }
        public string UserID { get; set; }//"12306",
        public string UserName { get; set; }//"某某",
        //"Mission_02",
        //"100"
    }
}