using System.Web.Http;
using AdClient.Services;
using AdClient.Models.Requests;

namespace AdClient.Web.Controllers
{
    [RoutePrefix("api/v1/groups")]
    public class GroupsController : ApiController
    {
        private readonly IGroupsService _groupSvc;

        public GroupsController()
        {
            _groupSvc = new GroupsService();
        }

        [Route("")]
        public IHttpActionResult GetAllGroups()
        {
            return Ok(_groupSvc.GetAllGroups());
        }
        
        /// <summary>
        /// Gets the specified group from Active Directory.
        /// </summary>
        /// <param name="groupName">The group name in Active Directory that you want to have returned.</param>
        /// <returns>Returns the Group principal from Active Directory, otherwise, returns 'null'.</returns>
        [Route("{groupName}")]
        public IHttpActionResult GetGroup(string groupName)
        {
            return Ok(_groupSvc.GetGroup(groupName));
        }

        [Route("~/api/v1/users/{samAccountName}/groups")]
        public IHttpActionResult GetUserGroups(string samAccountName)
        {
            return Ok(_groupSvc.GetUserGroups(samAccountName));
        }

        /// <summary>
        /// Adds the User principal to the specified group in Active Directory. 
        /// </summary>
        /// <param name="userPrincipal">The User principal in Active Directory.</param>
        /// <param name="groupPrincipal">The group in Active Directory that you'd like to add the UserPrincipal to.</param>
        [Route("add-to-group")]
        public IHttpActionResult PostAddUserToGroup(ChangeGroupRequest request)
        {
            return Ok(_groupSvc.AddUserToGroup(request.SamAccountName, request.GroupName));
        }

        /// <summary>
        /// Removes the User principal from the specified group in Active Directory. 
        /// </summary>
        /// <param name="userPrincipal">The User principal in Active Directory.</param>
        /// <param name="groupPrincipal">The group in Active Directory that you'd like to remove the User principal from.</param>
        [Route("remove-from-group")] 
        public IHttpActionResult PostRemoveUserFromGroup(ChangeGroupRequest request)
        {
            return Ok(_groupSvc.RemoveUserFromGroup(request.SamAccountName, request.GroupName));
        }

        ///// <summary>
        ///// Removes all group memberships for a specified User principal and sets their primary group to "Domain Guests".
        ///// </summary>
        ///// <param name="samAccountName">The user in Active Directory that you want to remove all group memberships for.</param>
        ///// <returns>Returns 'true' if all groups are removed from the user successfully, otherwise, returns 'false'.</returns>
        [Route("~/api/v1/users/{samAccountName}/remove-all-groups")]
        public IHttpActionResult PutRemoveAllUserGroupMemberships(string samAccountName)
        {
            return Ok(_groupSvc.RemoveAllUserGroupMemberships(samAccountName));
        }               
    }
}
