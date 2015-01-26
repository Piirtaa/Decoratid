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

    /*What's the Decoratid concept of "Idiomatic"
     * -has a unique (domain-wide) idiom root name, {Idiom}.  eg. HasId
     * -has human readable representation of its state, aka "the Idiomatique", via the format 
     *      {Idiom}.HasProperty({PropertyName},{Value}).HasProperty({PropertyName},{Value})
     * -can also parse the Idiomatique and hydrate 
     * -as well as have state be human readable via the Idiomatique, we also have "Idiomatic conditional expressions", which
     *  allow for comparison/filtering of IIdiomatic of the same type via the format  
     *      #{Idiom} {Expression} {Data}
     *      eg.( HasId.Equals("myid"), HasDateCreated.After(YYYYMMDDhhmmss))
     *  
     *      These "Idiomatic Conditional Expressions" are also used from the Decoratid Command Line to filter data from 
     *      the current store.  The DCL expects data in the format:
     *          [THING] [ACTION] [DATA]
     *          -where THING is a "SOID EXPRESSION" - <Type>(Id), that leads to a currentStore.Get(SOID)
     *          -or THING is a "Idiomatic Conditional Expression" - #HasId.Equals("myid").Or(#HasId.Equals("yourid"))
     *              -where # prefixes a search expression,  IConditionOf<IIdiomatic>
     *              -these expressions operate on the specified store (if none given, assumes current store)
     *                  eg.  @store.#HasId.Equals("myid") translates to an IConditionOf<IIdiomatic> ??not sure think about this some more
     *          -where ACTION is the "Idiomatic Operation"
     *          
     *          there is autocomplete after the idiom is specified.  ie. once "#HasId." has been entered.  The "." will
     *              do an expression dictionary autocomplete returning "Idiomatic Conditional Expressions" and 
     *              "Idiomatic Operations".
     *              
     * -when constructing an instance of something the use of a "Has{Idiom}(data)" approach will be prevalent.
     *  eg. "myid".AsId().HasDateCreated(#now).HasBits(0111011010).HasName("bro").HasNameValue("sup", "yo,guy")
     * 
     * # # typically connotes an idiom
     * " " connotes a string literal
     * @ @ connotes a session thing (could be current stores, default things, etc)
     * Not sure if to have prefix/suffix tagging to resolve literals (eg. "hey") and keywords (eg. #now#, #today#)
     * 
     */
    /// <summary>
    /// thing converts to a Decoratid Idiomatic textual description
    /// </summary>
    /// <remarks>
    /// Command Format:  
    /// Has {Idiom}.HasProperty({PropertyName},{Value}).HasProperty({PropertyName},{Value})
    /// Decoration Stacks likewise can convert to a series of these lines.
    /// </remarks>
    public interface IIsIdiomatic
    {
        /// <summary>
        /// quick check flag to determine if something is idiomatic
        /// </summary>
        bool IsIdiomatic { get; }

        string IdiomName { get; } //should come from an attribute, just putting it here as a reminder
        /// <summary>
        /// returns the textual representation of state according to the template {Idiom}.HasProperty({PropertyName},{Value}).HasProperty({PropertyName},{Value})
        /// </summary>
        /// <returns></returns>
        string GetIdiomaticRepresentation();
        /// <summary>
        /// sets current state by parsing the command
        /// </summary>
        void ParseIdiomaticRepresentation();
        /// <summary>
        /// sets the property to the value.  should only work on idiomatic properties
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="val"></param>
        void HasProperty(string propertyName, object val); 
    }

}
