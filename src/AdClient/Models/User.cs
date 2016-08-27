using System;
using System.Collections.Generic;

namespace AdClient.Models
{
    public class User
    {
        public Guid? Guid { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string SamAccountName { get; set; }
        public string Description { get; set; }
        public string EmailAddress { get; set; }
        public int BadLogonCount { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? DisabledDate { get; set; }
        public string DisabledBy { get; set; }
        public DateTime? LastBadPasswordAttempt { get; set; }
        public DateTime? LastLogon { get; set; }
        public DateTime? LastPasswordSet { get; set; }
        public bool IsAccountLockedOut { get; set; }
        public string AltRecipient { get; set; }
        public bool MsExchHideFromAddressLists { get; set; }
        public string IpPhone { get; set; }
        public int PrimaryGroupId { get; set; }
        public string DistinguishedName { get; set; }

        public List<Group> Groups { get; set; }
    }
}
