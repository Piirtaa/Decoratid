using CuttingEdge.Conditions;
using Decoratid.Idioms.Stringing.Products;
using Decoratid.Idioms.TypeLocating;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Stringing
{
    /// <summary>
    /// Default implementation of IHydrator
    /// </summary>
    public class StringableUtil 
    {
        #region Ctor
        private StringableUtil() { }
        #endregion

        #region Fluent Static 
        public static StringableUtil New()
        {
            return new StringableUtil();
        }
        #endregion

        #region Methods
        /// <summary>
        /// If the text parses to a known type, etc, returns true.  otherwise false.  
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsCompliant(string text)
        {
            bool rv = false;
            var list = LengthEncoder.LengthDecodeList(text);
            if (list == null)
                return rv;
            if (list.Count != 2)
                return rv;

            //pull the type out
            string typeName = list[0];
            var type = TheTypeLocator.Instance.Locator.FindAssemblyQualifiedType(typeName);
            if (type == null)
                return rv;

            if (!typeof(IStringable).IsAssignableFrom(type))
                return rv;

            rv = true;
            return rv;
        }
        #endregion

        #region IHydrator

        /// <summary>
        /// dehydrates hydrateable into the type prefix format
        /// </summary>
        /// <param name="hyd"></param>
        /// <returns></returns>
        public string Dehydrate(IStringable hyd)
        {
            if (hyd == null)
                return null;
            
            string data = hyd.Dehydrate();
            return LengthEncoder.LengthEncodeList(hyd.GetType().AssemblyQualifiedName, data);
        }

        /// <summary>
        /// Reconstitutes an object from the dehydrated, type-prefixed state, using the Hydrate mechanism
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IRehydrating Hydrate(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            if (!IsCompliant(text))
                throw new InvalidOperationException("non compliant encoding");

            var list = LengthEncoder.LengthDecodeList(text);
            string typeName = list[0];
            var type = TheTypeLocator.Instance.Locator.FindAssemblyQualifiedType(typeName);
            Condition.Requires(type).IsNotNull();

            var obj = ReflectionUtil.CreateUninitializedObject(type);
            IRehydrating hyd = obj as IRehydrating;
            hyd.Hydrate(list[1]);
            return hyd;
        }
        #endregion

    }
}
