using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronzl.Framework.Contract
{
    public class MessageBase
    {
        public string err_code { get; set; }

        public bool success { get; set; }

        public string msg { get; set; }
    }
}
