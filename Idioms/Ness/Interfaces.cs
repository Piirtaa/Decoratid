using Decoratid.Core;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Ness
{
    /// <summary>
    /// defines Ness as a set of behaviours associated with an idiomatic name.
    /// </summary>
    public interface INess : IHasId<string>
    {
        Type DecoratedType { get; }
        Type DecoratingType { get; }
        string Name { get; }

        object PerformOperation(string opName, object thing, params object[] args);
        bool? IsConditionTrue(string conditionName, object thing, params object[] args);
        object DecorateWith(object thing, params object[] args);

        IStoreOf<INessOperation> OperationStore { get; }
        IStoreOf<INessCondition> ConditionStore { get; }
    }

    public interface ITypedNess<Tness, Tdecorated> : INess
    {
        LogicOfTo<Arg<Tdecorated>, Tness> DecoratorLogic { get; }
    }

    public interface INessOperation : IHasId<string>
    {
        object PerformOperation(object thing, params object[] args);
    }
    public interface INessOperation<Tness, Tres> : INessOperation
    {
        LogicOfTo<Arg<Tness>, Tres> Logic { get; }
    }
    public interface INessCondition : IHasId<string>
    {
        bool? IsConditionTrue(object thing, params object[] args);
    }
    public interface INessCondition<Tness> : INessCondition
    {
        IConditionOf<Arg<Tness>> Conditional { get; }
    }

    /*
     * //create a ness "ness" of T.  T is the decorating type
     * NessDef<T>.New("ness")
     * 
     * //fluently add a condition
     * .AddCondition(
     * 
     * //build an operation
     * 
     * 
     * //build an arg (the container of the data).  ILogic only takes one arg and returns only one arg, so we have to fluently
     *      build up the arg as a container.  
     *      
     * 
     * 
     * 
     * NotImplementedLogicOf<T>.New()
     *  .HasId<string>("operation1")
     *  .HasArg<string>("stringarg1")
     *  .HasArg<int>("countarg2")
     *  
     * 
     * 
     * 
     */ 
}
