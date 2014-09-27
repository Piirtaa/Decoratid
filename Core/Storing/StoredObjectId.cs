using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Extensions;
using System.Reflection;
using Decoratid.Core.Identifying;
using Decoratid.Core.Contextual;

namespace Decoratid.Core.Storing
{
    /// <summary>
    /// holds id and type info for an IHasId instance - the fundamental pointer to an object in a store.
    /// Is itself an IHasId and thus can be stored.
    /// </summary>
    [Serializable]
    public sealed class StoredObjectId : IStoredObjectId, IEquatable<StoredObjectId>
    {
        #region Ctor
        /// <summary>
        /// explicitly specifies the type and id of the stored objct
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="id"></param>
        public StoredObjectId(Type objectType, object id)
        {
            Condition.Requires(objectType).IsNotNull();
            Condition.Requires(id).IsNotNull();

            //copy the id/type over
            this._objectId = id;
            this._objectType = objectType;
        }

        /// <summary>
        /// ctor takes an IHasId.  Kacks if it isn't.
        /// </summary>
        public StoredObjectId(IHasId iHasId)
        {
            Condition.Requires(iHasId).IsNotNull();

            //copy the id/type over
            this._objectId = iHasId.Id;
            this._objectType = iHasId.GetType();

        }
        #endregion

        #region Properties
        private readonly Type _objectType;
        private readonly object _objectId;
        /// <summary>
        /// the stored object type
        /// </summary>
        public Type ObjectType { get { return this._objectType; } }
        /// <summary>
        /// the stored object id
        /// </summary>
        public object ObjectId { get { return this._objectId; } }
        #endregion

        #region IHasId
        public Tuple<Type, object> Id
        {
            get { return new Tuple<Type, object>(this.ObjectType, this.ObjectId); }
        }

        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            return Equals(obj as StoredObjectId);
        }
        /// <summary>
        /// returns {Type}:{Id}
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", this.ObjectType.Name, this.ObjectId.ToString());
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public bool Equals(StoredObjectId other)
        {
            if (other == null)
                return false;

            if (!(other.ObjectType.Equals(this.ObjectType)))
                return false;

            return other.ObjectId.Equals(this.ObjectId);
        }
        #endregion

        #region Static Fluent Methods
        public static StoredObjectId New(Type objectType, object id)
        {
            return new StoredObjectId(objectType, id);
        }
        public static StoredObjectId New(IHasId iHasId)
        {
            return new StoredObjectId(iHasId);
        }
        #endregion
    }

    public static class SOIDExtensions
    {
        /// <summary>
        /// creates a stored object id that points to AsId instance
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static StoredObjectId CreateSOID4AsId<TId>(this TId obj)
        {
            return StoredObjectId.New(typeof(AsId<TId>), obj);
        }
        /// <summary>
        /// creates a stored object id that points to AsHasId instance
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static StoredObjectId CreateSOID4AsHasId<TId, T>(this TId obj)
        {
            return StoredObjectId.New(typeof(AsHasId<TId, T>), obj);
        }

        /// <summary>
        /// creates a stored object id for ContextualAsId
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static StoredObjectId CreateSOID4ContextualAsId<TId, TContext>(this TId obj)
        {
            return StoredObjectId.New(typeof(ContextualId<TId, TContext>), obj);
        }
    }

    ///// <summary>
    ///// generic version of StoreObjectId.  Useful when we want to specify a generic pointer to constrain types.  Also is tighter language.
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// 
    //[Serializable]
    //public class StoredObjectIdOf<T> : IStoredObjectId 
    //{
    //    #region Ctor
    //    public StoredObjectIdOf(T id)
    //    {
    //        this._objectId = id;
    //    }
    //    #endregion

    //    #region Properties
    //    private readonly object _objectId;
    //    public object ObjectId { get { return this._objectId; } }
    //    public Type ObjectType { get { return typeof(T); } }
    //    #endregion

    //    #region IHasId
    //    public Tuple<Type, object> Id
    //    {
    //        get { return new Tuple<Type, object>(this.ObjectType, this.ObjectId); }
    //    }

    //    object IHasId.Id
    //    {
    //        get { return this.Id; }
    //    }
    //    #endregion

    //    #region Overrides
    //    public override bool Equals(object obj)
    //    {
    //        if (obj == null)
    //            return false;

    //        if (!(obj is StoredObjectId))
    //            return false;

    //        StoredObjectId sor = (StoredObjectId)obj;
    //        if (!(sor.ObjectType.Equals(this.ObjectType)))
    //            return false;

    //        return sor.ObjectId.Equals(this.ObjectId);
    //    }
    //    /// <summary>
    //    /// returns {Type}:{Id}
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string ToString()
    //    {
    //        return string.Format("{0}:{1}", this.ObjectType.Name, this.ObjectId.ToString());
    //    }
    //    public override int GetHashCode()
    //    {
    //        return this.ToString().GetHashCode();
    //    }
    //    #endregion


    //}

}
