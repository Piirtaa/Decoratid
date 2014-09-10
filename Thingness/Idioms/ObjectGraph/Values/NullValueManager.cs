using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Stringable;
using Decoratid.Thingness.Idioms.Stringable.Decorations;

namespace Decoratid.Thingness.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// Is always the FIRST manager in the Chain of Responsibility
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
