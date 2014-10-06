using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Idioms.Depending;
using System;

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
            if (argDecorator != null)
                this.ArgDecorator = argDecorator.MakeLogicOfTo();
            if (argValidator != null)
                this.ArgValidator = argValidator.MakeLogicOf();
            if (action != null)
                this.Action = action.MakeLogicOfTo();
            if (resultDecorator != null)
                this.ResultDecorator = resultDecorator.MakeLogicOfTo();
            if (resultValidator != null)
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


    }
}
