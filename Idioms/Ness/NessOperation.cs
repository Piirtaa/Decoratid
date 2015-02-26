using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Ness
{
    public class NessOperation<Tness, Tres> : INessOperation<Tness, Tres>
    {
        #region Ctor
        public NessOperation(string id, LogicOfTo<Arg<Tness>, Tres> logic)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            Condition.Requires(logic).IsNotNull();
            this.Id = id;
            this.Logic = logic;
        }
        #endregion

        #region Fluent Static
        public static NessOperation<Tness, Tres> New(string id, LogicOfTo<Arg<Tness>, Tres> logic)
        {
            return new NessOperation<Tness, Tres>(id, logic);
        }
        #endregion

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public LogicOfTo<Arg<Tness>, Tres> Logic { get; private set; }
        #endregion

        #region INessOperation
        public object PerformOperation(object thing, params object[] args)
        {
            //build the Arg up
            Tness ness = (Tness)thing;
            var arg = Arg<Tness>.New(ness);

            args.WithEach(x =>
            {
                arg.AddValue(x);
            });

            //perform
            LogicOfTo<Arg<Tness>, Tres> res = this.Logic.Perform(arg) as LogicOfTo<Arg<Tness>, Tres>;
            return res.Result;
        }
        #endregion
    }
}
