using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;

namespace Decoratid.Extensions
{
    public static class IEqualsExtensions
    {
        static CompareLogic _compareLogic = new CompareLogic();

        /// <summary>
        /// does a deep compare
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool IsDeepEqual(this object obj, object obj2)
        {
            return _compareLogic.Compare(obj, obj2).AreEqual;
        }
    }
}
