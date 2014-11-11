using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// only handles value types.  uses binary serializer
    /// </summary>
    public sealed class ValueTypeValueManager : BinarySerializingValueManager
    {
        public const string ID = "Value";

        #region Ctor
        public ValueTypeValueManager() { }
        #endregion

        #region IHasId
        public override string Id { get { return ID; } }
        #endregion

        #region INodeValueManager
        public void RewriteNodePath(GraphPath path, object obj)
        {
            GraphingUtil.RewriteBackingFieldNodePath(path);
        }
        public override bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return obj.GetType().IsValueType;
        }
        #endregion

    }
}
