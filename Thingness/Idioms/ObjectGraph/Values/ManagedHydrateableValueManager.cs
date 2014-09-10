using CuttingEdge.Conditions;
using Decoratid.Serialization;
using Decoratid.Thingness.Idioms.Store;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Stringable;
using Decoratid.Thingness.Idioms.Stringable.Decorations;
using Decoratid.Thingness.Idioms.Hydrateable;

namespace Decoratid.Thingness.Idioms.ObjectGraph.Values
{
    /// <summary>
    /// serialization interface if a type knows the value manager that can serialize this type 
    /// </summary>
    public interface IManagedHydrateable
    {
        string GetValueManagerId();
    }

    /// <summary>
    /// Handles instances that are IManagedHydrateable.  Note that the graph that calls this must have a reference to the 
    /// manager the IManagedHydrateable refers to in its set, or shit will break, yo.  
    /// </summary>
    public sealed class ManagedHydrateableValueManager : INodeValueManager
    {
        public const string ID = "ManagedHydrateable";

        #region Ctor
        public ManagedHydrateableValueManager() { }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region INodeValueManager
        public bool CanHandle(object obj, IGraph uow)
        {
            return obj is IManagedHydrateable;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            IManagedHydrateable hyd = obj as IManagedHydrateable;
            var mgrId = hyd.GetValueManagerId();
            var mgr = uow.ChainOfResponsibility.GetValueManagerById(mgrId);
            var val =  mgr.DehydrateValue(obj, uow);

            return TextDecorator.LengthEncodeList(mgrId, val);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list = TextDecorator.LengthDecodeList(nodeText);
            Condition.Requires(list).HasLength(2);

            //this is where we examine our context to see if we have the value manager required , if not we return null
            var mgr  = uow.ChainOfResponsibility.GetValueManagerById(list[0]);
            if (mgr != null)
            {
                var rv = mgr.HydrateValue(list[1], uow);
                return rv;
            }
            return null;
        }
        #endregion

    }
}
