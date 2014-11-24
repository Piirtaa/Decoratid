using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Core.ValueOfing;
using System.Runtime.Serialization;

namespace Decoratid.Storidioms.Factoried
{
    /// <summary>
    /// a store that can build/save an object from its get, if the get returns nothing
    /// </summary>
    public interface IFactoryStore : IDecoratedStore
    {
        /// <summary>
        /// the strategy used to create an object from an IHasId, when it cannot be found by a Get
        /// </summary>
        LogicOfTo<IStoredObjectId, IHasId> Factory { get; set; }
    }

    /// <summary>
    /// has a factory to build/save an object if it isn't not avail on a get
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FactoryDecoration : DecoratedStoreBase, IFactoryStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public FactoryDecoration(LogicOfTo<IStoredObjectId, IHasId> factory, IStore decorated)
            : base(decorated)
        {
            Condition.Requires(factory).IsNotNull();
            this.Factory = factory;
        }
        #endregion

        #region ISerializable
        protected FactoryDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Factory = (LogicOfTo<IStoredObjectId, IHasId>)info.GetValue("Factory", typeof(LogicOfTo<IStoredObjectId, IHasId>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Factory", Factory);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IFactoryDecoration
        public LogicOfTo<IStoredObjectId, IHasId> Factory { get; set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var returnValue = new FactoryDecoration(this.Factory, store);

            return returnValue;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// looks for the object but if it isn't there it will be created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IHasId Get(IStoredObjectId soId)
        {
            var retval = this.Decorated.Get(soId);

            //not here
            if (retval == null)
            {
                //build it
                lock (this._stateLock)
                {
                    var factoryLogic = this.Factory.Perform(soId) as LogicOfTo<IStoredObjectId, IHasId>;
                    retval = factoryLogic.Result;
                    //save it
                    if (retval != null)
                    {
                        this.SaveItem(retval);
                    }
                }
            }
            return retval;
        }
        #endregion
    }

    public static class FactoryDecorationExtensions
    {
        /// <summary>
        /// gets the factory layer
        /// </summary>
        /// <param name="decorated"></param>
        /// <returns></returns>
        public static FactoryDecoration GetFactory(this IStore decorated)
        {
            return decorated.FindDecoratorOf<FactoryDecoration>(true);
        }

        /// <summary>
        /// provides a factory on the get if an item doesn't exist
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static FactoryDecoration HasFactory(this IStore decorated, LogicOfTo<IStoredObjectId, IHasId> factory)
        {
            Condition.Requires(decorated).IsNotNull();
            return new FactoryDecoration(factory, decorated);
        }
    }
}
