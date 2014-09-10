using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Extensions
{
    /// <summary>
    /// Extensions to wrap actions/functions
    /// </summary>
    public static class TryCatchExtensions
    {
        /// <summary>
        /// Traps exceptions and returns false if one is raised
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool TrapErrors(this Action action)
        {
            bool returnValue = false;

            try
            {
                action();
                returnValue = true;
            }
            catch
            {
                
            }

            return returnValue;
        }

        /// <summary>
        /// Traps exceptions and returns function return, made nullable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Nullable<T> TrapErrors<T>(this Func<T> func)
                        where T : struct
        {
            Nullable<T> returnValue = null;

            try
            {
                returnValue = func();
            }
            catch
            {
                returnValue = null;
            }

            return returnValue;
        }
        /// <summary>
        /// Traps exceptions and returns function return, null if exceptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T TrapErrors<T>(this Func<T> func)
            where T : class
        {
            T returnValue = null;

            try
            {
                returnValue = func();
            }
            catch
            {
                returnValue = null;
            }

            return returnValue;
        }
    }
}
