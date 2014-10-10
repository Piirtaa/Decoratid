using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Stringing;
using System.Linq;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// This manager does not itself know how to handle values, but it will take an educated guess, dammit!
    /// It delegates to Graph's ManagerSet to determine which manager is appropriate, and wraps it.  
    /// One would use this to serialize things they suspect might be decorated, or when the thing is 
    /// ambiguous.
    /// </summary>
    public sealed class UndeclaredValueManager : INodeValueManager
    {
        public const string ID = "Undeclared";

        #region Ctor
        public UndeclaredValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public bool CanHandle(object obj, IGraph uow)
        {
            var mgr = uow.ChainOfResponsibility.FindHandlingValueManager(obj, uow, UndeclaredValueManager.ID);
            return mgr != null;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            var mgr = uow.ChainOfResponsibility.FindHandlingValueManager(obj, uow, UndeclaredValueManager.ID);

            if (mgr == null)
                return null;

            var data = mgr.DehydrateValue(obj, uow);
            return LengthEncoder.LengthEncodeList(mgr.Id, data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            if (string.IsNullOrEmpty(nodeText))
                return null;

            var list = LengthEncoder.LengthDecodeList(nodeText);
            Condition.Requires(list).HasLength(2);

            var mgr = uow.ChainOfResponsibility.GetValueManagerById(list.ElementAt(0));

            if (mgr == null)
                return null;

            //if the chain of responsibility produces This as manager, we're in an infinite loop situation and should back out
            if (mgr != null && mgr is UndeclaredValueManager)
                return null;

            var obj = mgr.HydrateValue(list.ElementAt(1), uow);
            return obj;
        }
        #endregion

    }
}
