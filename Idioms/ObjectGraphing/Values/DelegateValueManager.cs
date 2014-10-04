using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// handles delegates only
    /// </summary>
    public sealed class DelegateValueManager : BinarySerializingValueManager
    {
        public const string ID = "Delegate";

        #region Ctor
        public DelegateValueManager() { }
        #endregion

        #region IHasId
        public override string Id { get { return ID; } }
        #endregion

        #region INodeValueManager
        public override bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return obj is Delegate;
        }
        #endregion

    }
}
