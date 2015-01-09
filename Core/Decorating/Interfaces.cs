using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Decorating
{
    /// <summary>
    /// most basic definition of a decoration.  
    /// </summary>
    public interface IDecoration 
    {
        object Decorated { get; }
    }


    /// <summary>
    /// a generic decoration
    /// </summary>
    /// <remarks>
    /// Note that the implementors of this MUST also derive from T - which really is the whole point of a decoration.
    /// Only c# can't have a generic type inherit from the generic arg type, so it can't be declared here.
    /// The "This" property declares a conversion to T, explicitly.
    /// </remarks>
    public interface IDecorationOf<T> : IDecoration
    {
        /// <summary>
        /// in a chain of decorations, it's the core value being decorated
        /// </summary>
        T Core { get; }
        /// <summary>
        /// the immediate thing we are decorating
        /// </summary>
        new T Decorated { get; }
        /// <summary>
        /// MUST return a reference to this but cast as T.  This gets around the c# generic inheritance language constraint
        /// </summary>
        T This { get; }

        /// <summary>
        /// Essentially is a clone mechanism.  Allow the current decoration to recreate an instance like itself when
        /// provided with a thing to decorate - think of this as a ctor with only one arg (the thing) and all other args
        /// coming from the current instance.
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        IDecorationOf<T> ApplyThisDecorationTo(T thing);
    }


}
