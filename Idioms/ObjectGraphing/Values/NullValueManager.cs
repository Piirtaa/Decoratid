
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using System;
using System.Collections.Generic;
namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// Is always the FIRST manager in the Chain of Responsibility.  Handles null values.
    /// </summary>
    public sealed class NullValueManager : INodeValueManager
    {
        public const string ID = "Null";

        #region Ctor
        public NullValueManager() { }
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
            return obj == null;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            return null;
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            return null;
        }
        #endregion
    }
}
