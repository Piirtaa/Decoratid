using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core
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
     * Decorations:
        *  DecorationOfBase
        *      defines the base class implementation of the Decorator pattern.  (The concept of "Decoration" 
            *  in its various forms is central to Decoratid.)  To implement a decorator requires a thing to decorate, usually
            *  with the "thingness" specified by an interface or base class.  
         *
     *     Polyface
         *     is the facilitator of injecting/compositing behaviour/things into a final, larger thing.  
             * Types which support Polyfacing (eg. implement IPolyfacing), allow their individual behaviours to be linked
             * to a Polyface.  Able to access one another, the faces in a Polyface can wire fluently, and in this way 
             * composite behaviours.

     *      *  The core functions of Polyface and IPolyfacing are:
             *      Is<T>(T behaviour) - which sets the Polyface(creates new Polyface if one doesn't exist) behaviour 
             *      As<T>() - which gets the Polyface behaviour (creates new Polyface if one doesn't exist)
             * 
             *  Thus a Polyface is able to have several faces with the use of Is/As.  And likewise each IPolyfacing face
             *  can be decorated (eg. decorating the face and replacing the Polyface's face with the new decoration.  AKA wrap-placing)
             *  with the use of Is/As.
             *  
             *      For example, the call to decorate a face and wrapplace it is: 
             *          IPolyfacing.As<T>().DecorateWithX().Is<T>();
             *              -get the T face, decorate it, set it     
     * 
     *      In this way we facilitate decoration by adding new Faces and decoration of each Face, itself.
     * 
     * ILogic:
             *  The general idea of ILogic is to provide a decoration point around a simple process.  
     *              At this level we decorate with things like: PerformWhen, Trap and Log errors during performance,
     *              performance benchmarking.  It wraps but does not intercept any data as there is no data 
     *              to intercept in the void Perform() sig.  
     *              
         *              TODO: I guess if we get really fancy we can define another,
         *              fancier interface that enables interception of args and result as with DecoratingIntercptionChain....


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
    
}
