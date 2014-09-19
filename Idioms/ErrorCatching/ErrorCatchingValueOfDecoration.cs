using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core;
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.ErrorCatching
{
    /// <summary>
    /// Traps all errors that are thrown from the logic
    /// </summary>
    public class ErrorCatchingValueOfDecoration<T> : DecoratedValueOfBase<T>
    {
        #region Ctor
        public ErrorCatchingValueOfDecoration(IValueOf<T> decorated)
            : base(decorated)
        {
        }
        #endregion

        #region Methods
        public override T GetValue()
        {
            try
            {
                return Decorated.GetValue();
            }
            catch
            {
            }
            return default(T);
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            return new ErrorCatchingValueOfDecoration(thing);
        }
        #endregion
    }
}
