using Decoratid.Core.Identifying;
using Decoratid.Idioms.HasBitsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Storidioms.Indexing
{
    public class IndexGenerator : IIndexGenerator
    {
        
        #region Ctor

        #endregion

        #region IIndexGenerators
        void AddBitToIndex(string name, Func<IHasId, bool> HasBitLogic, int index = -1);
        public List<IIndexingBitLogic> BitLogic { get; }
        public IHasBits GenerateIndex(IHasId obj);
   
        #endregion
    }
}
