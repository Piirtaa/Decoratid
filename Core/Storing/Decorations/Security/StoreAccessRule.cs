using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Conditions;
using Sandbox.Thingness.Dependencies;

namespace Sandbox.Store.Decorations.Security
{
    //use condition instead of func - base condition (one that has IHasId and IUserInfoStore as properties - to be populated on condition test)
    //        public Func<IHasId, IUserInfoStore, bool?> Strategy { get; protected set; }
    //

    public class StoredItemSecurityContext
    {
        #region Ctor
        public StoredItemSecurityContext(StoredObjectId item , IUserInfoStore user)
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
    }


    /// <summary>
    /// defines a rule that can be persisted, can be ordered relative to other orders via a dependency, 
    /// to determine whether a User has access (ie. rule type) to a StoredItem 
    /// </summary>
    public interface IStoreAccessRule : IHasId<string>, IHasDependencyOf<StoreAccessRule>
    {
        /// <summary>
        /// the condition that, if true, gives the user access to the store.  
        /// </summary>
        /// <remarks>
        /// return null if access is undecided, false if decided to not exist
        /// </remarks>
        IConditionOf<StoredItemSecurityContext> HasAccessCondition {get;}
        StoredItemAccessMode RuleType { get; protected set; }

        //TODO: consolidate RuleType into the context
        //what about whether the rule is a general rule or item specific
    }

    /// <summary>
    /// This is a rule.  It is persisted in a rule store.  It defines whether a User (via IUserContextStore proxy) has access to a 
    /// resource or resources via a strategy. It also has a dependency on other rules.
    /// </summary>
    public class StoreAccessRule : IStoreAccessRule
    {
        #region Ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">the id of the rule</param>
        /// <param name="ruleType">the action the rule checks access for</param>
        /// <param name="strategy">the strategy that determines whether access is granted. returns true if granted, false if not-granted, null if undecided</param>
        public StoreAccessRule(string id, StoredItemAccessMode ruleType, IConditionOf<StoredItemSecurityContext> condition)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            Condition.Requires(strategy).IsNotNull();

            this.Id = id;
            this.Strategy = strategy;
            this.RuleType = ruleType;

            this.Dependency = new DependencyOf<StoreAccessRule>(this);
        }
        #endregion

        #region IHasId
        public string Id
        {
            get;
            private set;
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region IHasDependencyOf
        public IDependencyOf<StoreAccessRule> Dependency
        {
            get;
            private set;
        }
        #endregion

        #region  Properties
        /// <summary>
        /// Read, Save, or Delete
        /// </summary>
        public StoredItemAccessMode RuleType { get; protected set; }
        /// <summary>
        /// returns true if granted, false if not-granted, null if undecided
        /// </summary>
        public IConditionOf<StoredItemSecurityContext> HasAccessCondition { get; protected set; }
        /// <summary>
        /// If this rules applies to one item, specify it here
        /// </summary>
        public IHasId Item { get; set; }
        #endregion

        #region Methods
        public bool? HasAccess(IHasId item, IUserInfoStore user)
        {
            return this.Strategy(item, user);
        }
        #endregion
    }

    
}
