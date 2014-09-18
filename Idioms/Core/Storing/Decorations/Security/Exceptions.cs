using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Store;
using Sandbox.Extensions;
using CuttingEdge.Conditions;

namespace Sandbox.Store.Decorations.Security
{
    [Serializable]
    public class StoreSecurityException : ApplicationException
    {
        #region Declarations
        private IHasId _item = null;
        private IUserInfoStore _user = null;
        private StoredItemAccessMode _ruleType = StoredItemAccessMode.Read;
        #endregion

        #region Ctor
        public StoreSecurityException(IHasId item, IUserInfoStore user, StoredItemAccessMode ruleType)
            : base(string.Format("{2} Access denied on {0} for {1}", item, user, ruleType))
        {
            Condition.Requires(item).IsNotNull();
            Condition.Requires(user).IsNotNull();
            this._item = item;
            this._user = user;
            this._ruleType = ruleType;
        }
        protected internal StoreSecurityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            info.AddValue("_item", this._item);
            info.AddValue("_ruleType", this._ruleType);
            info.AddValue("_user", this._user);
        }
        #endregion

        #region Overrides
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // call base
            base.GetObjectData(info, context);

            //pull out added info
            this._item = info.GetValue("_item", typeof(object)) as IHasId;
            this._user = info.GetValue("_user", typeof( object)) as IUserInfoStore;
            this._ruleType = (StoredItemAccessMode)info.GetValue("_ruleType", typeof(StoredItemAccessMode));
        }
        #endregion

        #region Properties
        public IHasId Item { get { return _item; } }
        public IUserInfoStore User { get { return _user; } }
        public StoredItemAccessMode RuleType { get { return _ruleType; } }
        #endregion
    }
}
