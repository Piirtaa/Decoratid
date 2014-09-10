using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness
{
    /// <summary>
    /// Generic event arg class.  Rather than build a fancy eventarg class, use generic containment
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgOf<T> : EventArgs
    {
        #region Declarations
        private T _value;
        #endregion

        #region Ctor
        public EventArgOf(T value)
        {
            this._value = value;
            
        }
        #endregion

        #region Properties
        public T Value
        {
            get { return _value; }
        }
        #endregion
    }

    /// <summary>
    /// Extension class that gives helper methods for EventArgOf-related classes
    /// </summary>
    public static class EventArgOfExtensions
    {
        /// <summary>
        /// Builds an EventArgOf instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EventArgOf<T> BuildEventArgs<T>(this EventHandler<EventArgOf<T>> handler, T data)
        {
            return new EventArgOf<T>(data);
        }

        public static void BuildAndFireEventArgs<T>(this EventHandler<EventArgOf<T>> handler, T data)
        {
            if (handler == null)
                return;

            var arg = new EventArgOf<T>(data);
            handler(null, arg);
        }
    }
}
