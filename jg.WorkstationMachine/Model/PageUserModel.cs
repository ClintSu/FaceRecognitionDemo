using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace jg.WorkstationMachine.Model
{
    public class PageUserModel
    {
        public string PageName { get; set; }

        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserGroup { get; set; }
        public string WorkTime { get; set; }
        public string MachineID { get; set; }
        public string MachineMAC{ get; set; }

        public ImageSource ImageSource { get; set; }

    }
}
