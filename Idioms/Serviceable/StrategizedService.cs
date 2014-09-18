using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Serviceable
{
    public class StrategizedService<T> : ServiceBase
    {
        #region Ctor
        public StrategizedService(ILogger logger, T context, Action<T> initStrategy,
            Action<T> startStrategy,
            Action<T> stopStrategy): base(logger)
        {
            this.Context = context;
            this.InitStrategy = initStrategy;
            this.StartStrategy = startStrategy;
            this.StopStrategy = stopStrategy;
        }
        #endregion

        #region Properties
        public T Context { get; private set; }
        private Action<T> InitStrategy { get;  set; }
        private Action<T> StartStrategy { get;  set; }
        private Action<T> StopStrategy { get;  set; }
        #endregion

        #region Overrides
        protected override void initialize()
        {
            if (this.InitStrategy != null)
            {
                this.InitStrategy(this.Context);
            }
        }
        protected override void start()
        {
            if (this.StartStrategy != null)
            {
                this.StartStrategy(this.Context);
            }
        }
        protected override void stop()
        {
            if (this.StopStrategy != null)
            {
                this.StopStrategy(this.Context);
            }
        }
        #endregion
    }
}
