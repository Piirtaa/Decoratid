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
    /// handles dates only
    /// </summary>
    public sealed class DateValueManager : INodeValueManager
    {
        public const string ID = "Date";

        #region Ctor
        public DateValueManager() { }
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

            return obj is DateTime;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            Condition.Requires(obj).IsNotNull();
            var data = ((DateTime)obj).ToString();
            return LengthEncoder.LengthEncode(data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var data = LengthEncoder.LengthDecode(nodeText);
            var dt = DateTime.Parse(data);
            return dt;
        }
        #endregion

    }
}
