using AdClient.Extensions;
using AdClient.Models;
using AdClient.Requests;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdClient.Services
{
    public interface IUsersService
    {
        bool ValidateCredentials(string userName, string password);
        List<User> GetAllUsers();
        User GetUser(string samAccountName);
        bool IsGroupMember(string samAccountName, string groupName);
        bool MoveUser(string userDistinguishedName, string newContainer);
        bool UpdateUser(string description, bool enabled, string altReceipient, string ipPhone, bool msExchHideFromAddressLists, string samAccountName);
    }

    public class UsersService : IUsersService
    {
        private readonly string _domain = ConfigurationManager.AppSettings["RootDomain"];
        private readonly string _rootOu = ConfigurationManager.AppSettings["RootOu"];
        private readonly string _serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
        private readonly string _servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

        private readonly PrincipalContext _ctx;

        public UsersService()
        {
            try
            {
                _ctx = new PrincipalContext(ContextType.Domain, _domain, _rootOu, ContextOptions.Negotiate, _serviceUser, _servicePassword);

                try
                {
                    //used to catch AuthenticationExceptions
                    var connectedServer = _ctx.ConnectedServer;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<User> GetAllUsers()
        {
            var ps = new PrincipalSearcher(new UserPrincipalEx(_ctx) { ObjectCategory = "Person" });
            return ps.FindAll().ToUserList();
        }

        public User GetUser(string samAccountName)
        {
            var userPrincipal = Get(samAccountName);
            return userPrincipal.ToUser();
        }

        public bool IsGroupMember(string samAccountName, string groupName)
        {
            var userPrincipal = Get(samAccountName);
            return userPrincipal.IsGroupMember(groupName);
        }

        public bool MoveUser(string userDistinguishedName, string newContainer)
        {
            bool result;

            var user = new DirectoryEntry($"LDAP://{_domain}/{userDistinguishedName}");
            result = user.MoveTo($"LDAP://{_domain}/{newContainer}");

            return result;
        }

        public bool UpdateUser(string description, bool enabled, string altReceipient, string ipPhone, bool msExchHideFromAddressLists, string samAccountName)
        {
            var userPrincipal = Get(samAccountName);

            return userPrincipal.Update(new UpdateUserRequest
            {
                AltRecipient = altReceipient,
                Description = description,
                Enabled = enabled,
                IpPhone = ipPhone,
                MsExchHideFromAddressLists = msExchHideFromAddressLists,
                SamAccountName = samAccountName
            });
        }

        public bool ValidateCredentials(string userName, string password)
        {
            var isValid = _ctx.ValidateCredentials(userName, password, ContextOptions.Negotiate);
            return isValid;
        }

        private UserPrincipalEx Get(string samAccountName)
        {
            var userPrincipal = UserPrincipalEx.FindByIdentity(_ctx, IdentityType.SamAccountName, samAccountName);
            return userPrincipal;
        }
    }
}
