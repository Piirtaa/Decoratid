using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.ValueOfing
{
    /// <summary>
    /// interface giving us facility to decorate getting a value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValueOf<T> 
    {
        T GetValue();
    }
}
