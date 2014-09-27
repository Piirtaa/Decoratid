using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Conditional;
using Decoratid.Extensions;
using Decoratid.Core.Storing.Decorations.Validating;
using Decoratid.Thingness;
using Decoratid.Idioms.Decorating;

namespace Decoratid.Core.Storing.Decorations.StoreOf
{
    #region  IStore Of Exactly Constructs
    public interface IGettableStoreOfExactly<T> : IGettableStore where T : IHasId
    {
        T GetById(object id);
    }

    public interface IWriteableStoreOfExactly<T> : IValidatingStore where T : IHasId
    {
        ///// <summary>
        ///// hides base/forces implementation of validator to be IsExactlyOfValidator
        ///// </summary>
        //new IsExactlyOfValidator<T> ItemValidator { get; }
    }

    public interface IGetAllableStoreOfExactly<T> : IGetAllableStore where T : IHasId
    {
        new List<T> GetAll();
    }
    /// <summary>
    /// a store of items that are of type T (exact match, no derived types)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStoreOfExactly<T> : IStore,
        IGettableStoreOfExactly<T>, IWriteableStoreOfExactly<T>, IGetAllableStoreOfExactly<T>
        where T : IHasId
    {
    }
    #endregion

    /// <summary>
    /// Turns a store into a "storeOf".  decorates a store such that the only items that can be stored within the store are of type TItem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StoreOfExactlyDecoration<T> : ValidatingDecoration, IStoreOfExactly<T> where T : IHasId
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        protected StoreOfExactlyDecoration() : base() { }
        public StoreOfExactlyDecoration(IStore decorated)
            : base(decorated, new IsExactlyOfValidator<T>())
        {
        }
        #endregion

        //#region Properties
        //public new IsExactlyOfValidator<T> ItemValidator
        //{
        //    get { return base.ItemValidator as IsExactlyOfValidator<T>; }
        //    set { base.ItemValidator = value; }
        //}
        //#endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new StoreOfExactlyDecoration<T>(store);
        }
        #endregion

        #region Overrides
        public new List<T> GetAll()
        {
            return base.GetAll().ConvertListTo<T, IHasId>();
        }
        #endregion

        public virtual T GetById(object id)
        {
            var item = this.Get(StoredObjectId.New(typeof(T), id));

            if (item == null)
                return default(T);

            return (T)item;
        }
    }
}
