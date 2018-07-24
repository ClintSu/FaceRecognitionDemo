using jg.WorkstationMachine.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace jg.WorkstationMachine
{
    class OrderDocumentRenderer : IDocumentRenderer
    {
        public void Render(FlowDocument doc, object data)
        {

            Model.PageUserModel pageUser = data as Model.PageUserModel;

            Image image =  doc.FindName("HeaderImage") as Image;
            image.Source = Globals.BitSource;

            Run run1 = doc.FindName("RunUserID") as Run;
            run1.Text = "作业人员编号：" + Globals.UserID;
            Run run2 = doc.FindName("RunUserName") as Run;
            run2.Text = "作业人员姓名：" + Globals.UserName;
            Run run3 = doc.FindName("RunUserGroup") as Run;
            run3.Text = "作业人员分组：" + Globals.FaceGroupId;
            Run run4 = doc.FindName("RunWorkTime") as Run;
            run4.Text = "作业开始时间：" + Globals.UseTime;
            Run run5 = doc.FindName("RunMachineID") as Run;
            run5.Text = "工位机编号  ：" + Globals.MachineID;

            Run run7= doc.FindName("PageTitle") as Run;
            run7.Text = pageUser.PageName+ " 学习工作页";
        }
    }
}
