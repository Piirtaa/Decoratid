using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Contextual
{
    /// <summary>
    /// wraps something into a context of itself. Not really sure why I have this class - I guess the general idea here was
    /// to container-ize/conform something to IHasContext, maybe so types like Polyface can attach contexts to themselves 
    /// with a known Idiom (this is actually probably a good idea if we think of Polyface as a 
    /// type definition transformed into a runtime class definition)
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public class AsContext<T> : IHasContext<T>
    {
        #region Ctor
        public AsContext(T obj)
        {
            this.Context = obj;
        }
        #endregion

        #region IHasContext
        public T Context { get; set; }
        object IHasContext.Context
        {
            get
            {
                return this.Context;
            }
            set
            {
                this.Context = (T)value;
            }
        }
        #endregion

        #region Fluent Static
        public static AsContext<T> New(T obj)
        {
            return new AsContext<T>(obj);
        }
        #endregion
    }

    public static class AsContextExtensions
    {
        public static AsContext<T> BuildAsContext<T>(this T context)
        {
            if (context == null)
                throw new ArgumentNullException();

            return AsContext<T>.New(context);
        }
    }
}
