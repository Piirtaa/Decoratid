using Decoratid.Core;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.HasBitsing;
using Decoratid.Storidioms.StoreOf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using CuttingEdge.Conditions;

namespace Decoratid.Storidioms.Indexing
{
    //define behaviour of the logic for each bit
    using IndexingBitLogic = IsA<IHasId<int>, IHasName, LogicOfTo<IHasId, bool>>;

    /// <summary>
    /// defines factory that will generate HasBits for an item(IHasId)
    /// </summary>
    public interface IIndexFactory
    {
        /// <summary>
        /// sets the logic for a particular bit
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hasBitLogic"></param>
        /// <param name="index">if not provided will increment the hasbits size and set as last bit logic</param>
        void SetBitLogic(string name, Func<IHasId, bool> hasBitLogic, int index = -1);
        /// <summary>
        /// for a hasId generates the hasbits index
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        IHasBits GenerateIndex(IHasId obj);
    }

    /// <summary>
    /// Responsible for maintaining the bit logic for when a HasId needs to have its
    /// index calculated.  internally maintains a store of IndexingBitLogic
    /// </summary>
    public class IndexFactory : IIndexFactory
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public IndexFactory()
        {
            this.StoreOfBitLogic = NaturalInMemoryStore.New().IsOf<IndexingBitLogic>();
        }
        #endregion

        #region Properties
        private IStoreOf<IndexingBitLogic> StoreOfBitLogic { get; set; }
        #endregion

        #region IIndexFactory
        public void SetBitLogic(string name, Func<IHasId, bool> hasBitLogic, int index = -1)
        {
            lock (this._stateLock)
            {
                Condition.Requires(name).IsNotNullOrEmpty();
                Condition.Requires(hasBitLogic).IsNotNull();

                //DO A GENERAL VALIDATION OF LIST INTEGRITY
                //get all the bitlogic items
                var bitLogics = this.StoreOfBitLogic.GetAll().OrderBy(x => x.Id).ToArray();

                //validate they're all sequential starting from zero
                for (int i = 0; i < bitLogics.Length; i++)
                {
                    var each = bitLogics[i];
                    //validate an entry exists
                    Condition.Requires(each).IsNotNull("null bit entry @ " + i);
                    //validate the indexes are consistent
                    Condition.Requires(each.Id.ToString()).IsEqualTo(i.ToString(), "index mismatch @ " + i);
                }
                //OK WE'RE VALID

                //set the item
                IndexingBitLogic storeItem = null;
                //if specifying index, it must be existing or next new
                if (index >= 0)
                {
                    Condition.Requires(index).IsLessOrEqual(bitLogics.Length, "out of range index");

                    //build the new item
                    storeItem = IndexingBitLogic.New(LogicOfTo<IHasId, bool>.New(hasBitLogic).HasId<int>(index).HasName(name));
                }
                else if (index == -1)
                {
                    //build the new item
                    storeItem = IndexingBitLogic.New(LogicOfTo<IHasId, bool>.New(hasBitLogic).HasId<int>(bitLogics.Length).HasName(name));
                }
                else
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                //save the store item
                this.StoreOfBitLogic.SaveItem(storeItem);
            }
        }
        public IHasBits GenerateIndex(IHasId obj)
        {
            //get all the bitlogic items
            var bitLogics = this.StoreOfBitLogic.GetAll().OrderBy(x => x.Id).ToArray();

            var rv = NaturalBitArrayHasBits.New(new BitArray(bitLogics.Length));

            //foreach item, position by position, set the hasbits values
            bitLogics.WithEach(x =>
            {
                var nameFace = x.As<IHasName>();
                var indexFace = x.As<IHasId<int>>();
                var logicFace = x.As<LogicOfTo<IHasId, bool>>();

                var logic = logicFace.Perform(obj) as LogicOfTo<IHasId, bool>;
                rv.SetBit(indexFace.Id, logic.Result);
            });

            return rv;
        }
        #endregion
    }
}
