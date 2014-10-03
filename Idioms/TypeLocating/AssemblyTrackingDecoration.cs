using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using Decoratid.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace Decoratid.Idioms.TypeLocating
{
    public interface IAssemblyTrackingTypeLocator : ITypeLocatorDecoration
    {
        Dictionary<string, AssemblyName> Assemblies { get;}
        void TrackAssembly(AssemblyName aName);
    }
    /// <summary>
    /// doesn't do anything but keeps track of assemblies loaded
    /// </summary>
    [Serializable]
    public class AssemblyTrackingDecoration : TypeLocatorDecorationBase, IAssemblyTrackingTypeLocator
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public AssemblyTrackingDecoration(ITypeLocator decorated)
            : base(decorated)
        {
            this.Assemblies = new Dictionary<string, AssemblyName>();
            //listen to app domain events
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        }
        #endregion

        #region ISerializable
        protected AssemblyTrackingDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Our Events
        public event EventHandler<EventArgOf<AssemblyName>> AssemblyRegistered;
        public void OnAssemblyRegistered(AssemblyName aName)
        {
            //skip if no listeners attached
            if (this.AssemblyRegistered == null)
                return;

            var args = this.AssemblyRegistered.BuildEventArgs(aName);
            //fire the event
            this.AssemblyRegistered(this, args);
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of the loaded assemblies
        /// </summary>
        public Dictionary<string, AssemblyName> Assemblies { get; private set; }
        #endregion

        #region Assembly Event Handlers
        void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            try
            {
                var assm = args.LoadedAssembly;
                var aName = assm.GetName();
                this.TrackAssembly(aName);
            }
            catch { }
        }
        #endregion


        #region Methods
        /// <summary>
        /// Adds the assembly to the local registry of assemblies. 
        /// </summary>
        /// <param name="assm"></param>
        public void TrackAssembly(AssemblyName aName)
        {
            if (aName == null) { return; }

            if (this.IsAssemblyTracked(aName)) { return; }

            lock (this._stateLock)
            {
                if (this.IsAssemblyTracked(aName)) { return; }

                this.Assemblies.Add(aName.FullName, aName);

                try
                {
                    //raise event
                    this.OnAssemblyRegistered(aName);
                }
                catch { }
            }
        }
        public override List<Type> Locate(Func<Type, bool> filter)
        {
            return Decorated.Locate(filter);
        }
        public override IDecorationOf<ITypeLocator> ApplyThisDecorationTo(ITypeLocator thing)
        {
            return new AssemblyTrackingDecoration(thing);
        }
        #endregion

        #region Assembly Tracking 
        /// <summary>
        /// Is the assembly tracked?
        /// </summary>
        /// <param name="aName"></param>
        /// <returns></returns>
        private bool IsAssemblyTracked(AssemblyName aName)
        {
            bool returnValue = false;

            //null check
            if (aName == null) { return returnValue; }

            //grab name - this kacks with perms and other misc. constraints 
            string name = string.Empty;
            try
            {
                name = aName.FullName;
            }
            catch
            {
                //if kacks, skip
                return returnValue;
            }

            returnValue = this.Assemblies.ContainsKey(aName.FullName);

            return returnValue;
        }


        #endregion
    }

    public static class AssemblyTrackingDecorationExtensions
    {
        public static AssemblyTrackingDecoration TrackAssemblies(this ITypeLocator decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new AssemblyTrackingDecoration(decorated);
        }
    }
}
