using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Conditions;
using Sandbox.Extensions;
using Sandbox.Store.Decorations.Named;

namespace Sandbox.Store.Decorations.Common
{

    public class HasIdDecoration : AbstractDecoration, IHasIdStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        /// <summary>
        /// ctor with no default eviction condition factory.  any items added will not be evicted without an eviction being set
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="backgroundIntervalMSecs"></param>
        public HasIdDecoration(SandboxId id, 
            IStore decorated)
            : base(decorated)
        {

            if (id.IsEmpty)
                throw new InvalidOperationException("id is empty");

            this.Id = id;
        }
        #endregion

        #region IHasIdStore
        public SandboxId Id
        {
            get  ; private set;
        }

        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

    }
}
