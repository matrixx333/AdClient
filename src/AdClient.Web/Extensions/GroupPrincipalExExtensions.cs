using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdClient.Models;
using System.DirectoryServices.AccountManagement;
using AdClient.Web.Models;
using AdClient.Requests;
using AdClient.Web.Extensions;

namespace AdClient.Web.Extensions
{
    public static class GroupPrincipalExExtension
    {
        public static Group ToGroup(this Principal p)
        {
            if (p == null)
                return null;

            var group = new Group();

            group.Description = p.Description;
            group.Guid = p.Guid;
            group.Name = p.Name;

            return group;
        }

        public static List<Group> ToGroupList(this IEnumerable<Principal> principals)
        {
            return principals.Cast<GroupPrincipalEx>().Select(g => g.ToGroup()).ToList();
        }

        public static bool AddUser(this GroupPrincipalEx groupPrincipal, UserPrincipalEx userPrincipal)
        {
            bool wasSuccessful = false;

            if (userPrincipal == null) return wasSuccessful;

            if (groupPrincipal != null)
            {
                groupPrincipal.Members.Add(userPrincipal);
                groupPrincipal.Save();
                wasSuccessful = true;
            }

            return wasSuccessful;
        }

        public static bool RemoveUser(this GroupPrincipalEx groupPrincipal, UserPrincipalEx userPrincipal)
        {
            bool wasSuccessful = false;

            if (userPrincipal == null) return wasSuccessful;

            if (groupPrincipal != null)
            {
                groupPrincipal.Members.Remove(userPrincipal);
                groupPrincipal.Save();
                wasSuccessful = true;
            }

            return wasSuccessful;
        }
    }
}