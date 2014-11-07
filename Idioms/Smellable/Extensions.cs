using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Smellable
{
    public static class SmellExtensions
    {
        /// <summary>
        /// throws exception if smell is not Good.  So, on bad and indeterminate smells it's kakky
        /// </summary>
        /// <param name="smellable"></param>
        public static void SmellCheck(this ISmellable smellable)
        {
            Condition.Requires(smellable).IsNotNull();
            if (!smellable.SmellsGood().GetValueOrDefault())
                throw new SmellException("does not smell good");
        }
    }
}
