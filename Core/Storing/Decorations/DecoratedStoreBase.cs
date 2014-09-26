using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using Decoratid.Extensions;
using Decoratid.Idioms.Dependencies;
using System.Runtime.Serialization;
using System.Reflection;
using Decoratid.Idioms.Decorating;

namespace Decoratid.Idioms.Storing.Decorations
{
    /// <summary>
    /// a store decoration
    /// </summary>
    public interface IDecoratedStore : IStore, IDecorationOf<IStore>
    {
    }

    /// <summary>
    /// abstract class that provides templated implementation of a Decorator/Wrapper of IStore
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class DecoratedStoreBase : DecorationOfBase<IStore>, IDecoratedStore
    {
        #region Ctor
        protected DecoratedStoreBase() :base() { }
        public DecoratedStoreBase(IStore decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Properties
        public override IStore This { get { return this; } }
        #endregion

        #region IStore
        public virtual IHasId Get(IStoredObjectId soId)
        {
            return this.Decorated.Get(soId);
        }
        public virtual List<T> Search<T>(SearchFilter filter) where T : IHasId
        {
            return this.Decorated.Search<T>(filter);
        }
        public virtual void Commit(ICommitBag bag)
        {
            this.Decorated.Commit(bag);
        }
        public virtual List<IHasId> GetAll()
        {
            return this.Decorated.GetAll();
        }
        #endregion


    }
}
