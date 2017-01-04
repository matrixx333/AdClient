using AdClient.Extensions;
using AdClient.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdClient.Services
{
    public interface IGroupsService
    {
        List<Group> GetAllGroups();
        Group GetGroup(string groupName);
        List<Group> GetUserGroups(string samAccountName);
        bool AddUserToGroup(string samAccountName, string groupName);
        bool RemoveUserFromGroup(string samAccountName, string groupName);
        bool RemoveAllUserGroupMemberships(string samAccountName);
    }

    public class GroupsService : IGroupsService
    {
        private readonly string _domain = ConfigurationManager.AppSettings["RootDomain"];
        private readonly string _rootOu = ConfigurationManager.AppSettings["RootOu"];
        private readonly string _serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
        private readonly string _servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

        private readonly PrincipalContext _ctx;

        public GroupsService()
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

        public bool AddUserToGroup(string samAccountName, string groupName)
        {
            bool wasSuccessful = false;

            var userPrincipal = GetUser(samAccountName);
            var groupPrincipal = Get(groupName);
            groupPrincipal.AddUser(userPrincipal);

            wasSuccessful = true;

            return wasSuccessful;
        }

        public List<Group> GetAllGroups()
        {
            var ps = new PrincipalSearcher(new GroupPrincipalEx(_ctx) { ObjectCategory = "Group" });
            var allGroups = ps.FindAll().ToGroupList();

            return allGroups;
        }

        public Group GetGroup(string groupName)
        {
            var gp = Get(groupName);
            return gp.ToGroup();
        }

        public List<Group> GetUserGroups(string samAccountName)
        {
            var up = GetUser(samAccountName);
            var groups = new List<Group>();

            if (up != null)
            {
                groups = up.GetGroups().ToGroupList();
            }

            return groups;
        }

        public bool RemoveAllUserGroupMemberships(string samAccountName)
        {
            var wasSuccessful = false;
            const string domainAdmins = "Domain Admins";
            const string domainGuests = "Domain Guests";
            var domainAdminsGroup = Get(domainAdmins);
            var domainGuestsGroup = Get(domainGuests);
            var up = GetUser(samAccountName);

            // If the user is already a Domain Guest, return
            if (up.PrimaryGroupId == (int)PrimaryGroupId.DomainGuests)
                return wasSuccessful;

            up.ToDomainGuests();

            var userGroups = up.GetGroups()
                .Where(g => g.Name != domainGuests)
                .Where(g => g.Name != domainAdmins)
                .ToGroupPrincipalList();

            foreach (var group in userGroups)
            {
                group.RemoveUser(up);
            }

            if (!up.IsGroupMember(domainGuests))
            {
                domainGuestsGroup.AddUser(up);
            }

            if (up.IsGroupMember(domainAdmins))
            {
                domainAdminsGroup.RemoveUser(up);
            }

            wasSuccessful = true;

            return wasSuccessful;
        }

        public bool RemoveUserFromGroup(string samAccountName, string groupName)
        {
            bool wasSuccessful = false;
            var userPrincipal = GetUser(samAccountName);
            var groupPrincipal = Get(groupName);
            groupPrincipal.RemoveUser(userPrincipal);

            wasSuccessful = true;

            return wasSuccessful;
        }

        private GroupPrincipalEx Get(string groupName)
        {
            var groupPrincipal = GroupPrincipalEx.FindByIdentity(_ctx, groupName);
            return groupPrincipal;
        }

        private UserPrincipalEx GetUser(string samAccountName)
        {
            var userPrincipal = UserPrincipalEx.FindByIdentity(_ctx, IdentityType.SamAccountName, samAccountName);
            return userPrincipal;
        }
    }
}
