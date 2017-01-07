namespace AdClient.Models.Requests
{
    public class UpdateUserRequest
    {
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string AltRecipient { get; set; }
        public string IpPhone { get; set; }
        public bool MsExchHideFromAddressLists { get; set; }
        public string SamAccountName { get; set; }
    }
}
