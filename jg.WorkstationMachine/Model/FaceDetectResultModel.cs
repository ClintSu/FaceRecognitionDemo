using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jg.WorkstationMachine.Model
{
    public class FaceDetectResultModel
    {
        public int face_num { get; set; }
        public List<FaceModel> face_list { get; set; }
    }
}
