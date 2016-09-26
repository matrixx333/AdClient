using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdClient.Requests
{
    public class MoveUserRequest
    {
        public string UserDistinguishedName { get; set; }
        public string NewContainer { get; set; }
    }
}
