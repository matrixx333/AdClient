using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using AdClient.Web.Models;
using AdClient.Web.Extensions;
using AdClient.Models;
using AdClient.Requests;
using System.Configuration;
using System.Web.Http;

namespace AdClient.Web.Controllers
{
    [RoutePrefix("api/v1/users")]
    public class UsersController : ApiController
    {
        private readonly string _domain = ConfigurationManager.AppSettings["RootDomain"];
        private readonly string _rootOu = ConfigurationManager.AppSettings["RootOu"];
        private readonly string _serviceUser = ConfigurationManager.AppSettings["ServiceUser"];
        private readonly string _servicePassword = ConfigurationManager.AppSettings["ServicePassword"];

        private readonly PrincipalContext _ctx;

        public UsersController()
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

        [Route("validate")]
        public IHttpActionResult PostValidateCredentials(Credentials creds)
        {
            var isValid = _ctx.ValidateCredentials(creds.Username, creds.Password, ContextOptions.Negotiate);
            return Ok(isValid);
        }

        [Route("")]
        public IHttpActionResult GetAllUsers()
        {
            var ps = new PrincipalSearcher(new UserPrincipalEx(_ctx) { ObjectCategory = "Person" });
            return Ok(ps.FindAll().ToUserList());
        }

        /// <summary>
        /// Gets the specified user from Active Directory.
        /// </summary>
        /// <param name="samAccountName">The user name in Active Directory that you want to have returned.</param>
        /// <returns>Returns the User principal from Active Directory, otherwise, returns 'null'</returns>
        [Route("{samAccountName}")]
        public IHttpActionResult GetUser(string samAccountName)
        {
            var userPrincipal = Get(samAccountName);
            return Ok(userPrincipal.ToUser());
        }

        [Route("{samAccountName}/is-member")]
        public IHttpActionResult PostIsGroupMember(string samAccountName, IsGroupMemberRequest request)
        {
            var userPrincipal = Get(samAccountName);  
                      
            return Ok(userPrincipal.IsGroupMember(request.GroupName));
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
            bool result;

            var user = new DirectoryEntry($"LDAP://{_domain}/{request.UserDistinguishedName}");
            result = user.MoveTo($"LDAP://{_domain}/{request.NewContainer}");

            return Ok(result);
        }

        /// <summary>
        /// Updates the description field for a User principal in Active Directory.
        /// </summary>
        /// <param name="userPrincipal">The User principal in Active Directory that you want to update the description field for.</param>
        /// <returns>Returns 'true' if the descripion is saved successfully, otherwise, returns 'false'.</returns>
        [Route("{samAccountName}")]
        public IHttpActionResult PutUpdate(UpdateUserRequest request)
        {
            var userPrincipal = Get(request.SamAccountName);
            return Ok(userPrincipal.Update(request));
        }
        
        private UserPrincipalEx Get(string samAccountName)
        {
            var userPrincipal = UserPrincipalEx.FindByIdentity(_ctx, IdentityType.SamAccountName, samAccountName);
            return userPrincipal;
        }
    }
}
