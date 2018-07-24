using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jg.WorkstationMachine.Model
{
    public class Face: Baidu.Aip.Face.Face
    {
        const string APP_ID = "11439188";
        const string API_KEY = "pZjalMcYBp6lMdSkRwOH1zQy";
        const string SECRET_KEY = "DBMQktcrnQvrybonQtrQE9241E4zHOc9";

        public Face():base(API_KEY, SECRET_KEY)
        {
        }
    }
}
