using System;
using System.Collections.Generic;
using System.Configuration;
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
            if (entry == null) return wasSuccessful;

            var container = new DirectoryEntry(newContainer);
                        
            entry.MoveTo(container);
            entry.CommitChanges();
            entry.Close();

            wasSuccessful = true;            

            return wasSuccessful;
        }

    }
}