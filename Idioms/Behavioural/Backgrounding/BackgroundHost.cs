using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Decoratid.Idioms.Core.Logical;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.Hydrating;
using Decoratid.Idioms.Core.Disposing;
using Decoratid.Idioms.Core.Behaving;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Core.Backgrounding
{

    public interface IHasBackgroundHost
    {
        BackgroundHost Background { get; }
    }

    /// <summary>
    /// sealed, disposable container class that does stuff in the background on a timer interval
    /// </summary>
    public sealed class BackgroundHost : DisposableBase, IPolyfacing
    {
        #region Declarations
        private readonly object _stateLock = new object();

        //timer stuff
        private System.Timers.Timer _timer = null;
        protected bool _isInBackground = false;
        protected double _backgroundIntervalMSecs = 30000; //30 second default
        protected bool _isEnabled = true;
        #endregion

        #region Ctor
        /// <summary>
        /// starts the background process (enabled), on a 30 second poll, with no action to perform
        /// </summary>
        public BackgroundHost() : this(true, 30000, null)
        {
        }
        public BackgroundHost(bool isEnabled, double backgroundIntervalMSecs, ILogic backgroundAction)
        {
            this._backgroundIntervalMSecs = backgroundIntervalMSecs;
            this.BackgroundAction = backgroundAction;

            //do this last
            this.IsEnabled = isEnabled;
        }
        #endregion

        #region Fluent Static
        public static BackgroundHost NewBlank()
        {
            return new BackgroundHost();
        }
        public static BackgroundHost New(bool isEnabled, double backgroundIntervalMSecs, ILogic backgroundAction)
        {
            return new BackgroundHost(isEnabled, backgroundIntervalMSecs, backgroundAction);
        }
        #endregion

        #region IPolyfacing
        Polyface RootFace { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Set to false if the background process is to be disabled
        /// </summary>
        public bool IsEnabled 
        {
            get { return this._isEnabled; }
            set
            {
                lock (this._stateLock)
                {
                    this._isEnabled = value;
                   
                    if (this._isEnabled)
                    {
                        this.InitTimer();
                    }
                    else
                    {
                        this.DisposeTimer();
                    }
                }
            }
        }
        public double BackgroundIntervalMSecs 
        { 
            get { return this._backgroundIntervalMSecs; }
            set { this._backgroundIntervalMSecs = value; this.InitTimer(); } 
        }
        public ILogic BackgroundAction { get; set; }
        #endregion

        #region Overrides
        protected override void DisposeManaged()
        {
            this.DisposeTimer();
        }
        #endregion

        #region Timer Helpers
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //skip out if not enabled
            if (!this.IsEnabled) { return; }


            if (this._isInBackground) { return; }

            lock (this._stateLock)
            {
                if (this._isInBackground) { return; }
                this._isInBackground = true;
            }

            try
            {
                if (this.BackgroundAction != null)
                {
                    this.BackgroundAction.Perform();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                lock (this._stateLock)
                {
                    this._isInBackground = false;
                }
            }
        }
        /// <summary>
        /// initializes the timer
        /// </summary>
        protected void InitTimer()
        {
            this.DisposeTimer();

            if (this.BackgroundIntervalMSecs > 0)
            {
                this._timer = new System.Timers.Timer();
                this._timer.Interval = this.BackgroundIntervalMSecs;

                this._timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
                this._timer.AutoReset = true;
                this._timer.Enabled = true;
            }
        }
        /// <summary>
        /// disposes the timer
        /// </summary>
        protected void DisposeTimer()
        {
            if (this._timer != null)
            {
                this._timer.Enabled = false;
                this._timer.Dispose();
                this._timer = null;
            }
        }
        #endregion


        //#region IHasHydrationMap
        //public IHydrationMap GetHydrationMap()
        //{
        //    var hydrationMap = new HydrationMapValueManager<BackgroundHost>();
        //    hydrationMap.RegisterDefault("BackgroundAction", (x) => { return x.BackgroundAction; }, (x, y) => { x.BackgroundAction = y as ILogic; });
        //    hydrationMap.Register("_backgroundIntervalMSecs", (x) => { return x._backgroundIntervalMSecs; }, (x, y) => { x._backgroundIntervalMSecs = (double)y; }, PrimitiveValueManager.ID);
        //    //the setter on this will initialize (which may trigger the InitTimer call)
        //    hydrationMap.Register("IsEnabled", (x) => { return x._isEnabled; }, (x, y) => { x.IsEnabled = (bool)y; }, PrimitiveValueManager.ID);

        //    return hydrationMap;
        //}
        //#endregion


    }

    public static class BackgroundExtensions
    {
        public static Polyface AddBackground(this Polyface root, bool isEnabled, double backgroundIntervalMSecs, ILogic backgroundAction)
        {
            Condition.Requires(root).IsNotNull();
            var bg = BackgroundHost.New(isEnabled, backgroundIntervalMSecs, backgroundAction);
            root.Is(bg);
            return root;
        }
        public static BackgroundHost GetBackground(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<BackgroundHost>();
            return rv;
        }
        public static Polyface AddSelfAwareBackground(this Polyface root, ILogicOf<Polyface> action)
        {

        }
    }
}
