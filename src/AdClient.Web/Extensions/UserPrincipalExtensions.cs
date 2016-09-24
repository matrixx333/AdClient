using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdClient.Models;
using AdClient.Web.Models;
using System.DirectoryServices.AccountManagement;
using AdClient.Requests;
using AdClient.Web.Extensions;
using System.DirectoryServices;

namespace AdClient.Web.Extensions
{
    public static class UserPrincipalExExtension
    {
        public static User ToUser(this UserPrincipalEx p)
        {
            if (p == null)
                return null;

            var user = new User
            {
                AltRecipient = p.AltRecipient,
                BadLogonCount = p.BadLogonCount,
                Description = p.Description,
                DisabledBy = null,
                DisabledDate = null,
                DisplayName = p.DisplayName,
                DistinguishedName = p.DistinguishedName,
                Enabled = p.Enabled,
                EmailAddress = p.EmailAddress,
                Groups = null,
                Guid = p.Guid,
                IpPhone = p.IpPhone,
                IsAccountLockedOut = p.IsAccountLockedOut(),
                LastBadPasswordAttempt = p.LastBadPasswordAttempt,
                LastLogon = p.LastLogon,
                LastPasswordSet = p.LastPasswordSet,
                MsExchHideFromAddressLists = p.MsExchHideFromAddressLists,
                Name = p.Name,
                SamAccountName = p.SamAccountName,
                PrimaryGroupId = p.PrimaryGroupId
            };

            return user;
        }

        public static List<User> ToUserList(this IEnumerable<Principal> principals)
        {
            return principals.Cast<UserPrincipalEx>().Select(u => u.ToUser()).ToList();
        }

        /// <summary>
        /// Sets the primary group for the specified User principal to "Domain Guests".
        /// </summary>
        /// <param name="userPrincipal">The principal of the account in Active Directory that you'd like to set the primary group for.</param>    
        public static bool ToDomainGuests(this UserPrincipalEx userPrincipal)
        {
            bool wasSuccessful = false;
            if (userPrincipal != null)
            {
                userPrincipal.PrimaryGroupId = (int)PrimaryGroupId.DomainGuests;
                userPrincipal.Save();
                wasSuccessful = true;
            }

            return wasSuccessful;
        }

        public static bool IsGroupMember(this UserPrincipalEx userPrincipal, string groupName)
        {
            return userPrincipal.GetGroups().Any(g => g.Name.ToLower() == groupName.ToLower());
        }

        public static bool MoveUser(this UserPrincipalEx userPrincipal, string newContainer)
        {
            var wasSuccessful = false;
            if (newContainer == null) return wasSuccessful;

            if (userPrincipal != null)
            {
                var termUser = new DirectoryEntry(userPrincipal.DistinguishedName);

                termUser.MoveTo(new DirectoryEntry(newContainer));
                termUser.CommitChanges();
                termUser.Close();
                wasSuccessful = true;
            }

            return wasSuccessful;
        }

        public static bool Update(this UserPrincipalEx userPrincipal, UpdateUserRequest request)
        {
            var wasSuccessful = false;
            if (request == null) return wasSuccessful;

            if (userPrincipal != null)
            {
                userPrincipal.AltRecipient = null;
                userPrincipal.Description = request.Description;
                userPrincipal.Enabled = request.Enabled;
                userPrincipal.IpPhone = request.IpPhone;
                userPrincipal.MsExchHideFromAddressLists = request.MsExchHideFromAddressLists;
                userPrincipal.Save();
                wasSuccessful = true;
            }
            return wasSuccessful;
        }
    }
}
