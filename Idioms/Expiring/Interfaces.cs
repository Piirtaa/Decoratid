using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Expiring
{
    /// <summary>
    /// has a window that it can be in/out of
    /// </summary>
    public interface IWindowable 
    {
        bool IsInWindow(DateTime dt);
    }

    /// <summary>
    /// something that expires
    /// </summary>
    public interface IExpirable
    {
        bool IsExpired();
    }


    /// <summary>
    /// has a last touched date 
    /// </summary>
    public interface IHasLastTouched
    {
        DateTime LastTouchedDate { get; }
    }

    /// <summary>
    /// decorate items that can be touched with this interface
    /// </summary>
    public interface ICanTouch : IHasLastTouched
    {
        /// <summary>
        /// updates the last touched date
        /// </summary>
        void Touch();
    }

    //public static class ExpiryExtensions
    //{

    //    public static bool IsExpired(this IExpirable obj)
    //    {
    //        return obj.ExpiryDate < DateTime.UtcNow;
    //    }
    //}
}
