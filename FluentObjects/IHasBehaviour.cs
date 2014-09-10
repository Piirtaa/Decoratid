using System;
using System.Collections.Generic;

namespace Sandbox.FluentObjects
{
    /// <summary>
    /// signature for a "behaviour" method
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="context"></param>
    /// <param name="args"></param>
    public delegate object BehaviourDelegate<TContext>(TContext context, params object[] args);

    public interface IHasBehaviour<TContext> : IHasContext<TContext>
    {
        Dictionary<string, BehaviourDelegate<TContext>> Behaviours { get; }
    }

    public static class BehaviourExtensions
    {
        /// <summary>
        /// invokes a behaviour
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="fluentObj"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object InvokeBehaviour<TContext>(this IHasBehaviour<TContext> fluentObj, string name, params object[] args)
        {
            if (fluentObj == null)
                return null;

            object returnValue = null;

            BehaviourDelegate<TContext> beh = null;

            if (fluentObj.Behaviours.TryGetValue(name, out beh))
            {
                returnValue = beh.Invoke(fluentObj.Context, args);
            }

            return returnValue;
        }


        /// <summary>
        /// Sets behaviour. is fluent
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="fluentObj"></param>
        /// <param name="name"></param>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public static IHasBehaviour<TContext> SetBehaviour<TContext>(this IHasBehaviour<TContext> fluentObj, string name,
            Delegate behaviour)
        {
            if (fluentObj == null)
                return null;

            fluentObj.Behaviours[name] = behaviour;

            return fluentObj;
        }

    }
}
