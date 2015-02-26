using CuttingEdge.Conditions;
using Decoratid.Core.Conditional.Of;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core.Decorating;
using Decoratid.Core;
using Decoratid.Core.Logical;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Stringing;
using Decoratid.Storidioms.StoreOf;
using Decoratid.Core.Storing;

namespace Decoratid.Idioms.Ness
{
    /// <summary>
    /// implements Ness
    /// </summary>
    /// <typeparam name="Tness"></typeparam>
    /// <typeparam name="Tdecorated"></typeparam>
    public class NessDefinition<Tness, Tdecorated> : ITypedNess<Tness, Tdecorated>
    {
        #region Ctor
        public NessDefinition(string name)
        {

            Condition.Requires(name).IsNotNullOrEmpty();
            this.Name = name;
            this.ConditionStore = NaturalInMemoryStore.New().IsOf<INessCondition>();
            this.OperationStore = NaturalInMemoryStore.New().IsOf<INessOperation>();
        }
        #endregion

        #region Fluent Static
        public static NessDefinition<Tness, Tdecorated> New(string name)
        {
            return new NessDefinition<Tness, Tdecorated>(name);
        }
        #endregion

        #region IHasId
        public string Id { get { return string.Join("|", this.Name, this.DecoratedType.Name, this.DecoratingType.Name); } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public Type DecoratedType { get { return typeof(Tdecorated); } }
        public Type DecoratingType { get { return typeof(Tness); } }
        public string Name { get; private set; }
        public LogicOfTo<Arg<Tdecorated>, Tness> DecoratorLogic { get; set; }
        private IStoreOf<INessOperation> OperationStore { get; set; }
        private IStoreOf<INessCondition> ConditionStore { get; set; }
        #endregion

        #region Registration
        private INessCondition GetCondition(string name)
        {
            var rv = this.ConditionStore.GetAllById<INessCondition>(name);
            if (rv != null && rv.Count > 0)
                return rv[0];

            return null;
        }
        private INessOperation GetOperation(string name)
        {
            var rv = this.OperationStore.GetAllById<INessOperation>(name);
            if (rv != null && rv.Count > 0)
                return rv[0];

            return null;
        }
        public NessDefinition<Tness, Tdecorated> AddCondition(string name, IConditionOf<Arg<Tness>> condition)
        {
            var item = NessCondition<Tness>.New(name, condition);
            this.ConditionStore.SaveItem(item);
            return this;
        }
        public NessDefinition<Tness, Tdecorated> AddOperation<Tres>(string name, LogicOfTo<Arg<Tness>, Tres> operation)
        {
            var item = NessOperation<Tness, Tres>.New(name, operation);
            this.OperationStore.SaveItem(item);
            return this;
        }
        #endregion

        #region Methods
        public object PerformOperation(string opName, object thing, params object[] args)
        {
            var op = this.GetOperation(opName);
            Condition.Requires(op).IsNotNull();
            return op.PerformOperation(thing, args);
        }

        public bool? IsConditionTrue(string conditionName, object thing, params object[] args)
        {
            var cond = this.GetCondition(conditionName);
            Condition.Requires(cond).IsNotNull();
            return cond.IsConditionTrue(thing, args);
        }

        public object DecorateWith(object thing, params object[] args)
        {
            Condition.Requires(this.DecoratorLogic).IsNotNull();
            //build the args
            var arg = Arg<Tdecorated>.New(thing);
            args.WithEach(x =>
            {
                arg.AddValue(x);
            });

            var result = this.DecoratorLogic.Perform(arg);
            var rv = result as ILogicTo<Tness>;
            if (rv != null)
                return rv.Result;

            return null;
        }
        #endregion






    }
}
