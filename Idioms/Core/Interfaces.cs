using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core
{
    /*
     * At this layer we define as many "General Concepts" as we can, so that we can facilitate
     * Decoration along these conceptual lines.  For example, this is where "cross-cutting concerns"
     * such as logging, exception trapping, perf monitoring, conditional execution, can be injected
     * into the flow.
     * 
     * The general concepts here that relate to modelling Data and Process are: IHasId, IHasContext, IPerforming.
     * The latter, in particular, is what hooks us into cross-cutting concern territory.
     * 
     * As we drill into more complicated concepts, such as Stores, Graphing, Interception, Reconstitution,
     * we will be further narrowing and specializing the conceptual lines.  Similarly decorations specialize as well,
     * as they follow the conceptual definitions.
     * 
     * 
     *
     */


    #region IHasContext
    public interface IHasContext
    {
        object Context { get; set; }
    }
    /// <summary>
    /// defines something that operates on a generic type for context/state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasContext<T> : IHasContext
    {
        new T Context { get; set; }
    }

    /// <summary>
    /// defines something that has a context (eg. IHasContext) and also has a factory to produce the context value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasContextFactory<T> : IHasContext<T>
    {
        void BuildContext();
    }
    #endregion

    #region IHasId
    /*
        IHasId is what defines a storeable item.
     * 
    */
    /// <summary>
    /// indicate something has an Id of an indeterminate type
    /// </summary>
    public interface IHasId
    {
        object Id { get; }
    }
    public interface IHasSettableId
    {
        void SetId(object id);
    }
    /// <summary>
    /// indicate something has an Id of a known type T
    /// </summary>
    public interface IHasId<T> : IHasId
    {
        new T Id { get; }
    }
    public interface IHasSettableId<T>
    {
        void SetId(T id);
    }
    #endregion

    /// <summary>
    /// indicates a thing performs some action, or does something.  This is the decorative hook upon which all
    /// cross-cutting concerns can be injected (via decorations, which is the essence of this framework)
    /// </summary>
    public interface IPerforming
    {
        void Perform();
    }
}
