using System.DirectoryServices.AccountManagement;

namespace AdClient.Web.Models
{
    [DirectoryObjectClass("user")]
    [DirectoryRdnPrefix("CN")]
    public class UserPrincipalEx : UserPrincipal
    {
        public UserPrincipalEx(PrincipalContext context) : base(context) { }
        public UserPrincipalEx(PrincipalContext context, string samAccountName, string password, bool enabled)
            : base(context, samAccountName, password, enabled) { }

        [DirectoryProperty("altRecipient")]
        public string AltRecipient
        {
            get
            {
                if (ExtensionGet("altRecipient").Length != 1)
                    return null;

                return (string)ExtensionGet("altRecipient")[0];
            }
            set { ExtensionSet("altRecipient", value); }
        }

        [DirectoryProperty("ipPhone")]
        public string IpPhone
        {
            get
            {
                if (ExtensionGet("ipPhone").Length != 1)
                    return null;

                return (string)ExtensionGet("ipPhone")[0];
            }
            set { ExtensionSet("ipPhone", value); }
        }

        [DirectoryProperty("authOrig")]
        public string AuthOrig
        {
            get
            {
                if (ExtensionGet("authOrig").Length != 1)
                    return null;

                return (string)ExtensionGet("authOrig")[0];
            }
            set { ExtensionSet("authOrig", value); }
        }

        [DirectoryProperty("msExchHideFromAddressLists")]
        public bool MsExchHideFromAddressLists
        {
            get
            {
                if (ExtensionGet("msExchHideFromAddressLists").Length != 1)
                    return false;

                return (bool)ExtensionGet("msExchHideFromAddressLists")[0];

            }
            set { ExtensionSet("msExchHideFromAddressLists", value); }
        }

        [DirectoryProperty("primaryGroupID")]
        public int PrimaryGroupId
        {
            get
            {
                if (ExtensionGet("primaryGroupID").Length != 1)
                    return 0;

                return (int)ExtensionGet("primaryGroupID")[0];
            }
            set { ExtensionSet("primaryGroupID", value); }
        }

        [DirectoryProperty("objectCategory")]
        public string ObjectCategory
        {
            get
            {
                if (ExtensionGet("objectCategory").Length != 1)
                    return null;

                return (string)ExtensionGet("objectCategory")[0];
            }
            set { ExtensionSet("objectCategory", value); }
        }

        public static new UserPrincipalEx FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityValue);
        }

        public static new UserPrincipalEx FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (UserPrincipalEx)FindByIdentityWithType(context, typeof(UserPrincipalEx), identityType, identityValue);
        }
    }
}
