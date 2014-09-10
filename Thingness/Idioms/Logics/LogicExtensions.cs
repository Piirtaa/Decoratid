using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Logging;
using Decoratid.Thingness.Idioms.Logics.Decorations;
using Decoratid.Thingness.Idioms.ValuesOf;
using Decoratid.Extensions;

namespace Decoratid.Thingness.Idioms.Logics
{
    public static class LogicExtensions
    {
        #region from logic conversions
        public static Action ToAction(this Logic logic)
        {
            if (logic == null) { return null; }

            return () => { logic.CloneAndPerform(); };
        }
        public static Action<T> ToAction<T>(this LogicOf<T> logic)
        {
            if (logic == null) { return null; }

            return (x) => { logic.CloneAndPerform(x.ValueOf()); };
        }
        public static Func<T> ToFunc<T>(this LogicTo<T> logic)
        {
            if (logic == null) { return null; }

            return () => { return logic.CloneAndPerform(); };
        }
        public static Func<Targ, Tres> ToFunc<Targ, Tres>(this LogicOfTo<Targ, Tres> logic)
        {
            if (logic == null) { return null; }

            return (x) =>
            {
                Tres res = logic.CloneAndPerform(x.ValueOf());

                return res;
            };
        }
        #endregion

        #region to logic conversions
        public static Logic MakeLogic(this Action action)
        {
            Condition.Requires(action).IsNotNull();
            return new Logic(action);
        }
        public static LogicOf<T> MakeLogicOf<T>(this Action<T> action)
        {
            Condition.Requires(action).IsNotNull();
            return new LogicOf<T>(action);
        }
        public static LogicOf<T> MakeLogicOf<T>(this Action<T> action, IValueOf<T> context)
        {
            Condition.Requires(action).IsNotNull();
            Condition.Requires(context).IsNotNull();
            return new LogicOf<T>(action, context);
        }
        public static LogicTo<T> MakeLogicTo<T>(this Func<T> function)
        {
            Condition.Requires(function).IsNotNull();
            return new LogicTo<T>(function);
        }
        public static LogicOfTo<Targ, TRes> MakeLogicOfTo<Targ, TRes>(this Func<Targ, TRes> function)
        {
            Condition.Requires(function).IsNotNull();
            return new LogicOfTo<Targ, TRes>(function);
        }
        public static LogicOfTo<Targ, TRes> MakeLogicOfTo<Targ, TRes>(this Func<Targ, TRes> function, IValueOf<Targ> context)
        {
            Condition.Requires(function).IsNotNull();
            Condition.Requires(context).IsNotNull();
            return new LogicOfTo<Targ, TRes>(function, context);
        }
        #endregion

        #region Error Catching
        public static ErrorCatchingDecoration DecorateWithErrorCatching(this ILogic logic, ILogger logger = null)
        {
            Condition.Requires(logic).IsNotNull();
            if (logic is ErrorCatchingDecoration)
            {
                return (ErrorCatchingDecoration)logic;
            }
            return new ErrorCatchingDecoration(logic, logger);
        }
        public static ErrorCatchingDecoration DecorateWithErrorCatchingDefaultLogger(this ILogic logic)
        {
            Condition.Requires(logic).IsNotNull();
            if (logic is ErrorCatchingDecoration)
            {
                return (ErrorCatchingDecoration)logic;
            }
            return new ErrorCatchingDecoration(logic, LoggingManager.Instance.GetLogger());
        }
        #endregion

        #region Clone And Perform
        /// <summary>
        /// performs the logic.  Clones if ICloneableLogic prior to perform.  
        /// Sets context if it's ILogicOf, and if the context is supplied.  Returns result if it's ILogicTo, else Nothing.VOID.  
        /// </summary>
        /// <param name="logic"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static object SmartPerform(this ILogic logic, object context = null)
        {
            Condition.Requires(logic).IsNotNull();

            ILogic realLogic = logic;

            if (logic is ICloneableLogic)
                realLogic = ((ICloneableLogic)logic).Clone();

            //if we have context set it
            if (typeof(ILogicOf<>).IsInstanceOfGenericType(realLogic))
            {
                if (context != null)
                {
                    IHasContext hasContext = realLogic as IHasContext;
                    hasContext.Context = context;
                }
            }

            realLogic.Perform();

            //if we have returnvalue return it
            if (typeof(ILogicTo<>).IsInstanceOfGenericType(realLogic))
            {
                //return the result
                var pi = realLogic.GetType().GetProperty("Result", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                var rv = pi.GetValue(realLogic);

                return rv;
            }

            //else return void
            return Nothing.VOID;
        }
        #endregion
    }
}
