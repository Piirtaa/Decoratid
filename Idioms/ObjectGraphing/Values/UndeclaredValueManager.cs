using CuttingEdge.Conditions;
using Decoratid.Serialization;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing.Decorations.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.Stringing;
using Decoratid.Idioms.Stringing.Decorations;
using Decoratid.Idioms.Hydrating;

namespace Decoratid.Idioms.ObjectGraph.Values
{
    //TODO: revisit the chain of responsibility design, and tighten it up so it's more self describing

    /// <summary>
    /// This manager does not itself know how to handle values, but it will take an educated guess, dammit!
    /// It delegates to Graph's ManagerSet to determine which manager is appropriate, and wraps it.  
    /// One would use this to serialize stores they suspect might be decorated, or when the thing is 
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
            var mgr = uow.ChainOfResponsibility.FindHandlingValueManager(obj, uow);
            return mgr != null;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            var mgr = uow.ChainOfResponsibility.FindHandlingValueManager(obj, uow);
            if (mgr == null)
                return null;

            var data = mgr.DehydrateValue(obj, uow);
            return TextDecorator.LengthEncodeList(mgr.Id, data);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            if (string.IsNullOrEmpty(nodeText))
                return null;

            var list = TextDecorator.LengthDecodeList(nodeText);
            Condition.Requires(list).HasLength(2);

            var mgr = uow.ChainOfResponsibility.GetValueManagerById(list.ElementAt(0));
            var obj = mgr.HydrateValue(list.ElementAt(1), uow);
            return obj;
        }
        #endregion

    }
}
