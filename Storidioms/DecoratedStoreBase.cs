using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Decoratid.Extensions;
using System.Threading;

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
#if DEBUG
            Debug.WriteLine(string.Format("{0} {1} Store getting {2}", Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, soId.With(x => x.ToString())));
#endif

            var rv = this.Decorated.Get(soId);

#if DEBUG
            Debug.WriteLine(string.Format("{0} {1} Store get returns {2}", Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, rv.With(x => x.GetStoredObjectId().ToString())));
#endif
            return rv;
        }
        public virtual List<T> Search<T>(SearchFilter filter) where T : IHasId
        {
#if DEBUG
            Debug.WriteLine(string.Format("{0} {1} Store searching", Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));
#endif
            var rv = this.Decorated.Search<T>(filter);
#if DEBUG
            rv.WithEach(x =>
            {
                Debug.WriteLine(string.Format("{0} {1} Store search returns {2}", Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.GetStoredObjectId().ToString()));
            });
#endif
            return rv;
        }

        public virtual void Commit(ICommitBag bag)
        {
#if DEBUG
            bag.ItemsToSave.WithEach(x =>
            {
                Debug.WriteLine(string.Format("{0} {1} Store saving {2}", Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.GetStoredObjectId().ToString()));
            });
            bag.ItemsToDelete.WithEach(x =>
            {
                Debug.WriteLine(string.Format("{0} {1} Store deleting {2}", Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.ToString()));
            });
#endif
            this.Decorated.Commit(bag);
        }
        public virtual List<IHasId> GetAll()
        {
#if DEBUG
            Debug.WriteLine(string.Format("{0} {1} Store get all", Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));
#endif
            var rv = this.Decorated.GetAll();
#if DEBUG
            rv.WithEach(x =>
            {
                Debug.WriteLine(string.Format("{0} {1} Store get all returns {2}", Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.GetStoredObjectId().ToString()));
            });
#endif
            return rv;
        }
        #endregion


    }
}
