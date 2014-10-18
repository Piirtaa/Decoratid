using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Conditional;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Backgrounding;
using Decoratid.Idioms.Polyfacing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decoratid.Idioms.ConditionalWaiting
{
    /// <summary>
    /// this guy waits around on a condition.  uses background polling.  very low-tech, bro
    /// </summary>
    [Serializable]
    public sealed class ConditionalWaiter : DisposableBase, IConditionalWaiter, IPolyfacing, ISerializable
    {
        #region Declarations
        private readonly object _stateLock = new object();
        private BackgroundHost _background;
        #endregion

        #region Ctor
        public ConditionalWaiter(ICondition condition, ICondition stopWaitingCondition = null) : base()
        {
            CuttingEdge.Conditions.Condition.Requires(condition).IsNotNull();
            this.Condition = Condition;
            this.StopWaitingCondition = stopWaitingCondition;

            this._background = new BackgroundHost(true, 1000,
                Logic.New(() =>
                {
                    lock (_stateLock)
                    {
                        if (Condition.Evaluate().GetValueOrDefault())
                        {
                            this._isResolved = true;
                            Monitor.Pulse(_stateLock);
                        }
                        else if (this.StopWaitingCondition != null && this.StopWaitingCondition.Evaluate().GetValueOrDefault() == true)
                        {
                            this._isResolved = false;
                            Monitor.Pulse(_stateLock);
                        }
                    }
                }));
        }
        #endregion

        #region ISerializable
        private ConditionalWaiter(SerializationInfo info, StreamingContext context)
        {
            this.Condition = (ICondition)info.GetValue("Condition", typeof(ICondition));
            this.Condition = (ICondition)info.GetValue("StopWaitingCondition", typeof(ICondition));
        }
        private void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Condition", this.Condition);
            info.AddValue("StopWaitingCondition", this.StopWaitingCondition);
        }
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region IHasCondition
        public ICondition Condition { get; set; }
        public ICondition StopWaitingCondition { get; private set; }
        #endregion

        #region IConditionalWaiter
        private bool? _isResolved = null; 

        public bool WaitAround()
        {
            lock (_stateLock)
            {
                this._isResolved = null;

                while (this._isResolved== null)
                {
                    if (Condition.Evaluate().GetValueOrDefault())
                        this._isResolved = true;

                    if (this.StopWaitingCondition != null && this.StopWaitingCondition.Evaluate().GetValueOrDefault() == true)
                        this._isResolved = false;

                    if (this._isResolved == null) //let the poll job do the check from now on
                        Monitor.Wait(_stateLock);
                }
            }
            return this._isResolved.GetValueOrDefault();
        }
        #endregion

        #region Overrides
        protected override void DisposeManaged()
        {
            if (this._background != null)
                this._background.Dispose();
        }
        #endregion

        #region Fluent Static
        public static ConditionalWaiter New(ICondition Condition, ICondition stopWaitingCondition)
        {
            return new ConditionalWaiter(Condition, stopWaitingCondition);
        }
        #endregion
    }

    public static class ConditionalWaiterExtensions
    {
        public static Polyface IsConditionalWaiter(this Polyface root, ICondition condition, ICondition stopWaitingCondition)
        {
            Condition.Requires(root).IsNotNull();
            var bg = new ConditionalWaiter(condition, stopWaitingCondition);
            root.Is(bg);
            return root;
        }
        public static ConditionalWaiter AsConditionalWaiter(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<ConditionalWaiter>();
            return rv;
        }



    }
}
