using Decoratid.Core.Contextual;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Core.Logical
{

    /// <summary>
    /// This interface encapsulates the functionality of "Logic", aka "go do something".  
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
    /// delegate, and also to restrict the types of delegates we are expecting (Logic, LogicOf, LogicTo, LogicOfTo).
    /// 
    /// The Perform() method is fluent and returns itself.  In the implementations of ILogic we clone the logic, and the clone, 
    /// containing the state of the operation, is returned.  Thus we get a stateful container of the operation but the original operation
    /// logic is stateless.  ILogic is thus ALWAYS STATELESS and should be designed with this in mind.
    /// </remarks>
    public interface ILogic 
    {
        ILogic Perform(object context = null);
    }

    public interface ICloneableLogic: ILogic
    {
        ILogic Clone();
    }
    /// <summary>
    /// some logic that requires a context (the Of)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogicOf<T> : IHasContext<T>, ILogic
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
