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
using Decoratid.Idioms.Core.ValueOfing;

namespace Decoratid.Idioms.Validating
{

    public class ValidatingValueOfDecoration<T> : DecoratedValueOfBase<T>
    {
        #region Ctor
        public LoggingValueOfDecoration(IValueOf<T> decorated, , ICondition isValidCondition)
            : base(decorated)
        {
            Logger = logger;
        }
        #endregion

        #region Properties
        public ICondition IsValidCondition {get; private set;}
        #endregion

        #region Methods
        public override T GetValue()
        {
            if (this.Logger != null)
                this.Logger.LogVerbose("GetValue started", null);

            try
            {
                return Decorated.GetValue();
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
                    this.Logger.LogVerbose("GetValue completed", null);
            }
            return default(T);
        }
        public override IDecorationOf<IValueOf<T>> ApplyThisDecorationTo(IValueOf<T> thing)
        {
            if (this.Logger != null)
                this.Logger.LogVerbose("ApplyThisDecorationTo started", thing);

            IDecorationOf<IValueOf<T>> rv = null;
            try
            {
                rv = new LoggingValueOfDecoration<T>(thing);
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
