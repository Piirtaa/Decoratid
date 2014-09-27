using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Extensions;

namespace Decoratid.Core.Storing
{
    /// <summary>
    /// a store that is implemented with strategies
    /// </summary>
    public class StrategizedStore 
    {
        #region Ctor
        public StrategizedStore(LogicOfTo<IHasId, IHasId> getStrategy, 
            Func<SearchFilter, List<IHasId>> searchStrategy, 
            Action<CommitBag> commitStrategy)
        {
            this.GetStrategy = getStrategy;
            this.SearchStrategy = searchStrategy;
            this.CommitStrategy = commitStrategy;
        }
        #endregion

        #region Properties
        public LogicOfTo<IHasId, IHasId> GetStrategy { get; private set; }
        public Func<SearchFilter, List<IHasId>> SearchStrategy { get; private set; }
        public Action<CommitBag> CommitStrategy { get; private set; }
        #endregion

        #region IStore
        public virtual T Get<T>(IHasId id) where T : IHasId
        {
            Condition.Requires(id).IsNotNull();

            return (T)this.GetStrategy(id);
        }
        public virtual List<T> Search<T>(SearchFilter filter) where T : IHasId
        {
            Condition.Requires(filter).IsNotNull();

            List<T> returnValue = new List<T>();
            Type filterType = typeof(T);

            //lock and retrieve the values
            List<IHasId> vals = this.SearchStrategy(filter);

            vals.WithEach(x =>
            {
                //if the item is the wrong type, skip it
                var type = x.GetType();
                if (filterType.IsAssignableFrom(type))
                {
                    if (filter.Filter((T)x))
                    {
                        returnValue.Add((T)x);
                    }
                }
            });

            return returnValue;
        }
        public virtual void Commit(CommitBag bag)
        {
            Condition.Requires(bag).IsNotNull();

            this.CommitStrategy(bag);
        }
        #endregion
    }
}
