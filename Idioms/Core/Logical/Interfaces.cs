using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Logical.Decorations;
using Decoratid.Extensions;
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.Core.Logical
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
    ///     Logic - a simple go do something
    ///     LogicOf - takes an argument
    ///     LogicTo - returns some data
    ///     LogicOfTo - takes an argument, returns some data
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
    public interface ILogicOf<T> : IHasContext<IValueOf<T>>, ILogic
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
