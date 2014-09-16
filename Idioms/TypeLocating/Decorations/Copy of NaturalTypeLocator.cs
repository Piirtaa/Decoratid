using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Extensions;

namespace Decoratid.Idioms.TypeLocation
{
    /// <summary>
    /// default implementation of a type locator.  
    /// on construction will force all assemblies auto load all the assemblies configured by AssemblyTracker
    /// </summary>
    public class DefaultTypeLocator : ITypeLocator
    {
        #region Ctor
        public DefaultTypeLocator()
        {
            //force load of all assemblies
            AssemblyTracker.Instance.Load();
        }
        #endregion

        #region Methods
        public List<Type> Locate(Func<Type, bool> filter)
        {
            Condition.Requires(filter).IsNotNull();

            List<Type> returnValue = new List<Type>();

            AppDomain.CurrentDomain.GetAssemblies().ToList().WithEach(x =>
            {
                //grab the path of the assembly and store it for later
                if (!x.IsDynamic)
                {
                    x.GetExportedTypes().WithEach(type =>
                    {
                        if (filter(type))
                            returnValue.Add(type);
                    });
                }
            });

            return returnValue;
        }
        #endregion

    }
}
