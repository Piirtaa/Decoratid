using CuttingEdge.Conditions;
using Decoratid.Core.Conditional.Of;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core.Decorating;

namespace Decoratid.Idioms.Ness
{
    public class NessDefinition<T>: INess
    {
        #region Ctor
        public NessDefinition(string name)
        {
            
            Condition.Requires(name).IsNotNullOrEmpty();
            this.Name = name;
            this.DecoratingType = typeof(T);
            Type decOfType = typeof(IDecorationOf<>);
            Condition.Requires(this.DecoratingType.HasGenericDefinition(decOfType)).IsTrue("not a decoration of");
            this.DecoratedType = this.DecoratingType.GetGenericParameterType(decOfType);

            this.OperationsMap = new Dictionary<string, Func<object, object>>();
            this.ConditionalsMap = new Dictionary<string, IConditionOf<object>>();
            
        }
        #endregion

        #region Fluent Static
        public static NessDefinition<T> New(string name)
        {
            return new NessDefinition<T>(name);
        }
        #endregion

        #region Properties
        public Type DecoratedType { get; private set; }
        public Type DecoratingType { get; private set; }
        public string Name { get; private set; }

        public Dictionary<string, Func<object, object>> OperationsMap { get; private set; }
        public Dictionary<string, IConditionOf<object>> ConditionalsMap { get; private set; }
        public Func<object[], object> Constructor { get; private set; }
        #endregion

        #region Methods
        public NessDefinition<T> SetOp(string name, Func<T, object> op)
        {
            Condition.Requires(name).IsNotNullOrEmpty();
            Condition.Requires(
        }
        #endregion
    }
}
