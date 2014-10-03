
namespace Decoratid.Idioms.ObjectGraphing.Values
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
