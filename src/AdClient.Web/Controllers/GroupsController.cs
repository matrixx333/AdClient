using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using AdClient.Models;
using AdClient.Web.Models;
using AdClient.Web.Extensions;
using AdClient.Requests;
using System.Configuration;
using System.Web.Http;

namespace AdClient.Web.Controllers
{
    [RoutePrefix("api/v1/groups")]
    public class GroupsController : ApiController
    {
        private readonly string _domain = ConfigurationManager.AppSettings["RootDomain"];
        private readonly string _rootOu = ConfigurationManager.AppSettings["RootOu"];
        private readonly string _serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
        private readonly string _servicePassword = ConfigurationManager.AppSettings["ServicePassword"];
        
        private readonly PrincipalContext _ctx;

        public GroupsController()
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

        [Route("")]
        public IHttpActionResult GetAllGroups()
        {
            var ps = new PrincipalSearcher(new GroupPrincipalEx(_ctx) { ObjectCategory = "Group" });
            var allGroups = ps.FindAll().ToGroupList();
            
            return Ok(allGroups);
        }


        /// <summary>
        /// Gets the specified group from Active Directory.
        /// </summary>
        /// <param name="groupName">The group name in Active Directory that you want to have returned.</param>
        /// <returns>Returns the Group principal from Active Directory, otherwise, returns 'null'.</returns>
        [Route("{groupName}")]
        public IHttpActionResult GetGroup(string groupName)
        {
            var gp = Get(groupName);            
            return Ok(gp.ToGroup());
        }

        [Route("~/api/v1/users/{samAccountName}/groups")]
        public IHttpActionResult GetUserGroups(string samAccountName)
        {
            var up = GetUser(samAccountName);
            var groups = new List<Group>();

            if (up != null)
            {
                groups = up.GetAuthorizationGroups().ToGroupList();                
            }

            return Ok(groups);
        }

        /// <summary>
        /// Adds the User principal to the specified group in Active Directory. 
        /// </summary>
        /// <param name="userPrincipal">The User principal in Active Directory.</param>
        /// <param name="groupPrincipal">The group in Active Directory that you'd like to add the UserPrincipal to.</param>
        [Route("add-to-group")]
        public IHttpActionResult PostAddUserToGroup(ChangeGroupRequest request)
        {
            var userPrincipal = GetUser(request.SamAccountName);
            var groupPrincipal = Get(request.GroupName);
            groupPrincipal.AddUser(userPrincipal);

            return Ok();
        }

        /// <summary>
        /// Removes the User principal from the specified group in Active Directory. 
        /// </summary>
        /// <param name="userPrincipal">The User principal in Active Directory.</param>
        /// <param name="groupPrincipal">The group in Active Directory that you'd like to remove the User principal from.</param>
        [Route("remove-from-group")] 
        public IHttpActionResult PostRemoveUserFromGroup(ChangeGroupRequest request)
        {
            var userPrincipal = GetUser(request.SamAccountName);
            var groupPrincipal = Get(request.GroupName);
            groupPrincipal.RemoveUser(userPrincipal);

            return Ok();
        }

        ///// <summary>
        ///// Removes all group memberships for a specified User principal and sets their primary group to "Domain Guests".
        ///// </summary>
        ///// <param name="samAccountName">The user in Active Directory that you want to remove all group memberships for.</param>
        ///// <returns>Returns 'true' if all groups are removed from the user successfully, otherwise, returns 'false'.</returns>
        [Route("~/api/v1/users/{samAccountName}/remove-all-groups")]
        public IHttpActionResult PutRemoveAllUserGroupMemberships(string samAccountName)
        {
            var wasSuccessful = false;
            var userPrincipal = GetUser(samAccountName);
            var userGroups = userPrincipal.GetGroups().ToGroupPrincipalList();
            var domainGuestsGroup = Get("Domain Guests");

            domainGuestsGroup.AddUser(userPrincipal);
            userPrincipal.ToDomainGuests();

            foreach (var group in userGroups)
            {
                group.RemoveUser(userPrincipal);
            }

            wasSuccessful = true;

            return Ok(wasSuccessful);
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
