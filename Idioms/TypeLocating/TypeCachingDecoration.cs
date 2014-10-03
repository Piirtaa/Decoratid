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
    /// <summary>
    /// doesn't do anything but keeps track of types loaded
    /// </summary>
    [Serializable]
    public class TypeCachingDecoration : TypeLocatorDecorationBase
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public TypeCachingDecoration(AssemblyTrackingDecoration decorated)
            : base(decorated)
        {
            this.Types = new List<Type>();
            decorated.AssemblyRegistered += decorated_AssemblyRegistered;
        }

        void decorated_AssemblyRegistered(object sender, EventArgOf<AssemblyName> e)
        {
            lock (this._stateLock)
            {
                AssemblyName an = e.Value;

                //update the type list - has nothing to do with what assembly is loaded.  yes this is repetitive
                AppDomain.CurrentDomain.GetAssemblies().ToList().WithEach(x =>
                {
                    if (!x.IsDynamic)
                    {
                        x.GetExportedTypes().WithEach(type =>
                        {
                            if(!this.Types.Contains(type))
                                this.Types.Add(type);
                        });
                    }
                });
            }
        }
        #endregion

        #region ISerializable
        protected TypeCachingDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public List<Type> Types { get; private set; }
        #endregion

        #region Methods
        public override List<Type> Locate(Func<Type, bool> filter)
        {
            //look in our type list first
            List<Type> returnValue = new List<Type>();

            this.Types.WithEach(type =>
                    {
                        if (filter(type))
                            returnValue.Add(type);
                    });

            if (returnValue.Count > 0)
                return returnValue;

            return Decorated.Locate(filter);
        }
        public override IDecorationOf<ITypeLocator> ApplyThisDecorationTo(ITypeLocator thing)
        {
            Condition.Requires(thing).IsOfType(typeof(AssemblyTrackingDecoration));
            return new TypeCachingDecoration(thing as AssemblyTrackingDecoration);
        }
        #endregion
    }

    public static class TypeCachingDecorationExtensions
    {
        public static TypeCachingDecoration CacheTypes(this AssemblyTrackingDecoration decorated)
        {
            Condition.Requires(decorated).IsNotNull();
            return new TypeCachingDecoration(decorated);
        }
    }
}
