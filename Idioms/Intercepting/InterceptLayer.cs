using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Depending;
using System;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Intercepting
{
    /// <summary>
    /// defines a layer of an interception on a function
    /// </summary>
    /// <remarks>
    /// Each layer has 5 components. 
    ///     decorate/scrub arg
    ///     validate scrubbed arg
    ///     perform
    ///     decorate/scrub result
    ///     validate scrubbed result 
    /// </remarks>
    /// <typeparam name="TArg"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// 
    [Serializable]
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
            this.SetArgDecoration(argDecorator);
            this.SetArgValidation(argValidator);
            this.SetResultDecoration(resultDecorator);
            this.SetResultValidation(resultValidator);
            this.SetAction(action);
        }

        public InterceptLayer(string id)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
            this.Dependency = new DependencyOf<string>(this.Id);
        }
        #endregion

        #region Clone
        public InterceptLayer<TArg, TResult> Clone()
        {
            InterceptLayer<TArg, TResult> rv = new InterceptLayer<TArg, TResult>(this.Id);
            rv.ArgDecorator = this.ArgDecorator.With(x => x.Clone() as LogicOfTo<TArg, TArg>);
            rv.ArgValidator = this.ArgValidator.With(x => x.Clone() as LogicOf<TArg>);
            rv.Action = this.Action.With(x => x.Clone() as LogicOfTo<TArg, TResult>);
            rv.ResultDecorator = this.ResultDecorator.With(x => x.Clone() as LogicOfTo<TResult, TResult>);
            rv.ResultValidator = this.ResultValidator.With(x => x.Clone() as LogicOf<TResult>);
            rv.Id = this.Id;
            return rv;
        }
        #endregion

        #region IHasId
        public string Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region IHasDependencyOf
        public IDependencyOf<string> Dependency { get; private set; }
        #endregion

        #region Properties
        public LogicOfTo<TArg, TArg> ArgDecorator { get; private set; }
        public LogicOf<TArg> ArgValidator { get; private set; }
        public LogicOfTo<TArg, TResult> Action { get; private set; }
        public LogicOfTo<TResult, TResult> ResultDecorator { get; private set; }
        public LogicOf<TResult> ResultValidator { get; private set; }
        #endregion

        #region Fluent Methods
        public InterceptLayer<TArg, TResult> SetArgDecoration(Func<TArg, TArg> strategy)
        {
            if (strategy != null)
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
            if (strategy != null)
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
            if (action != null)
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
            if (strategy != null)
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
            if (strategy != null)
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


    }
}
