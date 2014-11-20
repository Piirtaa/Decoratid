using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Depending
{   
    /// <summary>
    /// A node (of T), presumably in a graph (of T), that has a list of other nodes (of T) it depends on.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDependencyOf<T>
    {
        T Self { get; }
        List<T> Prerequisites { get; }
    }

    /// <summary>
    /// Has a property, Dependency, that contains the dependency node info (self & deps)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasDependencyOf<T>
    {
        IDependencyOf<T> Dependency { get; }
    }


}
