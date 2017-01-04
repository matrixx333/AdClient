using System.DirectoryServices.AccountManagement;

namespace AdClient.Models
{
    [DirectoryObjectClass("group")]
    [DirectoryRdnPrefix("CN")]
    public class GroupPrincipalEx : GroupPrincipal
    {
        public GroupPrincipalEx(PrincipalContext context) : base(context) { }
        public GroupPrincipalEx(PrincipalContext context, string samAccountName) : base(context, samAccountName) { }

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

        public static new GroupPrincipalEx FindByIdentity(PrincipalContext context, string identityValue)
        {
            return (GroupPrincipalEx)FindByIdentityWithType(context, typeof(GroupPrincipalEx), identityValue);
        }

        public static new GroupPrincipal FindByIdentity(PrincipalContext context, IdentityType identityType, string identityValue)
        {
            return (GroupPrincipalEx)FindByIdentityWithType(context, typeof(GroupPrincipalEx), identityType, identityValue);
        }
    }
}
