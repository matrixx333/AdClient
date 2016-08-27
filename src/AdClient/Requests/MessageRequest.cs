using System;
using System.Collections.Generic;
using AdClient.Models;

namespace AdClient.Requests
{
    public class MessageRequest
    {
        public List<Group> UserGroups { get; set; }
        public DateTime TermDate { get; set; }
        public User TermUser { get; set; }
        public User ServiceUser { get; set; }
        public string ServicePassword { get; set; }
        public string IpPhone { get; set; }
        public string AltRecipient { get; set; }
    }
}
