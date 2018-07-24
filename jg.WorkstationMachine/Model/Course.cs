using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jg.WorkstationMachine.Model
{
    public class Course
    {
        public string courseName { get; set; }
        public List<CourseItem> courseItems { get; set; }
    }

    public class CourseItem
    {
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string description { get; set; }
        public string playName { get; set; }

        public string unityName { get; set; }
    }
}
