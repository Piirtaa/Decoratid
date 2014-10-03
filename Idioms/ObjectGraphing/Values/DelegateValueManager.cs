using System;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// handles delegates only
    /// </summary>
    public sealed class DelegateValueManager : INodeValueManager
    {
        public const string ID = "Delegate";

        #region Ctor
        public DelegateValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public bool CanHandle(object obj, IGraph uow)
        {
            return obj is Delegate;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            return SerializationManager.Instance.Serialize(BinarySerializationUtil.ID, obj);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            return SerializationManager.Instance.Deserialize(nodeText);
        }
        #endregion

    }
}
