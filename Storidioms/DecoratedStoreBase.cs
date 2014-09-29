using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Decoratid.Storidioms
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
        public DecoratedStoreBase(IStore decorated)
            : base(decorated)
        {
        }
        #endregion

        #region ISerializable
        protected DecoratedStoreBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
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
