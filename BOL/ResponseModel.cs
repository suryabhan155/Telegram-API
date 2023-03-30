using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL
{
    public class ResponseModel
    {
        public bool success { get; set; }

        public string message { get; set; }

        public Object data { get; set; }

        public int Status_Code { get; set; }
    }
}
