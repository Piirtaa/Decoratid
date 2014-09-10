using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Sandbox.Conditions;
using Sandbox.Thingness;


namespace Sandbox.Sandboxes
{

    /// <summary>
    /// base class implementation for ISandboxService that uses the Template pattern to expose overrideable 
    /// internals.
    /// </summary>
    public abstract class SandboxBase : ServiceBase, ISandbox
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        protected SandboxBase(string name) : base()
        {
            Condition.Requires(name).IsNotNullOrEmpty();
            this.Name = name;
        }
        #endregion

        #region Properties
        public string Name { get; private set; }
        #endregion

        #region Methods
        public virtual string GetStatus()
        {
            throw new NotImplementedException();
        }
        #endregion




    }
}
