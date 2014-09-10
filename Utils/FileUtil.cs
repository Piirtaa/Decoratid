using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    public static class FileUtil
    {

        public static FileStream GetLockedStream(string filePath)
        {
            //FileSecurity fs = new FileSecurity();
           
            // Create a file using the FileStream class.
            return new FileStream(filePath, FileMode.OpenOrCreate,
                FileAccess.ReadWrite, FileShare.None, 8, FileOptions.None);

            //    FileSystemRights.Modify, FileShare.None, 8, FileOptions.None, fs);
        }
    }
}
