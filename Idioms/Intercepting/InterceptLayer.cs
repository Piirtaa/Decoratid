using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Storing.Products;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Dependencies;
using System.Runtime.Serialization;
using Decoratid.Core.Logical;
using Decoratid.Idioms.ValuesOf;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.Depending;

namespace Decoratid.Idioms.Intercepting
{
    /// <summary>
    /// defines a named layer that intercepts a Func of TArg,TResult and decorates it with strategies controlling the pipeline.
    /// </summary>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public class InterceptLayer<TArg, TResult> : IHasId<string>, IHasDependencyOf<string>
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public InterceptLayer(
                string id,
                Func<TArg, TArg> argDecorator,
                Action<TArg> argValidator,
                Func<TArg, TResult> action,
                Func<TResult, TResult> resultDecorator,
                Action<TResult> resultValidator
            )
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
            this.Dependency = new DependencyOf<string>(this.Id);
            this.ArgDecorator = argDecorator.MakeLogicOfTo();
            this.ArgValidator = argValidator.MakeLogicOf();
            this.Action = action.MakeLogicOfTo();
            this.ResultDecorator = resultDecorator.MakeLogicOfTo();
            this.ResultValidator = resultValidator.MakeLogicOf();
        }

        public InterceptLayer(string id)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
            this.Dependency = new DependencyOf<string>(this.Id);
        }
        #endregion

       
        #region IHasId
        public string Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region IHasDependencyOf
        public IDependencyOf<string> Dependency { get; private set; }
        #endregion

        #region Placeholder
        public InterceptLayer<TArg, TResult> BaseLayer { get; set; }
        #endregion

        #region Properties
        public LogicOfTo<TArg, TArg> ArgDecorator { get; private set; }
        public LogicOf<TArg> ArgValidator { get; private set; }
        /// <summary>
        /// Set this if the intercept has some logic to replace the core action being performed
        /// </summary>
        public LogicOfTo<TArg, TResult> Action { get; private set; }
        public LogicOfTo<TResult, TResult> ResultDecorator { get; private set; }
        public LogicOf<TResult> ResultValidator { get; private set; }
        #endregion

        #region Fluent Methods
        public InterceptLayer<TArg, TResult> SetArgDecoration(Func<TArg, TArg> strategy)
        {
            this.ArgDecorator = strategy.MakeLogicOfTo();
            return this;
        }
        public InterceptLayer<TArg, TResult> SetArgDecoration(LogicOfTo<TArg, TArg> strategy)
        {
            this.ArgDecorator = strategy;
            return this;
        }
        public InterceptLayer<TArg, TResult> SetArgValidation(Action<TArg> strategy)
        {
            this.ArgValidator = strategy.MakeLogicOf();
            return this;
        }
        public InterceptLayer<TArg, TResult> SetArgValidation(LogicOf<TArg> strategy)
        {
            this.ArgValidator = strategy;
            return this;
        }
        public InterceptLayer<TArg, TResult> SetAction(Func<TArg, TResult> action)
        {
            this.Action = action.MakeLogicOfTo();
            return this;
        }
        public InterceptLayer<TArg, TResult> SetAction(LogicOfTo<TArg, TResult> action)
        {
            this.Action = action;
            return this;
        }
        public InterceptLayer<TArg, TResult> SetResultDecoration(Func<TResult, TResult> strategy)
        {
            this.ResultDecorator = strategy.MakeLogicOfTo();
            return this;
        }
        public InterceptLayer<TArg, TResult> SetResultDecoration(LogicOfTo<TResult, TResult> strategy)
        {
            this.ResultDecorator = strategy;
            return this;
        }
        public InterceptLayer<TArg, TResult> SetResultValidation(Action<TResult> strategy)
        {
            this.ResultValidator = strategy.MakeLogicOf();
            return this;
        }
        public InterceptLayer<TArg, TResult> SetResultValidation(LogicOf<TResult> strategy)
        {
            this.ResultValidator = strategy;
            return this;
        }
        public InterceptLayer<TArg, TResult> AddDependency(string id)
        {
            this.Dependency.Prerequisites.Add(id);
            return this;
        }
        #endregion

        #region Methods
        public TArg DecorateArg(TArg arg)
        {
            if (this.ArgDecorator == null)
                return arg;
            return this.ArgDecorator.CloneAndPerform(arg.ValueOf());
        }
        public void ValidateArg(TArg arg)
        {
            if (this.ArgValidator == null)
                return;
            this.ArgValidator.CloneAndPerform(arg.ValueOf());
        }
        public TResult DecorateResult(TResult res)
        {
            if (this.ResultDecorator == null)
                return res;
            return this.ResultDecorator.CloneAndPerform(res.ValueOf());
        }
        public void ValidateResult(TResult res)
        {
            if (this.ResultValidator == null)
                return;
            this.ResultValidator.CloneAndPerform(res.ValueOf());
        }
        public TResult Do(TArg arg)
        {
            //recurse 
            if (this.Action == null)
                return this.BaseLayer.Do(arg);

            return this.Action.CloneAndPerform(arg.ValueOf());
        }
        #endregion

    }
}
