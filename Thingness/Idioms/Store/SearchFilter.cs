using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Decoratid.Thingness.Idioms.Store
{
    /// <summary>
    /// A wrapper class around Func IHasId,bool.  Facilitates decoration/extension.
    /// </summary>
    [Serializable]
    public class SearchFilter 
    {
        #region Ctor
        public SearchFilter() { }
        public SearchFilter(Func<IHasId, bool> filter)
        {
            Condition.Requires(filter).IsNotNull();
            this.Filter = filter;
        }
        #endregion

        #region Properties
        public Func<IHasId, bool> Filter { get; set; }
        #endregion

        #region Conversions
        public static implicit operator SearchFilter(Func<IHasId, bool> filter)
        {
            return new SearchFilter(filter);
        }
        public static implicit operator Func<IHasId, bool>(SearchFilter filter)
        {
            if (filter == null) { return null; }
            return filter.Filter;
        }
        #endregion

        #region Fluent Static
        public static SearchFilter New(Func<IHasId, bool> filter)
        {
            return new SearchFilter(filter);
        }
        #endregion
    }

    /// <summary>
    /// a generic version of SearchFilter that operates on a specific type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SearchFilterOf<T> : SearchFilter where T : IHasId
    {
        #region Ctor
        public SearchFilterOf()
            : base()
        {
        }
        public SearchFilterOf(Func<T, bool> filter): base()
        {
            //converts the generic expression to a non-generic one
            Condition.Requires(filter).IsNotNull();
            this.GenericFilter = filter;

            this.Filter = (x) =>
            {
                if(x == null)
                    return false;

                Type returnType = typeof(T);
                if(!returnType.IsAssignableFrom(x.GetType()))
                    return false;

                T tx = (T)x;
                return filter(tx);
            };
        }
        #endregion

        #region Properties
        public Func<T, bool> GenericFilter { get; set; }
        #endregion

        #region Conversions
        public static implicit operator SearchFilterOf<T>(Func<T, bool> filter)
        {
            return new SearchFilterOf<T>(filter);
        }
        public static implicit operator Func<T, bool>(SearchFilterOf<T> filter)
        {
            if (filter == null) { return null; }
            return filter.GenericFilter;
        }
        #endregion

        #region Fluent Static
        public static SearchFilterOf<T> NewOf(Func<T, bool> filter) 
        {
            return new SearchFilterOf<T>(filter);
        }
        #endregion
    }
}
