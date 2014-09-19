using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.Core;

namespace Decoratid.Idioms.Logging
{
    /// <summary>
    /// Adds a logger to logic
    /// </summary>
    public class LoggingLogicDecoration : DecoratedLogicBase, IHasLogger
    {
        #region Ctor
        public LoggingLogicDecoration(ILogic decorated, ILogger logger = null)
            : base(decorated)
        {
            Logger = logger;
        }
        #endregion

        #region Properties
        public ILogger Logger { get; private set; }
        #endregion

        #region Methods
        public override void Perform()
        {
            if (this.Logger != null)
                this.Logger.LogVerbose("Perform started", null);

            try
            {
                Decorated.Perform();
            }
            catch (Exception ex)
            {
                if (this.Logger != null)
                    this.Logger.LogError(ex.Message, null, ex);

                throw;
            }
            finally
            {
                if (this.Logger != null)
                    this.Logger.LogVerbose("Perform completed", null);
            }
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            if (this.Logger != null)
                this.Logger.LogVerbose("ApplyThisDecorationTo started", thing);

            IDecorationOf<ILogic> rv = null;
            try
            {
                rv = new LoggingLogicDecoration(thing);
            }
            catch (Exception ex)
            {
                if (this.Logger != null)
                    this.Logger.LogError(ex.Message, null, ex);

                throw;
            }
            finally
            {
                if (this.Logger != null)
                    this.Logger.LogVerbose("ApplyThisDecorationTo completed", null);
            }

            return rv;
        }
        #endregion
    }


}
