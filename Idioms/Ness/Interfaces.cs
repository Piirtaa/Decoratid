using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;
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

        /// <summary>
        /// using the 
        /// </summary>
        /// <param name="opName"></param>
        /// <param name="thing"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        object PerformOperation(string opName, object thing, params object[] args);
        bool? IsConditionTrue(string conditionName, object thing, params object[] args);
        object DecorateWith(object thing, params object[] args);
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
     *      build up the arg as a container.  IsA works great here. 
     * 
     * ArgOf<T>.New()
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
