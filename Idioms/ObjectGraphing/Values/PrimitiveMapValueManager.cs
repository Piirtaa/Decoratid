using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.ObjectGraphing.Values.Decorations;
using Decoratid.Idioms.Stringing;
using Decoratid.Utils;
using System;
using System.Collections.Generic;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// allows strategies to do the job for the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PrimitiveMapValueManager<T> : INodeValueManager
    {
        public const string ID = "PrimitiveMap_{0}";

        #region Ctor
        public PrimitiveMapValueManager(Func<T, string> dehydrateStrategy, Func<string, T> hydrateStrategy)
        {
            Condition.Requires(dehydrateStrategy).IsNotNull();
            Condition.Requires(hydrateStrategy).IsNotNull();
            this.DehydrateStrategy = dehydrateStrategy;
            this.HydrateStrategy = hydrateStrategy;
        }
        #endregion

        #region IHasId
        public string Id { get { return string.Format(ID, typeof(T).Name); } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public Func<T, string> DehydrateStrategy { get; private set; }
        public Func<string, T> HydrateStrategy { get; private set; }
        #endregion


        #region INodeValueManager
        public virtual void RewriteNodePath(GraphPath path, object obj)
        {
            GraphingUtil.RewriteBackingFieldNodePath(path);
        }
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return null;
        }
        public virtual bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return obj is T;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            Condition.Requires(obj).IsNotNull();
            var data = this.DehydrateStrategy((T)obj);
            return LengthEncoder.LengthEncode(data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var dat = LengthEncoder.LengthDecode(nodeText);
            T obj = this.HydrateStrategy(dat);
            return obj;
        }
        #endregion

        #region Fluent Static
        public static PrimitiveMapValueManager<T> New(Func<T, string> dehydrateStrategy, Func<string, T> hydrateStrategy)
        {
            return new PrimitiveMapValueManager<T>(dehydrateStrategy, hydrateStrategy);
        }
        #endregion
    }

}
