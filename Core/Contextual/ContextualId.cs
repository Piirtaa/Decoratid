using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Contextual
{
    /// <summary>
    /// wraps something into an id of itself with some contextual object.  pretty much a generic container
    /// where you supply the id and data.  As with all containers, one must know the exact type of the
    /// container to retrieve it with a get.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public class ContextualId<TId, TContext> : IHasId<TId>, IHasContext<TContext>
    {
        #region Ctor
        public ContextualId(TId obj, TContext context)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            this.Id = obj;
            this.Context = context;

        }
        #endregion

        #region IHasContext
        public TContext Context { get; set; }
        object IHasContext.Context
        {
            get
            {
                return this.Context;
            }
            set
            {
                this.Context = (TContext)value;
            }
        }
        #endregion

        #region Calculated Properties
        public TId Id { get; set; }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Fluent Static
        public static ContextualId<TId, TContext> New(TId obj, TContext context)
        {
            return new ContextualId<TId, TContext>(obj, context);
        }

        #endregion
    }

    public static class ContextualAsIdExtensions
    {
        public static ContextualId<TId, TContext> BuildContextualId<TId, TContext>(this TId thing, TContext context)
        {
            if (thing == null)
                throw new ArgumentNullException();

            return ContextualId<TId, TContext>.New(thing, context);
        }
    }
}
