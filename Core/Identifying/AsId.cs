using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Identifying
{
    /// <summary>
    /// wraps something into an id of itself
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public class AsId<TId> : IHasId<TId>
    {
        #region Ctor
        public AsId(TId obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

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

        #endregion
    }

    public static partial class AsIdExtensions
    {
        public static AsId<TId> BuildAsId<TId>(this TId obj)
        {
            return new AsId<TId>(obj);
        }
    }
}
