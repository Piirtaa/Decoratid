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
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store get starts {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, soId.With(x => x.ToString())));
#endif

            var rv = this.Decorated.Get(soId);

#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store get returns {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, rv.With(x => x.GetStoredObjectId().ToString())));
#endif
            return rv;
        }
        public virtual List<T> Search<T>(SearchFilter filter) where T : IHasId
        {
#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store search starts", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));
#endif
            var rv = this.Decorated.Search<T>(filter);
#if DEBUG
            rv.WithEach(x =>
            {
                Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store search returns {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.GetStoredObjectId().ToString()));
            });

            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store search ends", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));
#endif
            return rv;
        }

        public virtual void Commit(ICommitBag bag)
        {
#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store commit starts", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));

            bag.ItemsToSave.WithEach(x =>
            {
                Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store saving {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.GetStoredObjectId().ToString()));
            });
            bag.ItemsToDelete.WithEach(x =>
            {
                Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store deleting {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.ToString()));
            });
#endif
            this.Decorated.Commit(bag);

#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store commit ends", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));
#endif
        }
        public virtual List<IHasId> GetAll()
        {
#if DEBUG
            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store get all starts", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));
#endif
            var rv = this.Decorated.GetAll();
#if DEBUG
            rv.WithEach(x =>
            {
                Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store get all returns {3}", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName, x.GetStoredObjectId().ToString()));
            });

            Debug.WriteLine(string.Format("Id:{0}  Thread:{1}  Type:{2} Store get all ends", (this as IHasId).With(o => o.Id).With(o => o.ToString()), Thread.CurrentThread.ManagedThreadId, this.GetType().FullName));

#endif
            return rv;
        }
        #endregion


    }
}
