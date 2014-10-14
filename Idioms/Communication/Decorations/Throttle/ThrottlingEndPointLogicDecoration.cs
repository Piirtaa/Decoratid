using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Logging;
using Decoratid.Thingness;
using Decoratid.Idioms.Decorating;

namespace Decoratid.Communication.Decorations.Throttle
{
    /// <summary>
    /// decorates the endpoint logic to provide concurrency throttling
    /// </summary>
    public interface IThrottlingEndPointLogic : IDecoratedEndPointLogic
    {
        int MaxConnections { get; set; }
        /// <summary>
        /// reset the count of current connections
        /// </summary>
        void Reset();
    }
    /// <summary>
    /// decorates the endpoint logic to provide concurrency throttling
    /// </summary>
    public class ThrottlingEndPointLogicDecoration : DecoratedEndPointLogicBase, IThrottlingEndPointLogic
    {
        #region Declarations
        protected SemaphoreSlim _maxConnectionsSemaphore;
        #endregion

        #region Ctor
        public ThrottlingEndPointLogicDecoration(IEndPointLogic decorated, int maxConnections) 
            : base(decorated)
        {
            MaxConnections = maxConnections;

            this.Reset();
        }
        #endregion

        #region MaxConnections
        public int MaxConnections { get;  set; }
        #endregion

        #region Overrides
        public void Reset()
        {
            if (this.MaxConnections > 0)
            {
                _maxConnectionsSemaphore = new SemaphoreSlim(MaxConnections);
            }
        }
        public override string HandleRequest(string data)
        {
            this.MarkStartRequest();
            string val =  base.HandleRequest(data);
            this.MarkEndRequest();
            return val;
        }
        public override IDecorationOf<IEndPointLogic> ApplyThisDecorationTo(IEndPointLogic thing)
        {
            return new ThrottlingEndPointLogicDecoration(thing, this.MaxConnections);
        }
        #endregion

        #region Helpers
        protected void MarkStartRequest()
        {
            //this.Log("MarkStartRequest", LogLevel.Verbose_5);

            if (this._maxConnectionsSemaphore != null)
            {
                this._maxConnectionsSemaphore.Wait();
            }
        }
        protected void MarkEndRequest()
        {
            //this.Log("MarkEndRequest", LogLevel.Verbose_5);

            if (this._maxConnectionsSemaphore != null)
            {
                this._maxConnectionsSemaphore.Release();
            }
        }
        #endregion
    }

    public static partial class Extensions
    {
        public static IThrottlingEndPointLogic DecorateWithThrottling(this IEndPointLogic decorated, int maxConnections)
        {
            Condition.Requires(decorated).IsNotNull();
            return new ThrottlingEndPointLogicDecoration(decorated, maxConnections);
        }
    }
}
