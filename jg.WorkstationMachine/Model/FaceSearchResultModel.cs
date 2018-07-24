using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jg.WorkstationMachine.Model
{
    public class FaceSearchResultModel
    {
        public string face_token { get; set; }
        public List<SearchUser> user_list { get; set; }
    }

    public class SearchUser
    {
        public string group_id { get; set; }
        public string user_id { get; set; }
        public string user_info { get; set; }
        public float score { get; set; }
    }
}
