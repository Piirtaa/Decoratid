using CuttingEdge.Conditions;
using Decoratid.Idioms.Polyfacing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Throttling
{
    [Serializable]
    public class NaturalThrottle : IThrottle, IPolyfacing
    {
        #region Declarations
        private SemaphoreSlim _maxConnectionsSemaphore;
        #endregion

        #region Ctor
        public NaturalThrottle(int maxConcurrency)
        {
            Condition.Requires(maxConcurrency).IsGreaterThan(0);
            this.ConcurrencyLimit = maxConcurrency;
            _maxConnectionsSemaphore = new SemaphoreSlim(maxConcurrency);
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region Properties
        public int ConcurrencyLimit { get; private set; }
        #endregion

        #region Methods
        public void Reset()
        {
            _maxConnectionsSemaphore = new SemaphoreSlim(ConcurrencyLimit);
        }
        public void Perform(Action action)
        {
            if (action == null)
                return;

            this.StartOperation();
            try
            {
                action();
            }
            catch
            {
                throw;
            }
            finally
            {
                this.CompleteOperation();
            }
        }
        protected void StartOperation()
        {
            this._maxConnectionsSemaphore.Wait();
        }
        protected void CompleteOperation()
        {
            this._maxConnectionsSemaphore.Release();
        }
        #endregion

    }

    public static class NaturalThrottleExtensions
    {
        public static Polyface IsThrottle(this Polyface root, int maxConcurrency)
        {
            Condition.Requires(root).IsNotNull();
            var composited = new NaturalThrottle(maxConcurrency);
            root.Is(composited);
            return root;
        }
        public static NaturalThrottle AsThrottle(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<NaturalThrottle>();
            return rv;
        }
    }
}
