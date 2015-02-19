using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Core.Decorating
{
    /// <summary>
    /// most basic definition of a decoration.  Something that wraps something else
    /// </summary>
    public interface IDecoration 
    {
        /// <summary>
        /// the immediate thing we are decorating
        /// </summary>
        object Decorated { get; }

        /// <summary>
        /// the coremost decorated thing
        /// </summary>
        object Core { get; }

        
    }

    /// <summary>
    /// a decoration that is aware of who is decorating it
    /// </summary>
    public interface IDecoratorAwareDecoration : IDecoration
    {
        /// <summary>
        /// the thing decorating this
        /// </summary>
        object Decorator { get; set; }
        /// <summary>
        /// the outermost decoration in the stack
        /// </summary>
        object Outer { get; }
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
        new T Core { get; }
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




    ///// <summary>
    ///// thing converts to a Decoratid Idiomatic textual description
    ///// </summary>
    ///// <remarks>
    ///// </remarks>
    //public interface IIsIdiomatic
    //{
    //    /// <summary>
    //    /// quick check flag to determine if something is idiomatic
    //    /// </summary>
    //    bool IsIdiomatic { get; }

    //    string IdiomName { get; } //should come from an attribute, just putting it here as a reminder
    //    /// <summary>
    //    /// returns the textual representation of state according to the template {Idiom}.HasProperty({PropertyName},{Value}).HasProperty({PropertyName},{Value})
    //    /// </summary>
    //    /// <returns></returns>
    //    string GetIdiomaticRepresentation();
    //    /// <summary>
    //    /// sets current state by parsing the command
    //    /// </summary>
    //    void ParseIdiomaticRepresentation();
    //    /// <summary>
    //    /// sets the property to the value.  should only work on idiomatic properties
    //    /// </summary>
    //    /// <param name="propertyName"></param>
    //    /// <param name="val"></param>
    //    void HasProperty(string propertyName, object val); 
    //}

}
