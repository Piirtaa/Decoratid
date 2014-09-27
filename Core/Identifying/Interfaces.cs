using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Identifying
{

    #region IHasId
    /// <summary>
    /// indicate something has an Id of an indeterminate type
    /// </summary>
    public interface IHasId
    {
        object Id { get; }
    }
    public interface IHasSettableId
    {
        void SetId(object id);
    }
    /// <summary>
    /// indicate something has an Id of a known type T
    /// </summary>
    public interface IHasId<T> : IHasId
    {
        new T Id { get; }
    }
    public interface IHasSettableId<T>
    {
        void SetId(T id);
    }
    #endregion
    
}
