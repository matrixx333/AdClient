using System;
using System.Collections.Generic;
using System.Linq;
using AdClient.Models;
using System.Configuration;
using System.Web.Http;
using AdClient.Services;
using AdClient.Models.Requests;

namespace AdClient.Web.Controllers
{
    [RoutePrefix("api/v1/users")]
    public class UsersController : ApiController
    {
        private readonly string _rootDomain = ConfigurationManager.AppSettings["RootDomain"];
        private readonly string _rootOu = ConfigurationManager.AppSettings["RootOu"];
        private readonly string _serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
        private readonly string _servicePassword = ConfigurationManager.AppSettings["ServicePassword"];
        private readonly IUsersService _userSvc;

        public UsersController()
        {
            _userSvc = new UsersService(_rootDomain, _rootOu, _serviceUser, _servicePassword);
        }

        [Route("validate")]
        public IHttpActionResult PostValidateCredentials(Credentials creds)
        {
            return Ok(_userSvc.ValidateCredentials(creds.Username, creds.Password));
        }

        [Route("")]
        public IHttpActionResult GetAllUsers()
        {
            return Ok(_userSvc.GetAllUsers());
        }

        /// <summary>
        /// Gets the specified user from Active Directory.
        /// </summary>
        /// <param name="samAccountName">The user name in Active Directory that you want to have returned.</param>
        /// <returns>Returns the User principal from Active Directory, otherwise, returns 'null'</returns>
        [Route("{samAccountName}")]
        public IHttpActionResult GetUser(string samAccountName)
        {
            return Ok(_userSvc.GetUser(samAccountName));
        }

        [Route("{samAccountName}/is-member")]
        public IHttpActionResult PostIsGroupMember(string samAccountName, IsGroupMemberRequest request)
        {
            return Ok(_userSvc.IsGroupMember(samAccountName, request.GroupName));
        }

        /// <summary>
        /// Moves the user specified in the 'userDistinquishedName' parameter to the new container specified in the 'newContainer' parameter.
        /// NOTE: The user signed onto the computer running the program must be delegated rights to Active Directory or the MoveUser() method will not work.
        /// </summary>
        /// <param name="userDistinquishedName">LDAP distinquished name attribute of the user\container to be moved. Example ("LDAP://fabrikam.com/CN=User Name,OU=Sales,DC=fabrikam,DC=com").</param>
        /// <param name="newContainer">LDAP distinquished name attribute of the new container. Example ("LDAP://fabrikam.com/OU=HR,DC=fabrikam,DC=com").</param>
        /// <returns></returns>
        [Route("move")]
        public IHttpActionResult PostMoveUser(MoveUserRequest request)
        {
            return Ok(_userSvc.MoveUser(request.UserDistinguishedName, request.NewContainer));
        }

        /// <summary>
        /// Updates the description field for a User principal in Active Directory.
        /// </summary>
        /// <param name="userPrincipal">The User principal in Active Directory that you want to update the description field for.</param>
        /// <returns>Returns 'true' if the descripion is saved successfully, otherwise, returns 'false'.</returns>
        [Route("{samAccountName}")]
        public IHttpActionResult PutUpdate(UpdateUserRequest request)
        {
            return Ok(_userSvc.UpdateUser(request.Description, request.Enabled, request.AltRecipient, request.IpPhone, request.MsExchHideFromAddressLists, request.SamAccountName));
        }        
    }
}
