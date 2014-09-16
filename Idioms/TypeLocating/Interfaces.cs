using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.TypeLocating
{
    /// <summary>
    /// ITypeLocator searches for types based on a delegate filter. 
    /// </summary>
    public interface ITypeLocator
    {
        /// <summary>
        /// Returns all types that return bool from the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        List<Type> Locate(Func<Type, bool> filter);
    }

}
