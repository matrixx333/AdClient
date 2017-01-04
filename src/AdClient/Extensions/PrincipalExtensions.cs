using AdClient.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace AdClient.Extensions
{
    public static class PrincipalExtensions
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
            return principals.Cast<GroupPrincipal>().Select(g => g.ToGroup()).ToList();
        }

        public static IEnumerable<GroupPrincipal> ToGroupPrincipalList(this IEnumerable<Principal> principals)
        {
            return principals.Cast<GroupPrincipal>().ToList();
        }

        public static List<User> ToUserList(this IEnumerable<Principal> principals)
        {
            return principals.Cast<UserPrincipalEx>().Select(u => u.ToUser()).ToList();
        }
    }
}