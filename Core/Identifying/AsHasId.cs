using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Identifying
{

    /// <summary>
    /// Returns an object wrapped as an id.  Thus the name, AsId.  If a type doesn't implement IHasId, but it has a field (or something else) that can be used as a proxy for Id,
    /// we use this class which wraps instances as IHasId using the specified GetId strategy.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="T"></typeparam>
    public sealed class AsHasId<TId, T> : IHasId<TId>
    {
        #region Ctor
        public AsHasId(T objectToWrap, Func<T, TId> getIdStrategy)
        {
            if (objectToWrap == null)
                throw new ArgumentNullException("objectToWrap");

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

        #endregion
    }

    public static partial class AsHasIdExtensions
    {
        public static AsHasId<TId, T> BuildAsHasId<TId, T>(T objectToWrap, Func<T, TId> getIdStrategy)
        {
            return new AsHasId<TId, T>(objectToWrap, getIdStrategy);
        }
    }
}
