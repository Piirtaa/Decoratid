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
    /// Adds a logger to condition evaluation
    /// </summary>
    public class LoggingConditionDecoration : DecoratedConditionBase, IHasLogger
    {
        #region Ctor
        public LoggingConditionDecoration(ICondition decorated, ILogger logger = null)
            : base(decorated)
        {
            Logger = logger;
        }
        #endregion

        #region ISerializable
        protected LoggingConditionDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public ILogger Logger { get; private set; }
        #endregion

        #region Methods
        public override bool? Evaluate()
        {
            if (this.Logger != null)
                this.Logger.LogVerbose("Evaluate started", null);

            try
            {
                return base.Evaluate();
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
                    this.Logger.LogVerbose("Evaluate completed", null);
            }
            return false;
        }
        public override IDecorationOf<ICondition> ApplyThisDecorationTo(ICondition thing)
        {
            if (this.Logger != null)
                this.Logger.LogVerbose("ApplyThisDecorationTo started", thing);

            IDecorationOf<ICondition> rv = null;
            try
            {
                rv = new LoggingConditionDecoration(thing);
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

    public static class LoggingConditionDecorationExtensions
    {
        public static LoggingConditionDecoration WithLogging(ICondition decorated)
        {
            return new LoggingConditionDecoration(decorated);
        }
    }
}
