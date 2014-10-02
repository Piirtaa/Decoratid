using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Extensions;

namespace Decoratid.Idioms.TypeLocating
{
    /// <summary>
    /// default implementation of a type locator.  Walks thru all the non-dynamic currently loaded assemblies exported types
    /// </summary>
    public class NaturalTypeLocator : ITypeLocator
    {
        #region Ctor
        public NaturalTypeLocator()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// looks in all the non-dynamic currently loaded assemblies, amongst their exported types, for 
        /// a matching type
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
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


        #region Static Fluent
        public static NaturalTypeLocator New()
        {
            return new NaturalTypeLocator();
        }
        #endregion

    }
}
