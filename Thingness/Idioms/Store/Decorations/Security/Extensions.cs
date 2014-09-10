using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Store.Decorations.Common;
using Sandbox.Extensions;

namespace Sandbox.Store.Decorations.Security
{

    public static class SecurityExtensions
    {
        /// <summary>
        /// walks the audit trail to find the first entry, returns the actor who made it.  If there is no 
        /// secured audit info avail it will return null.
        /// </summary>
        /// <param name="audited"></param>
        /// <returns></returns>
        public static IUserInfoStore GetOwner(this IHasStoredItemAuditTrail audited)
        {
            var first = audited.AuditPoints.First();
            if (first == null)
                return null;

            if (first is ISecuredStoredItemAuditPoint)
                return ((ISecuredStoredItemAuditPoint)first).Actor;

            return null;
        }
    }
}
