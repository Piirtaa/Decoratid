using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Reflection
{
    /// <summary>
    /// Defines an interface that can read/write a type's properties. 
    /// </summary>
    public interface IObjectInspector 
    {
        Type Type { get; }
        object GetProperty(object obj, string name);
        void SetProperty(object obj, string name, object value);
    }
}
