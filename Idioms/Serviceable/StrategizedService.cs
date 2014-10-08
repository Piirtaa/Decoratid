using Decoratid.Core.Contextual;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Idioms.Serviceable
{
    [Serializable]
    public class StrategizedService : ServiceBase
    {
        #region Ctor
        public StrategizedService(ILogic initStrategy,
            ILogic startStrategy,
            ILogic stopStrategy)
            : base()
        {
            this.InitStrategy = initStrategy;
            this.StartStrategy = startStrategy;
            this.StopStrategy = stopStrategy;
        }
        #endregion

        #region ISerializable
        protected StrategizedService(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.InitStrategy = (ILogic)info.GetValue("InitStrategy", typeof(ILogic));
            this.StartStrategy = (ILogic)info.GetValue("StartStrategy", typeof(ILogic));
            this.StopStrategy = (ILogic)info.GetValue("StopStrategy", typeof(ILogic));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("InitStrategy", this.InitStrategy);
            info.AddValue("StartStrategy", this.StartStrategy);
            info.AddValue("StopStrategy", this.StopStrategy);

            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        private ILogic InitStrategy { get; set; }
        private ILogic StartStrategy { get; set; }
        private ILogic StopStrategy { get; set; }
        #endregion

        #region Overrides
        protected override void initialize()
        {
            if (this.InitStrategy != null)
            {
                this.InitStrategy.Perform();
            }
        }
        protected override void start()
        {
            if (this.StartStrategy != null)
            {
                this.StartStrategy.Perform();
            }
        }
        protected override void stop()
        {
            if (this.StopStrategy != null)
            {
                this.StopStrategy.Perform();
            }
        }
        #endregion

        #region Fluent Static
        public static StrategizedService New(ILogic initStrategy,
            ILogic startStrategy,
            ILogic stopStrategy)
        {
            return new StrategizedService(initStrategy, startStrategy, stopStrategy);
        }
        #endregion
    }
}
