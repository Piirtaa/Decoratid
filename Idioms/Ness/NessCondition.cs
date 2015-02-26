using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Ness
{
    public class NessCondition<Tness> : INessCondition<Tness>
    {
        #region Ctor
        public NessCondition(string id, IConditionOf<Arg<Tness>> condition)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            Condition.Requires(condition).IsNotNull();
            this.Id = id;
            this.Conditional = condition;
        }
        #endregion

        #region Fluent Static
        public static NessCondition<Tness> New(string id, IConditionOf<Arg<Tness>> condition)
        {
            return new NessCondition<Tness>(id, condition);
        }
        #endregion

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public IConditionOf<Arg<Tness>> Conditional { get; private set; }
        #endregion

        #region INessOperation
        public bool? IsConditionTrue(object thing, params object[] args)
        {
            //build the Arg up
            Tness ness = (Tness)thing;
            var arg = Arg<Tness>.New(ness);

            args.WithEach(x =>
            {
                arg.AddValue(x);
            });

            //perform
            var rv = this.Conditional.Evaluate(arg);
            return rv;
        }
        #endregion
    }
}
