using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdClient.Models.Requests
{
    public class ChangeGroupRequest
    {
        public string SamAccountName { get; set; }
        public string GroupName { get; set; }
    }
}
