using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// handles guids only
    /// </summary>
    public sealed class GuidValueManager : INodeValueManager
    {
        public const string ID = "Guid";

        #region Ctor
        public GuidValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return null;
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return obj is Guid;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            Condition.Requires(obj).IsNotNull();
            var data = ((Guid)obj).ToString();
            return LengthEncoder.LengthEncode(data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var data = LengthEncoder.LengthDecode(nodeText);
            var dt = Guid.Parse(data);
            return dt;
        }
        #endregion

    }
}
