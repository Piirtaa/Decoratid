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
    public class StrategizedServiceOf<T> : ServiceBase, IHasContext<T>
    {
        #region Ctor
        public StrategizedServiceOf(T context, ILogicOf<T> initStrategy,
            ILogicOf<T> startStrategy,
            ILogicOf<T> stopStrategy)
            : base()
        {
            this.Context = context;
            this.InitStrategy = initStrategy;
            this.StartStrategy = startStrategy;
            this.StopStrategy = stopStrategy;
        }
        #endregion

        #region ISerializable
        protected StrategizedServiceOf(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.InitStrategy = (ILogicOf<T>)info.GetValue("InitStrategy", typeof(ILogicOf<T>));
            this.StartStrategy = (ILogicOf<T>)info.GetValue("StartStrategy", typeof(ILogicOf<T>));
            this.StopStrategy = (ILogicOf<T>)info.GetValue("StopStrategy", typeof(ILogicOf<T>));
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("InitStrategy", this.InitStrategy);
            info.AddValue("StartStrategy", this.StartStrategy);
            info.AddValue("StopStrategy", this.StopStrategy);

            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasContext
        object IHasContext.Context
        {
            get
            {
                return this.Context;
            }
            set
            {
                this.Context = (T)value;
            }
        }
        public T Context { get; set; }
        #endregion

        #region Properties
        private ILogicOf<T> InitStrategy { get; set; }
        private ILogicOf<T> StartStrategy { get; set; }
        private ILogicOf<T> StopStrategy { get; set; }
        #endregion

        #region Overrides
        protected override void initialize()
        {
            if (this.InitStrategy != null)
            {
                this.InitStrategy.Context = this.Context.AsNaturalValue();
                this.InitStrategy.Perform();
            }
        }
        protected override void start()
        {
            if (this.StartStrategy != null)
            {
                this.StartStrategy.Context = this.Context.AsNaturalValue();
                this.StartStrategy.Perform();
            }
        }
        protected override void stop()
        {
            if (this.StopStrategy != null)
            {
                this.StopStrategy.Context = this.Context.AsNaturalValue();
                this.StopStrategy.Perform();
            }
        }
        #endregion

        #region Fluent Static
        public static StrategizedServiceOf<T> New(T context, ILogicOf<T> initStrategy,
            ILogicOf<T> startStrategy,
            ILogicOf<T> stopStrategy)
        {
            return new StrategizedServiceOf<T>(context, initStrategy, startStrategy, stopStrategy);
        }
        #endregion
    }
}
