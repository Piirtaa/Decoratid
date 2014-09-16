using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;

namespace Decoratid.Utils
{
    public static class ACLUtil
    {

        // Adds an ACL entry on the specified file for the specified account.
        public static void AddFileSecurity(string fileName, string account,
            FileSystemRights rights, AccessControlType controlType)
        {
            List<PlatformID> acceptPlatforms = new List<PlatformID>(){ PlatformID.Win32NT , PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.WinCE, PlatformID.Xbox};
            if (!acceptPlatforms.Contains(Environment.OSVersion.Platform))
                throw new InvalidOperationException("Not windows platform");

            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            // Add the FileSystemAccessRule to the security settings.
            fSecurity.AddAccessRule(new FileSystemAccessRule(account,
                rights, controlType));

            // Set the new access settings.
            File.SetAccessControl(fileName, fSecurity);

        }

        // Removes an ACL entry on the specified file for the specified account.
        public static void RemoveFileSecurity(string fileName, string account,
            FileSystemRights rights, AccessControlType controlType)
        {
            List<PlatformID> acceptPlatforms = new List<PlatformID>() { PlatformID.Win32NT, PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.WinCE, PlatformID.Xbox };
            if (!acceptPlatforms.Contains(Environment.OSVersion.Platform))
                throw new InvalidOperationException("Not windows platform");

            // Get a FileSecurity object that represents the
            // current security settings.
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            // Remove the FileSystemAccessRule from the security settings.
            fSecurity.RemoveAccessRule(new FileSystemAccessRule(account,
                rights, controlType));

            // Set the new access settings.
            File.SetAccessControl(fileName, fSecurity);

        }
    }
}
