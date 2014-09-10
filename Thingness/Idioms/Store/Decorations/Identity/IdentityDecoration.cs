using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Store.Decorations.Common;
using Sandbox.Thingness.Dependencies;
using Sandbox.Extensions;
using Sandbox.Store.Decorations.Intercepting;
using Sandbox.Store.Decorations.Eventing;
using Sandbox.Interception;
using Sandbox.Thingness;
using Sandbox.Interception.Decorating;

namespace Sandbox.Store.Decorations.Security
{

    /// <summary>
    /// implements secured store.  uses intercepting and eventing docorations to do this
    /// </summary>
    /// <remarks>
    /// Using IInterceptingStore as the framework upon which to extend security,
    /// we add the following intercepts:
    /// 
    /// On All operations (Get, Search, Commit)
    /// 	DecorateArg - decorate with current user 
    /// 	ValidateArg - validate we have a user
    /// 
    /// On Get operation
    /// 	ValidateResult - validate user has Get access, else filter to null
    /// 
    /// On Search operation
    /// 	ValidateResult - validate user has Get access, else filter items out
    /// 
    /// On Commit operation
    /// 	ValidateArg - validate user has Save and Delete perms on items to be saved
    /// </remarks>
    public class SecureDecoration : AbstractDecoration, ISecuredStore, IInterceptingStore, IEventingStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public SecureDecoration(IStore rulesStore, IStore decorated)
            : base(new EventingDecoration(decorated))
        {
            Condition.Requires(rulesStore).IsNotNull();
            this.RuleStore = rulesStore;

            #region Auth Token Intercepts
            this.InterceptCore.GetOperationIntercept.AddNextIntercept("auth",
                (tuple) =>
                {
                    //decorate with the current user
                    return new Tuple<Tuple<Type, IHasId>, object>(tuple.LastCore, CurrentUser.UserStore);
                },
                (tuple) =>
                {
                    //validate that we have a user
                    Condition.Requires(tuple.GetDecorationById("auth").Extension).IsNotNull();
                },
                null, null);

            this.InterceptCore.SearchOperationIntercept.AddNextIntercept("auth",
                (tuple) =>
                {
                    //decorate with the current user
                    return new Tuple<Tuple<Type, SearchFilter>, object>(tuple.LastCore, CurrentUser.UserStore);
                },
                (tuple) =>
                {
                    //validate that we have a user
                    Condition.Requires(tuple.GetDecorationById("auth").Extension).IsNotNull();
                },
                null, null);

            this.InterceptCore.CommitOperationIntercept.AddNextIntercept("auth",
                (tuple) =>
                {
                    //decorate with the current user
                    return new Tuple<CommitBag, object>(tuple.LastCore, CurrentUser.UserStore);
                },
                (tuple) =>
                {
                    //validate that we have a user
                    Condition.Requires(tuple.GetDecorationById("auth").Extension).IsNotNull();
                },
                null, null);
            #endregion

            #region Filtering Intercepts
            this.InterceptCore.GetOperationIntercept.AddNextIntercept("filter",
                null, null,
                (tuple) =>
                {
                    //set the core to null if the user doesn't have access
                    if (!this.HasAccessToItem(StoredItemAccessMode.Read, tuple.LastCore, tuple.GetDecorationById("auth").Extension as IUserInfoStore))
                    {
                        return new Tuple<IHasId, object>(null, null);
                    }
                    return null;//do nothing
                },
                null
                );

            this.InterceptCore.SearchOperationIntercept.AddNextIntercept("filter",
                null, null,
                (tuple) =>
                {
                    List<IHasId> itemsToRemove = new List<IHasId>();

                    tuple.LastCore.WithEach(item =>
                    {
                        //set the core to null if the user doesn't have access
                        if (!this.HasAccessToItem(StoredItemAccessMode.Read, item, tuple.GetDecorationById("auth").Extension as IUserInfoStore))
                        {
                            itemsToRemove.Add(item);
                        }
                    });

                    //remove those items from the returned list
                    List<IHasId> newItems = new List<IHasId>(tuple.LastCore);
                    itemsToRemove.WithEach(item =>
                    {
                        newItems.Remove(item);
                    });

                    return new Tuple<List<IHasId>, object>(newItems, null);
                },
                null
                );

            this.InterceptCore.CommitOperationIntercept.AddNextIntercept("filter",
                null,
                (tuple) =>
                {
                    //check access
                    tuple.LastCore.ItemsToDelete.WithEach(del =>
                    {
                        HasAccessToItemAndThrow(StoredItemAccessMode.Delete, del.Value, tuple.GetDecorationById("auth").Extension as IUserInfoStore);
                    });

                    tuple.LastCore.ItemsToSave.WithEach(save =>
                    {
                        HasAccessToItemAndThrow(StoredItemAccessMode.Save, save.Value, tuple.GetDecorationById("auth").Extension as IUserInfoStore);
                    });
                },
                null, null);
            #endregion
        }
        #endregion

        #region IInterceptingStore
        protected InterceptingDecoration InterceptCore
        {
            get
            {
                return this.FindDecoratorOf<InterceptingDecoration>(true);
            }
        }
        public DecoratingInterceptChain<Tuple<Type, IHasId>, IHasId> GetOperationIntercept
        {
            get
            {
                return this.InterceptCore.GetOperationIntercept;
            }
            set
            {
                this.InterceptCore.GetOperationIntercept = value;
            }
        }

        public DecoratingInterceptChain<Tuple<Type, SearchFilter>, List<IHasId>> SearchOperationIntercept
        {
            get
            {
                return this.InterceptCore.SearchOperationIntercept;
            }
            set
            {
                this.InterceptCore.SearchOperationIntercept = value;
            }
        }

        public DecoratingInterceptChain<CommitBag, Thingness.Void> CommitOperationIntercept
        {
            get
            {
                return this.InterceptCore.CommitOperationIntercept;
            }
            set
            {
                this.InterceptCore.CommitOperationIntercept = value;
            }
        }
        #endregion

        #region IEventingStore
        protected EventingDecoration EventingCore
        {
            get
            {
                return this.FindDecoratorOf<EventingDecoration>(true);
            }
        }
        //wire to the eventing layer
        public event EventHandler<EventArgOf<IHasId>> ItemRetrieved
        {
            add
            {
                this.EventingCore.ItemRetrieved += value;
            }
            remove
            {
                this.EventingCore.ItemRetrieved -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemRetrievedFiltered
        {
            add
            {
                this.EventingCore.ItemRetrievedFiltered += value;
            }
            remove
            {
                this.EventingCore.ItemRetrievedFiltered -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemSaved
        {
            add
            {
                this.EventingCore.ItemSaved += value;
            }
            remove
            {
                this.EventingCore.ItemSaved -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemSavedFiltered
        {
            add
            {
                this.EventingCore.ItemSavedFiltered += value;
            }
            remove
            {
                this.EventingCore.ItemSavedFiltered -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemDeleted
        {
            add
            {
                this.EventingCore.ItemDeleted += value;
            }
            remove
            {
                this.EventingCore.ItemDeleted -= value;
            }
        }
        public event EventHandler<EventArgOf<IHasId>> ItemDeletedFiltered
        {
            add
            {
                this.EventingCore.ItemDeletedFiltered += value;
            }
            remove
            {
                this.EventingCore.ItemDeletedFiltered -= value;
            }
        }
        #endregion

        #region ISecuredStore
        public IStore RuleStore { get; private set; }
        #endregion

        #region RuleStore Methods
        protected void HasAccessToItemAndThrow(StoredItemAccessMode ruleType, IHasId storeItem, IUserInfoStore user)
        {
            if (!this.HasAccessToItem(ruleType, storeItem, user))
            {
                throw new StoreSecurityException(storeItem, user, ruleType);
            }
        }
        protected bool HasAccessToItem(StoredItemAccessMode ruleType, IHasId storeItem, IUserInfoStore user)
        {
            var rules = this.GetItemRulesForUser(ruleType, storeItem, user);

            //if no rules are present, default to no access
            if (rules == null || rules.Count == 0)
                return false;

            foreach (var each in rules)
            {
                var res = each.HasAccess(storeItem, user);
                //if it's doesn't have a value the rule is skipped
                if (!res.HasValue)
                {
                    continue;
                }

                return res.Value;
            }

            return false;
        }

        /// <summary>
        /// for a particular stored item, accessing user, and access mode, get all applicable rules, in order of least 
        /// dependent rule to most
        /// </summary>
        /// <param name="ruleType"></param>
        /// <param name="storeItem"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        protected List<StoreAccessRule> GetItemRulesForUser(StoredItemAccessMode ruleType, IHasId storeItem, IUserInfoStore user)
        {
            //first look for item specific rules, and null item rules
            SearchFilterOf<StoreAccessRule> filter = new SearchFilterOf<StoreAccessRule>((x) =>
            {
                if (x.RuleType == ruleType &&
                    (x.Item == null || x.Item.Id.Equals(storeItem.Id)))
                    return true;

                return false;
            });

            var list = RuleStore.Search<StoreAccessRule>(filter);

            List<IHasDependencyOf<StoreAccessRule>> unsortedList = list.ConvertListTo<IHasDependencyOf<StoreAccessRule>, StoreAccessRule>();

            //now order the list by dependency from least to most
            var sortedList = DependencyUtil.SortHasADependency(unsortedList);

            //return
            var sortedConvertedList = sortedList.ConvertListTo<StoreAccessRule, IHasDependencyOf<StoreAccessRule>>();

            //add ALL overriding rules to the first on the list

            //RULE 1:  The owner will always have full access to item
            sortedConvertedList.Insert(0, new StoreAccessRule("OWNER", ruleType, (ihasId, accessUser) =>
            {
                if (accessUser.Id.Equals(user.Id))
                {
                    return true;
                }
                return null;
            }));

            return sortedConvertedList;
        }
        #endregion


    }
}
