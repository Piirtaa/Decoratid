using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid
{
    ///// <summary>
    ///// A class that allows for chainable/fluent actions
    ///// </summary>
    //public class FluentBool
    //{
    //    #region Declarations
    //    private readonly bool _value;
    //    #endregion

    //    #region Ctor
    //    public FluentBool(bool value)
    //    {
    //        this._value = value;
    //    }
    //    #endregion

    //    #region Properties
    //    public bool Value { get { return _value; } }
    //    #endregion

    //    #region Implicit Cast
    //    public static implicit operator bool(FluentBool o)
    //    {
    //        if (o == null) { return false; }
    //        return o.Value;
    //    }
    //    #endregion

    //    #region Fluent Do Calls
    //    public FluentBool Do(Action action)
    //    {
    //        bool returnValue = false;

    //        if (action != null)
    //        {
    //            try
    //            {
    //                action();
    //                returnValue = true;
    //            }
    //            catch { }
    //        }
    //        return new FluentBool(returnValue);
    //    }
    //    public FluentBool Do(Func<bool> func)
    //    {
    //        bool returnValue = false;

    //        if (func != null)
    //        {
    //            try
    //            {
    //                returnValue = func();
    //            }
    //            catch { }
    //        }
    //        return new FluentBool(returnValue);
    //    }
    //    #endregion
    //}

    public static class FluentBoolExtensions
    {
        /// <summary>
        /// A fluent extension to allow an action to be performed if the bool is true.  Avoids nesting ifs, so a simple sequential
        /// workflow can be chained together in one line.  CATCHES EXCEPTIONS!!
        /// </summary>
        public static bool CatchDoAction(this bool o, Action action)
        {
            bool returnValue = false;

            //skip out, cos there's nothing to do
            if (action == null) return false;
            //skip out, cos can't chain
            if (!o) return false;

            //do the call
            try
            {
                action();
                returnValue = true;
            }
            catch { }
            return returnValue;
        }

        /// <summary>
        /// A fluent extension to allow an action to be performed if the bool is true.  Avoids nesting ifs, so a simple sequential
        /// workflow can be chained together in one line.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool DoAction(this bool o, Action action)
        {
            bool returnValue = false;

            //skip out, cos there's nothing to do
            if (action == null) return false;
            //skip out, cos can't chain
            if (!o) return false;

            //do the call
            action();
            returnValue = true;
            
            return returnValue;
        }
        /// <summary>
        /// A fluent extension to allow an action to be performed if the bool is true.  Avoids nesting ifs, so a simple sequential
        /// workflow can be chained together in one line.  TRAPS EXCEPTIONS!!!
        /// </summary>
        public static bool CatchDoFunction(this bool o, Func<bool> func)
        {
            bool returnValue = false;

            //skip out, cos there's nothing to do
            if (func == null) return false;
            //skip out, cos can't chain
            if (!o) return false;

            try
            {
                //do the call
                returnValue = func();
            }
            catch { }

            return returnValue;
        }
        /// <summary>
        /// A fluent extension to allow an action to be performed if the bool is true.  Avoids nesting ifs, so a simple sequential
        /// workflow can be chained together in one line. 
        /// </summary>
        public static bool DoFunction(this bool o, Func<bool> func)
        {

            //skip out, cos there's nothing to do
            if (func == null) return false;
            //skip out, cos can't chain
            if (!o) return false;

            //do the call
            return func();
        }
    }
}
