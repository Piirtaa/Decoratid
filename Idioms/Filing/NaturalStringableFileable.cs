using CuttingEdge.Conditions;
using Decoratid.Idioms.Stringing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Filing
{
    [Serializable]
    public class NaturalStringableFileable : FileableStringableDecoration
    {
        #region Ctor
        public NaturalStringableFileable()
            : base(NaturalStringable.New())
        {
        }
        #endregion
    }
}
