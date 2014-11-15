using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Contextual
{
    #region IHasContext
    public interface IHasContext
    {
        object Context { get; set; }
    }
    /// <summary>
    /// defines something that operates on a generic type for context/state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasContext<T> : IHasContext
    {
        new T Context { get; set; }
    }
    #endregion

    public interface IContextualHasId : IHasId, IHasContext { }
 
    
}
