using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Conditions;

namespace Sandbox.Store.Decorations.Security.AccessConditions
{
    /// <summary>
    /// defines access to a single item for a single user
    /// </summary>
    public class PerUserAccessCondition : IConditionOf<StoredItemSecurityContext>
    {
        #region Ctor
        public PerUserAccessCondition(StoredObjectId item, IUserInfoStore user)
        {
            Condition.Requires(item).IsNotNull();
            Condition.Requires(user).IsNotNull();

            this.Item = item;
            this.User = user;
        }
        #endregion

        #region Properties
        public StoredObjectId Item { get; protected set; }
        public IUserInfoStore User { get; protected set; }
        #endregion


        #region ICondition
        /// <summary>
        /// do the user and item's match exactly? else return null
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool? Evaluate(StoredItemSecurityContext context)
        {
            if (context == null)
                return null;

            if(context.Item.ObjectId.Equals(this.Item.ObjectId) && 
                context.Item.ObjectType.Equals(this.Item.ObjectType) &&
                context.User.is
        }
        #endregion
    }
}
