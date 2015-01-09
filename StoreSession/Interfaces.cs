using Decoratid.Core.Identifying;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.StoreSession
{
    /*
     *  Command Processing Loop:
     *      Commands take the following format: {Thing} {Operation} {Values}
     * 
     *      Steps:
     *      1- parse into command format
     *      2- get object instance corresponding to Thing
     *          this: returns the store
     *          context: returns the current context instance
     *          context[#]: returns the current context item
     *          literal : returns the item in the store corresponding to this id         
     *          expression : performs a search in the store to values matching this expression
     * 
     *      3- for the thing (eg. store, context, store item, expression/store items)
     *          find the operation.  this corresponds to the ParsableOperationMap for the given
     *          thing - a hash of operation names to Func<thing,string[], OperationResult(string|error|success flag)>
     *          
     *          how to get the ParsableOperationMap?
     *            for each decoration layer, if type implements IHasParsableOperationMap, aggregate the dictionary entries
     *          
     *      4- evaluate the operation using the passed in arg values
     *          with the operation result, dump the output to results pane
     *          each cli entry will correspond to some output in the results pane
     *          the events pane is also around to handle session events 
     *              (when building up things to host in the session, we may want to subscribe to some events
     *              for our session to handle. )
     *          
     * 
     *      Session instance:
     *          subscribes to events - need to put subscription keyword standard
     *          keeps list of events received
     *          keeps list of results received
     *          has cli method - submit(text) and submit(thing, operation, data)
     *          
     *      Expressions:
     *      each idiom/decoration is searchable using the following expression syntax:
     *      
     *      |decoration values|
     *      or |decoration conditional values| where conditional is a decoration-specific name/key to a IConditionOf<decoration>
     *      
     *      a search expression that works on multiple decorations would look like
     *      |decoration1 conditional values|decoration2 conditional values|decoration3 conditional values|
     * 
     *      eg. HasDateCreated
     *      HasDateCreated YYYYMMDD:HHMMSS is the equivalence condition 
     *      HasDateCreated Before YYYYMMDD:HHMMSS is the before condition
     *      HasDateCreated After YYYYMMDD:HHMMSS is the after condition
     *      
     *      in this way the signature of the decoration is the equivalence signature
     *      |HasDateCreated YYYYMMDD:HHMMSS|
     *      
     *      the perfy-text search stuff is what will parse substrings out of decoration signatures/indexing
     *      
     *      Decoration Signature:
     *      |decoration1 values|decoration2 values|decoration3 values| for the entire decoration chain
     *      
     *      a trie is made, with nodes for each decoration signature
     *          "|decoration " is the trie key
     *          
     *      thus to quickly find all instances with a particular decoration we create an expression such as
     *      "|decoration Equals values|" 
     *      and this will pattern match all "|decoration" nodes
     * 
     */
 
    public interface IThing
    {
        IHasId<string> IHasId { get; }
        
    }

    public interface ISessionStore : IStoreWithIdOf<string>, IStoreOfUniqueId
    {
    
    }

    public interface ISession
    {
        ISessionStore Store { get; }
        
        object Context { get; set; }
        object this[int index]
        {
            get;
            set;
        }
    }
    /// <summary>
    /// defines data parsed from string[]
    /// </summary>
    public interface ICommandLineArgument
    {
        string ThingArg { get; }
        string ActionArg { get; }
        string[] ArgArgs { get; }

        object Thing { get; }

    }
}
