using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Logging;
using Decoratid.Thingness.Idioms.Logics.Decorations;
using Decoratid.Thingness.Idioms.ValuesOf;
using Decoratid.Extensions;

namespace Decoratid.Thingness.Idioms.Logics
{

    /// <summary>
    /// This interface encapsulates the functionality of "Perform", aka go do something.  
    /// </summary>
    /// <remarks>
    /// Some obvious extensions of the "go do something" logic are:
    ///  - go do something with some argument data
    ///  - go do something that returns some data
    ///  
    /// Thus we have the primitive ILogic implementations: 
    ///     ActionLogic - a simple go do something
    ///     ActionLogicOf - takes an argument
    ///     FunctionLogic - returns some data
    ///     FunctionLogicOf - takes an argument, returns some data
    ///     
    /// Essentially, we are wrapping a delegate.  The wrapping gives us a chance to explicitly control the serialization of the 
    /// delegate, and also to restrict the types of delegates we are expecting (ActionLogic, ActionLogicOf, FunctionLogic, FunctionLogicOf).
    /// </remarks>
    public interface ILogic
    {
        void Perform();
    }

    /// <summary>
    /// Logic that can clone itself
    /// </summary>
    public interface ICloneableLogic : ILogic
    {
        ILogic Clone();
    }

    /// <summary>
    /// some logic that requires a context (the Of)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogicOf<T> : ILogic, IHasContext<IValueOf<T>> 
    {
    }

    /// <summary>
    /// some logic that produces a result (the To)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogicTo<T> : ILogic
    {
        T Result { get; }
    }

}
