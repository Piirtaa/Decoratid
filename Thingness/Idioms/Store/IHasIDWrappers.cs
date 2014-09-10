using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;

namespace Decoratid.Thingness.Idioms.Store
{
    /// <summary>
    /// wraps something into an id of itself
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public class AsId<TId> : IHasId<TId>
    {
        #region Ctor
        public AsId() { }
        public AsId(TId obj)
        {
            this.Id = obj;
        }
        #endregion

        #region Calculated Properties
        public TId Id { get; set; }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Fluent Static
        public static AsId<TId> New(TId obj)
        {
            return new AsId<TId>(obj);
        }
        public static StoredObjectId CreateObjectId(TId obj)
        {
            return StoredObjectId.New(typeof(AsId<TId>), obj);
        }
        #endregion
    }

    /// <summary>
    /// wraps something into an id of itself with some contextual object.  pretty much a generic container
    /// where you supply the id and object to save.  As with all containers, one must know the exact type of the
    /// container to retrieve it with a get.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public class ContextualAsId<TId,TContext> : IHasId<TId>, IHasContext<TContext>
    {
        #region Ctor
        public ContextualAsId() { }
        public ContextualAsId(TId obj, TContext context)
        {
            this.Id = obj;
            this.Context = context;
            
        }
        #endregion

        #region IHasContext
        public TContext Context { get; set; }
        object IHasContext.Context
        {
            get
            {
                return this.Context;
            }
            set
            {
                this.Context = (TContext)value;
            }
        }
        #endregion

        #region Calculated Properties
        public TId Id { get; set; }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Fluent Static
        public static ContextualAsId<TId, TContext> New(TId obj, TContext context)
        {
            return new ContextualAsId<TId, TContext>(obj, context);
        }
        /// <summary>
        /// creates a storedobjectid
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static StoredObjectId CreateStoredObjectId(TId obj)
        {
            return StoredObjectId.New(typeof(ContextualAsId<TId, TContext>), obj);
        }
        #endregion


    }
        

    /// <summary>
    /// Returns an object wrapped as an id.  Thus the name, AsId.  If a type doesn't implement IHasId, but it has a field (or something else) that can be used as a proxy for Id,
    /// we use this class which wraps instances as IHasId using the specified GetId strategy.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="T"></typeparam>
    public sealed class AsHasId<TId,T> : IHasId<TId>
    {
        #region Ctor
        public AsHasId(T objectToWrap, Func<T, TId> getIdStrategy)
        {
            Condition.Requires(getIdStrategy).IsNotNull();
            this.WrappedInstance = objectToWrap;
            this.GetIdStrategy = getIdStrategy;
        }
        #endregion

        #region Properties
        public T WrappedInstance { get; private set; }
        private Func<T, TId> GetIdStrategy { get; set; }
        #endregion

        #region Calculated Properties
        public TId Id
        {
            get
            {
                return this.GetIdStrategy(this.WrappedInstance);
            }
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Fluent Static
        public static AsHasId<TId, T> New(T objectToWrap, Func<T, TId> getIdStrategy)
        {
            return new AsHasId<TId, T>(objectToWrap, getIdStrategy);
        }
        public static StoredObjectId CreateObjectId(TId obj)
        {
            return StoredObjectId.New(typeof(AsHasId<TId, T>), obj);
        }
        #endregion
    }


}
