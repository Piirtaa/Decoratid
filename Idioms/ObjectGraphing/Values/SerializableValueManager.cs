using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Extensions;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// handles ISerializable and Serializable attributed types
    /// </summary>
    public sealed class SerializableValueManager : BinarySerializingValueManager
    {
        public const string ID = "Serializable";

        #region Ctor
        public SerializableValueManager() { }
        #endregion

        #region IHasId
        public override string Id { get { return ID; } }
        #endregion

        #region INodeValueManager
        public override bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            return obj.IsMarkedSerializable();
        }
        #endregion

    }
}
