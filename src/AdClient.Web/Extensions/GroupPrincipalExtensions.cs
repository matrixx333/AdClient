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
    public static class GroupPrincipalExtension
    {
        public static bool AddUser(this GroupPrincipal groupPrincipal, UserPrincipalEx userPrincipal)
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

        public static bool RemoveUser(this GroupPrincipal groupPrincipal, UserPrincipalEx userPrincipal)
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