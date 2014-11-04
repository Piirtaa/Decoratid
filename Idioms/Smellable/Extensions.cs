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
        public static void SmellCheck(this ISmellable smellable)
        {
            //On this code:
            //the expression "does not smell good" is quite literally one of
            //those stupid human idioms to otherwise say something smells bad.  Fuck it, we're a fussy bunch
            //of passive agros eh.  This SmellsBad.  Hopefully we can do better as dumbass humans and
            //just say what we mean.  God Damn.
            
            //ps. In the case below it maintains the Smellable idiom in that an indeterminate smell WILL be
            //an exception circumstance at times, and this call allows us to ask that question.

            Condition.Requires(smellable).IsNotNull();
            if (!smellable.SmellsGood().GetValueOrDefault())
                throw new SmellException("does not smell good");
        }
    }
}
