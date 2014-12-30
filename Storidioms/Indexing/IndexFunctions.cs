using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Decorating;

namespace Decoratid.Storidioms.Indexing
{
    /// <summary>
    /// Helper class containing common index functions
    /// </summary>
    public static class IndexFunctions
    {
        /// <summary>
        /// returns the index that returns the object's decoration signature (a comma separated list of 
        /// decoration types
        /// </summary>
        public static LogicOfTo<IHasId, string> ExactDecorationSignatureFunction
        {
            get
            {
                return LogicOfTo<IHasId, string>.New(x =>
                {
                    return x.GetExactDecorationSignature();
                });
            }
        }
        public static LogicOfTo<IHasId, string> AlphabeticDecorationSignatureFunction
        {
            get
            {
                return LogicOfTo<IHasId, string>.New(x =>
                {
                    return x.GetAlphabeticDecorationSignature();
                });
            }
        }
    }
}
