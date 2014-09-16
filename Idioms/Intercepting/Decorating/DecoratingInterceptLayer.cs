using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Storing.Products;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Storing;
using Decoratid.Idioms.Dependencies;
using Decoratid.Thingness;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;


namespace Decoratid.Interception.Decorating
{
    /*  In this variation on the InterceptChain, et.al, this does the same basic functionality of providing
     * intercept points before and after a call, arrangeable in an onion.  The difference lies in that the extension
     * and decoration of arg/result data is done using ExtendedDecoration, which is a way of wrapping a layer of 
     * extended data around some core thing.  Extension/Decoration done in this way produces distinct "quantum" of data
     * at each layer of decoration.  Thus, the signature of the layer changes to reflect that we are now decorating
     * Decoration<TArg> and not TArg in our decoration steps.  And likewise, we are decorating Decoration<TResult>, 
     * not TResult.  The signature of the layer changes to:
     * 
        Func<Decoration<TArg>, Decoration<TArg>> ArgDecorator 
        Action<Decoration<TArg>> ArgValidator
        Func<Decoration<TResult>, Decoration<TResult>> ResultDecorator
        Action<Decoration<TResult>> ResultValidator
     * 
     * Note:  Decoration and ExtendedDecoration are immutable. For example, the argument decoration stage mutate the argument 
     * by wrapping immutable layer upon immutable layer.  Each decoration has its Extension, but it is readonly.  Yes, it is
     * possible for an intercept to completely replace the Decoration with its own implementation (and indeed this is how it
     * is supposed to work)
     * 
     *        The design philosophy is that each layer can only know about the things it surrounds.  Each decoration should
     *        only see the decorations it itself is decorating.  
     * 
     * An interesting point is the Action strategy that both takes and returns a decoration.  This gives us the ability to 
     * perform multiple activities within a single Action, possibly having each Extension be used as an argument in a distinct
     * separate activity.  
     * 
     * The process of the interception is as follows:
     *
     *  1.Decorate Arg:
         * foreach layer
         *      Run the ArgDecoratorStrategy and add a layer to the arg decoration
     *  2.Validate Arg:
         * foreach layer
         *      Run the ArgValidatorStrategy and kack if something is wrong
     *  3.Perform Core Action using the last layer of the arg decoration's core value
     *  4.Decorate Result:
         * foreach layer
         *      Run the ResultDecoratorStrategy and add a layer to the result decoration
     *  5.Validate Result:
         * foreach layer
         *      Run the ResultValidaotrStrategy and kack if something is wrong

     */

    /// <summary>
    /// defines the signature of the stages of an intercept
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IDecoratingInterceptLayer<TArg, TResult> : IHasId<string>,
        IHasDependencyOf<string>
    {
        /// <summary>
        /// given the arg decoration, produce the components for another decoration layer
        /// </summary>
        LogicOfTo<Onion<TArg>, OnionLayer<TArg>> ArgDecorator { get; }
        /// <summary>
        /// given the arg decoration, validate
        /// </summary>
        LogicOf<Onion<TArg>> ArgValidator { get; }
        /// <summary>
        /// given the result decoration, produce the components for another decoration layer
        /// </summary>
        LogicOfTo<Onion<TResult>, OnionLayer<TResult>> ResultDecorator { get; }
        /// <summary>
        /// given the result decoration, validate
        /// </summary>
        LogicOf<Onion<TResult>> ResultValidator { get; }
    }

    /// <summary>
    /// an interception layer 
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class DecoratingInterceptLayer<TArg, TResult> : IDecoratingInterceptLayer<TArg, TResult>
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public DecoratingInterceptLayer(
                string id,
                LogicOfTo<Onion<TArg>, OnionLayer<TArg>> argDecorator,
                LogicOf<Onion<TArg>> argValidator,
                LogicOfTo<Onion<TResult>, OnionLayer<TResult>> resultDecorator,
                LogicOf<Onion<TResult>> resultValidator
            )
        {
            Condition.Requires(id).IsNotNullOrEmpty();

            this.Id = id;
            this.Dependency = new DependencyOf<string>(this.Id);

            this.ArgDecorator = argDecorator;
            this.ArgValidator = argValidator;
            this.ResultDecorator = resultDecorator;
            this.ResultValidator = resultValidator;
        }

        public DecoratingInterceptLayer(string id)
        {
            Condition.Requires(id).IsNotNullOrEmpty();

            this.Id = id;

            this.Dependency = new DependencyOf<string>(this.Id);
        }
        #endregion

        #region IHasId
        public string Id
        {
            get;
            private set;
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region IHasDependencyOf
        public IDependencyOf<string> Dependency
        {
            get;
            private set;
        }
        #endregion

        #region Properties
        public LogicOfTo<Onion<TArg>, OnionLayer<TArg>> ArgDecorator { get; private set; }
        public LogicOf<Onion<TArg>> ArgValidator { get; private set; }
        public LogicOfTo<Onion<TResult>, OnionLayer<TResult>> ResultDecorator { get; private set; }
        public LogicOf<Onion<TResult>> ResultValidator { get; private set; }
        #endregion

        #region Fluent Methods
        public DecoratingInterceptLayer<TArg, TResult> SetArgDecoration(LogicOfTo<Onion<TArg>, OnionLayer<TArg>> strategy)
        {
            this.ArgDecorator = strategy;
            return this;
        }
        public DecoratingInterceptLayer<TArg, TResult> SetArgValidation(LogicOf<Onion<TArg>> strategy)
        {
            this.ArgValidator = strategy;
            return this;
        }
        public DecoratingInterceptLayer<TArg, TResult> SetResultDecoration(LogicOfTo<Onion<TResult>, OnionLayer<TResult>> strategy)
        {
            this.ResultDecorator = strategy;
            return this;
        }
        public DecoratingInterceptLayer<TArg, TResult> SetResultValidation(LogicOf<Onion<TResult>> strategy)
        {
            this.ResultValidator = strategy;
            return this;
        }
        public DecoratingInterceptLayer<TArg, TResult> AddDependency(string id)
        {
            this.Dependency.Prerequisites.Add(id);
            return this;
        }
        #endregion


    }
}
