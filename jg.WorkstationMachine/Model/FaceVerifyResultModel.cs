using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jg.WorkstationMachine.Model
{
    public class FaceVerifyResultModel
    {
        public float face_liveness { get; set; }
        public object thresholds { get; set; }
        public List<FaceModel> face_list { get; set; }
    }
}
