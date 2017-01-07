using AdClient.Models;
using AdClient.Models.Extensions;
using AdClient.Models.Requests;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

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
        bool ToDomainGuest(string samAccountName);
    }

    public class UsersService : IUsersService
    {
        private readonly PrincipalContext _ctx;
        private string _rootDomain;

        public UsersService(string rootDomain, string rootOu, string serviceUser, string servicePassword)
        {
            try
            {
                _rootDomain = rootDomain;
                _ctx = new PrincipalContext(ContextType.Domain, rootDomain, rootOu, ContextOptions.Negotiate, serviceUser, servicePassword);

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

            var user = new DirectoryEntry($"LDAP://{_rootDomain}/{userDistinguishedName}");
            result = user.MoveTo($"LDAP://{_rootDomain}/{newContainer}");

            return result;
        }

        public bool ToDomainGuest(string samAccountName)
        {
            var userPrincipal = Get(samAccountName);
            return userPrincipal.ToDomainGuests();
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
