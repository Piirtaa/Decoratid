using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Logical
{
    /// <summary>
    /// helper methods to build Logic instances that use Arg semantics
    /// </summary>
    public static class ArgUtils
    {
        
        public static LogicOfTo<Arg<TOf>, TTo> New<TOf, TTo>(this Func<TOf, TTo> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new LogicOfTo<Arg<TOf>, TTo>(
                (x) =>
                {
                    return function(x.GetFace<TOf>());
                });
            return rv;
        }
        public static LogicOfTo<Arg<TOf>, TTo> New<TOf, TArg1, TTo>(this Func<TOf, TArg1, TTo> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new LogicOfTo<Arg<TOf>, TTo>(
                (x) =>
                {
                    return function(x.As<TOf>(),
                        x.As<TArg1>()
                        );
                });
            return rv;
        }
        public static LogicOfTo<Arg<TOf>, TTo> New<TOf, TArg1, TArg2, TTo>(this Func<TOf, TArg1, TArg2, TTo> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new LogicOfTo<Arg<TOf>, TTo>(
                (x) =>
                {
                    return function(x.As<TOf>(),
                        x.As<TArg1>(),
                        x.As<TArg2>()
                        );
                });
            return rv;
        }
        public static LogicOfTo<Arg<TOf>, TTo> New<TOf, TArg1, TArg2, TArg3, TTo>(this Func<TOf, TArg1, TArg2, TArg3, TTo> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new LogicOfTo<Arg<TOf>, TTo>(
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
        public static LogicOfTo<Arg<TOf>, TTo> New<TOf, TArg1, TArg2, TArg3, TArg4, TTo>(this Func<TOf, TArg1, TArg2, TArg3, TArg4, TTo> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new LogicOfTo<Arg<TOf>, TTo>(
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
        public static LogicOfTo<Arg<TOf>, TTo> New<TOf, TArg1, TArg2, TArg3, TArg4, TArg5, TTo>(this Func<TOf, TArg1, TArg2, TArg3, TArg4, TArg5, TTo> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new LogicOfTo<Arg<TOf>, TTo>(
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
        public static LogicOfTo<Arg<TOf>, TTo> New<TOf, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TTo>(this Func<TOf, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TTo> function)
        {
            Condition.Requires(function).IsNotNull();
            var rv = new LogicOfTo<Arg<TOf>, TTo>(
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
