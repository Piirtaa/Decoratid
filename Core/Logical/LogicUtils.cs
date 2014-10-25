using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;

namespace Decoratid.Core.Logical
{
    public static class LogicUtils
    {
        public static bool IsOfLogic(this ILogic logic)
        {
            if (logic == null)
                return false;

            Type logicType = logic.GetType();
            return logicType.HasGenericDefinition(typeof(ILogicOf<>));
        }
        public static Type GetOfType(this ILogic logic)
        {
            if (!logic.IsOfLogic())
                return null;

            Type logicType = logic.GetType();
            foreach (Type intType in logicType.GetInterfaces())
            {
                if (intType.IsGenericType
                    && intType.GetGenericTypeDefinition() == typeof(ILogicOf<>))
                {
                    return intType.GetGenericArguments()[0];
                }
            }

            return null;
        }
    }
}
