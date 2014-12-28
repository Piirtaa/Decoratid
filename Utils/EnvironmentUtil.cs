using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    public static class EnvironmentUtil
    {
        public static string GetRunningAssemblyGUID()
        {
            string rv = null;
            Assembly asm = Assembly.GetExecutingAssembly();
            var attribs = (asm.GetCustomAttributes(typeof(GuidAttribute), true));
            rv = (attribs[0] as GuidAttribute).Value;
            return rv;
        }
    }
}
