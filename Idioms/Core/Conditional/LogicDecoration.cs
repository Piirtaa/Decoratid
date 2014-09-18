using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Core.Conditional.Decorations
{

    /// <summary>
    /// decorates as ICondition by Performing some logic when the condition is true
    [Serializable]
    public sealed class LogicDecoration : DecoratedConditionBase
    {
        #region Ctor
        public LogicDecoration(ICondition decorated, ILogic logic)
            : base(decorated)
        {
            Condition.Requires(logic).IsNotNull();
            this.Logic = logic;
        }
        #endregion

        #region Fluent Static
        public static LogicDecoration New(ICondition decorated, ILogic logic)
        {
            return new LogicDecoration(decorated, logic);
        }
        #endregion

        #region ISerializable
        protected LogicDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Logic = (ILogic)info.GetValue("Logic", typeof(ILogic));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Logic", this.Logic);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        protected 
        public ILogic Logic { get; set; }
        #endregion

    }

    public static class LogicDecorationExtensions
    {
        /// <summary>
        /// Converts a ConditionOf into an ICondition by providing an argument valueOf 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condOf"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ICondition DefineCondition<T>(this IConditionOf<T> condOf, IValueOf<T> val)
        {
            return ContextualDecoration<T>.New(condOf, val);
        }
    }
}
