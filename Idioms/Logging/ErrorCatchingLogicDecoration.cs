using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Logging;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;

namespace Decoratid.Idioms.Core.Logical.Decorations
{

    public class ErrorCatchingDecoration : DecoratedLogicBase
    {
        #region Ctor
        public ErrorCatchingDecoration(ILogic decorated, ILogger logger = null)
            : base(decorated)
        {
            Logger = logger;
        }
        #endregion

        #region Properties
        ILogger Logger { get; set; }
        #endregion

        #region Methods
        public override void Perform()
        {
            try
            {
                Decorated.Perform();
            }
            catch (Exception ex)
            {
                if (this.Logger != null)
                    this.Logger.LogError(ex.Message, null, ex);
            }
        }
        public override IDecorationOf<ILogic> ApplyThisDecorationTo(ILogic thing)
        {
            return new ErrorCatchingDecoration(thing);
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return string.Empty;
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
        }
        #endregion
    }


}
