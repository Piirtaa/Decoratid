using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.ObjectGraphing.Path;
using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// serialization interface if a type knows the value manager that can serialize this type 
    /// </summary>
    public interface IManagedHydrateable
    {
        string GetValueManagerId();
    }

    /// <summary>
    /// Handles instances that are IManagedHydrateable.  In other words, it handles types that know who its 
    /// value manager is.  Note: the provided graph must have a reference to the indicated ValueManager,
    /// or shit will break, yo.  
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
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return null;
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            if (obj == null)
                return false;

            //if it's not managedhydrateable skip immediately
            if (!(obj is IManagedHydrateable))
                return false;

            //do we have the indicated value manager?  if not, we'll skip and let the chain decide
            IManagedHydrateable hyd = obj as IManagedHydrateable;
            var mgrId = hyd.GetValueManagerId();
            var mgr = uow.ChainOfResponsibility.GetValueManagerById(mgrId);
            if (mgr == null)
                return false;

            return true;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            if (!(obj is IManagedHydrateable))
                return null;

            //do we have the indicated value manager?  if not, we'll skip and let the chain decide
            IManagedHydrateable hyd = obj as IManagedHydrateable;
            var mgrId = hyd.GetValueManagerId();
            var mgr = uow.ChainOfResponsibility.GetValueManagerById(mgrId);
            if (mgr == null)
                return null;

            var val =  mgr.DehydrateValue(obj, uow);

            return LengthEncoder.LengthEncodeList(mgrId, val);
        }
        public object HydrateValue(string nodeText, IGraph uow)
        {
            var list = LengthEncoder.LengthDecodeList(nodeText);
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
