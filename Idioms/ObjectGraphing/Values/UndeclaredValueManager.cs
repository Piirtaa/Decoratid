using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Idioms.Stringing;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using Decoratid.Idioms.ObjectGraphing.Path;

namespace Decoratid.Idioms.ObjectGraphing.Values
{
    /// <summary>
    /// This manager does not itself know how to handle values, but it will take an educated guess, dammit!
    /// It delegates to Graph's ManagerSet to determine which manager is appropriate.  We can provide hints in the form
    /// of manager id's to ignore.
    /// </summary>
    /// <remarks>
    /// Q. The graphing process determines which value manager to use for each node, so when would this class be used?
    /// A. It would be used in a situation where the graphing process has selected a value manager (not Undeclared), 
    /// and within that value manager's operation this class is called.  
    /// For example, if a HydrationMap exists, and that map specifies a mapping as being unknown.  
    /// It should NEVER be found in a chain of responsibility, and is validated against this misconfiguration.
    /// </remarks>
    public sealed class UndeclaredValueManager : INodeValueManager
    {
        public const string ID = "Undeclared";

        #region Ctor
        public UndeclaredValueManager(params string[] managerIdsToIgnore)
        {
            this.ManagerIdsToIgnore = managerIdsToIgnore;
        }
        #endregion

        #region IHasId
        public string Id { get { return ID; } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public string[] ManagerIdsToIgnore { get; private set; }
        #endregion

        //#region Methods
        //public INodeValueManager FindHandlingValueManager(object obj, IGraph uow)
        //{
        //    List<string> ignoreMgrIds = new List<string>();
        //    if (this.ManagerIdsToIgnore != null)
        //        ignoreMgrIds.AddRange(this.ManagerIdsToIgnore);
        //    if(!ignoreMgrIds.Contains(UndeclaredValueManager.ID))
        //        ignoreMgrIds.Add(UndeclaredValueManager.ID);

        //    var mgr = uow.ChainOfResponsibility.FindHandlingValueManager(obj, uow, ignoreMgrIds.ToArray());
        //    return mgr;
        //}
        //#endregion

        #region INodeValueManager
        public List<Tuple<object, GraphPath>> GetChildTraversalNodes(object obj, GraphPath nodePath)
        {
            return null;
        }
        public bool CanHandle(object obj, IGraph uow)
        {
            List<string> ignoreMgrIds = new List<string>();
            if (this.ManagerIdsToIgnore != null)
                ignoreMgrIds.AddRange(this.ManagerIdsToIgnore);
            if (!ignoreMgrIds.Contains(UndeclaredValueManager.ID))
                ignoreMgrIds.Add(UndeclaredValueManager.ID);

            var mgr = uow.ChainOfResponsibility.FindHandlingValueManager(obj, uow, ignoreMgrIds.ToArray());
            return mgr != null;
        }
        public string DehydrateValue(object obj, IGraph uow)
        {
            List<string> ignoreMgrIds = new List<string>();
            if (this.ManagerIdsToIgnore != null)
                ignoreMgrIds.AddRange(this.ManagerIdsToIgnore);
            if (!ignoreMgrIds.Contains(UndeclaredValueManager.ID))
                ignoreMgrIds.Add(UndeclaredValueManager.ID);

            var mgr = uow.ChainOfResponsibility.FindHandlingValueManager(obj, uow, ignoreMgrIds.ToArray());

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
