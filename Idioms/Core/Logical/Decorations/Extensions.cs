using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Logical.Decorations;
using Decoratid.Idioms.Core.ValueOfing;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Core.Logical.Decorations
{
    public static class Extensions
    {

        #region Error Catching
        //public static ErrorCatchingDecoration DecorateWithErrorCatching(this ILogic logic, ILogger logger = null)
        //{
        //    Condition.Requires(logic).IsNotNull();
        //    if (logic is ErrorCatchingDecoration)
        //    {
        //        return (ErrorCatchingDecoration)logic;
        //    }
        //    return new ErrorCatchingDecoration(logic, logger);
        //}
        //public static ErrorCatchingDecoration DecorateWithErrorCatchingDefaultLogger(this ILogic logic)
        //{
        //    Condition.Requires(logic).IsNotNull();
        //    if (logic is ErrorCatchingDecoration)
        //    {
        //        return (ErrorCatchingDecoration)logic;
        //    }
        //    return new ErrorCatchingDecoration(logic, LoggingManager.Instance.GetLogger());
        //}
        #endregion

    }
}
