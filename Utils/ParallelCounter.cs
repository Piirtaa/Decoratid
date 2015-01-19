using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{


    /// <summary>
    /// tracks parallel operations
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class ParallelCounter
    {
        #region Inner Classes
        private enum ItemStatusEnum
        {
            Waiting, Running, Complete
        }
        #endregion

        #region Declarations
        private readonly int _count;
        private readonly Action<int> _itemAction;
        private List<ItemStatusEnum> _pendingItems = new List<ItemStatusEnum>();
        private List<ItemStatusEnum> _items = new List<ItemStatusEnum>();
        
        #endregion

        #region Ctor
        public ParallelCounter(int count, Action<int> itemAction)
        {
            this._count = count;
            this._itemAction = itemAction;

            for (int i = 0; i < count; i++)
            {
                
            }
        }
        #endregion

        #region Status Functions
        private ItemStatusEnum GetStatus(int idx)
        {
            return this._items[idx];
        }
        private void MarkRunning(int idx)
        {
            this._items[idx] = ItemStatusEnum.Running;
        }
        public void MarkComplete(int idxFrom, int idxTo)
        {
            for (int i = idxFrom; i <= idxTo; i++)
                this._items[i] = ItemStatusEnum.Complete;
        }
        private int _nextFree;
        private int _lastFree;
        #endregion

        #region Methods
        public void Run()
        {
    //                    Parallel.For(0, _count -1, () => new List<StringSearchMatch>(),
    //(x, loop, subList) =>
    //{
    //    int grasp;
    //    var list = (this.Decorated as IByPositionStringSearcher).FindMatchesAtPosition(x, text, out grasp);
    //    subList.AddRange(list);
    //    return subList;
    //},
    //(x) => { rv.AddRange(x); }
        }
        #endregion

    }
}
