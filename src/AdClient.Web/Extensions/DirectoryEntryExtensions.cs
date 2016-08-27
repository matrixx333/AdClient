using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;

namespace AdClient.Web.Extensions
{
    public static class DirectoryEntryExtensions
    {
        public static bool MoveTo(this DirectoryEntry entry, string newContainer)
        {
            var wasSuccessful = false;
            if (newContainer == null) return wasSuccessful;

            if (entry != null)
            {
                entry.MoveTo(new DirectoryEntry(newContainer));
                entry.CommitChanges();
                entry.Close();
                wasSuccessful = true;
            }

            return wasSuccessful;
        }

    }
}