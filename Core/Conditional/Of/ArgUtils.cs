using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Conditional.Of
{
    /// <summary>
    /// helper methods to build Conditional instances that use Arg semantics
    /// </summary>
    public static class ArgUtils
    {
        public static IConditionOf<Arg<TOf>> New<TOf>(this Func<TOf, bool?> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new StrategizedConditionOf<Arg<TOf>>(
                (x) =>
                {
                    return function(x.As<TOf>());
                });
            return rv;
        }
        public static IConditionOf<Arg<TOf>> New<TOf, TArg1>(this Func<TOf, TArg1, bool?> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new StrategizedConditionOf<Arg<TOf>>(
                (x) =>
                {
                    return function(x.As<TOf>(),
                        x.As<TArg1>()
                        );
                });
            return rv;
        }
        public static IConditionOf<Arg<TOf>> New<TOf, TArg1, TArg2>(this Func<TOf, TArg1, TArg2, bool?> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new StrategizedConditionOf<Arg<TOf>>(
                (x) =>
                {
                    return function(x.As<TOf>(),
                        x.As<TArg1>(),
                        x.As<TArg2>()
                        );
                });
            return rv;
        }
        public static IConditionOf<Arg<TOf>> New<TOf, TArg1, TArg2, TArg3>(this Func<TOf, TArg1, TArg2, TArg3, bool?> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new StrategizedConditionOf<Arg<TOf>>(
                (x) =>
                {
                    return function(x.As<TOf>(),
                        x.As<TArg1>(),
                        x.As<TArg2>(),
                        x.As<TArg3>()
                        );
                });
            return rv;
        }
        public static IConditionOf<Arg<TOf>> New<TOf, TArg1, TArg2, TArg3, TArg4>(this Func<TOf, TArg1, TArg2, TArg3, TArg4, bool?> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new StrategizedConditionOf<Arg<TOf>>(
                (x) =>
                {
                    return function(x.As<TOf>(),
                        x.As<TArg1>(),
                        x.As<TArg2>(),
                        x.As<TArg3>(),
                        x.As<TArg4>()
                        );
                });
            return rv;
        }
        public static IConditionOf<Arg<TOf>> New<TOf, TArg1, TArg2, TArg3, TArg4, TArg5>(this Func<TOf, TArg1, TArg2, TArg3, TArg4, TArg5, bool?> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new StrategizedConditionOf<Arg<TOf>>(
                (x) =>
                {
                    return function(x.As<TOf>(),
                        x.As<TArg1>(),
                        x.As<TArg2>(),
                        x.As<TArg3>(),
                        x.As<TArg4>(),
                        x.As<TArg5>()
                        );
                });
            return rv;
        }
        public static IConditionOf<Arg<TOf>> New<TOf, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(this Func<TOf, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, bool?> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new StrategizedConditionOf<Arg<TOf>>(
                (x) =>
                {
                    return function(x.As<TOf>(),
                        x.As<TArg1>(),
                        x.As<TArg2>(),
                        x.As<TArg3>(),
                        x.As<TArg4>(),
                        x.As<TArg5>(),
                        x.As<TArg6>()
                        );
                });
            return rv;
        }
    }
}
