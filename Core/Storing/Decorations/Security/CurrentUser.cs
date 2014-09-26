using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.Store.Decorations.Security
{
    /// <summary>
    /// thread local storage user
    /// </summary>
    public class CurrentUser
    {
        public static ThreadLocal<IUserInfoStore> UserStore = new ThreadLocal<IUserInfoStore>(() => { return null; });
    }
}
