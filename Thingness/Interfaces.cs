using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness
{
    /// <summary>
    /// has a string Name property.  
    /// </summary>
    /// <remarks>
    /// Seems like a silly interface but lets me build some easily testable mocks
    /// </remarks>
    public interface IHasName
    {
        string Name { get; }
    }

    public interface IHasDateCreated
    {
        DateTime DateCreated { get; }
    }
    /// <summary>
    /// interface providing an Expiry 
    /// </summary>
    public interface IHasExpiry
    {
        DateTime ExpiryDate { get; }
    }

    /// <summary>
    /// interface providing a last touched date 
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

    public interface IHasContext
    {
        object Context { get; set; }
    }
    /// <summary>
    /// defines something that operates on a generic type for context/state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasContext<T>: IHasContext
    {
        new T Context { get; set; }
    }

    /// <summary>
    /// defines something that has a context (eg. IHasContext) and also has a factory to produce the context value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasContextFactory<T> : IHasContext<T>
    {
        void BuildContext();
    }

}
