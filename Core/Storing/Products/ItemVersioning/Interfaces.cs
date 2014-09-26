using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Store.ConcurrencyVersioning
{

    ///// <summary>
    ///// extends IStoreOf to support objects that have IIncrementalVersion info. 
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public interface IStoreOfIncrementallyVersionedOf<T> : IStoreOf<T>
    //            where T : IHasId
    //{
    //    IIncrementalVersion GetVersion(T item);
    //    bool IsLatestVersion(T item);
    //    void Touch(T item);
    //}

    public interface IHasIncrementalVersion
    {
        IIncrementalVersion GetIncrementalVersion();
    }

    /// <summary>
    /// IIncrementalVersion is an incrementable, comparable type, that is used to contain a version number used specifically for
    /// concurrency.  Every time a versioned thing is touched it gets incremented.  One can sort off this "number". 
    /// </summary>
    public interface IIncrementalVersion : IComparable, ISerializable
    {
        void Increment();
        string GetVersionText();
    }

    /// <summary>
    /// A generic IIncrementalVersion 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IIncrementalVersionOf<T> : IIncrementalVersion, IComparer<T>, IComparable<T>, IEquatable<T> 
    {

    }
}
