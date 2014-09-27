using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Polyfacing;

namespace Decoratid.Idioms.IdGenerating
{
    //inspiration from http://msdn.microsoft.com/en-us/magazine/gg309174.aspx

    /// <summary>
    /// holds the next starting id (of type long) to be brokered out from, for an entry of a given generatorId 
    /// </summary>
    public class LongIdGenerationEntry : IHasId<string>
    {
        #region Ctor
        public LongIdGenerationEntry(string generatorId, Int64 nextId)
        {
            Condition.Requires(generatorId).IsNotNullOrEmpty();
            this.Id = generatorId;
            this.NextId = nextId;
        }
        #endregion

        #region Properties
        public Int64 NextId { get; set; }
        public string GeneratorId { get { return this.Id; } }
        #endregion

        #region IHasId
        public string Id { get; set; }

        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region IHasPersistenceDates
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        #endregion
    }

    /// <summary>
    /// this class attaches to a store that contains entries of type LongIdGenerationEntry, and uses this to broker ranges of ids
    /// to be consumed
    /// </summary>
    public class LongIdBroker : IPolyfacing
    {
        #region Declarations
        private readonly object _stateLock = new object();
        private Int64 _lastId;
        private Int64 _upperLimit;
        private readonly int _rangeSize;
        private readonly int _maxRetries;
        private readonly IStore _store;
        private readonly string _generatorId;
        #endregion

        #region Ctor
        public LongIdBroker(
          string generatorId,
          IStore store,
          int rangeSize = 1000,
          int maxRetries = 25)
        {
            _generatorId = generatorId;
            _rangeSize = rangeSize;
            _maxRetries = maxRetries;
            _store = store;
        }
        #endregion

        #region IPolyfacing
        Polyface IPolyfacing.RootFace { get; set; }
        #endregion

        #region Methods
        public Int64 NextId()
        {
            lock (_stateLock)
            {
                if (_lastId == _upperLimit)
                {
                    //reserve more from the store
                    UpdateFromSyncStore();
                }
                return _lastId++;
            }
        }
        /// <summary>
        /// grabs a block of ids from the store, updating the store in the process
        /// </summary>
        private void UpdateFromSyncStore()
        {
            int retryCount = 0;
            
            //try until we max our tries out
            while (retryCount < _maxRetries + 1)
            {
                try
                {
                    //get the id status from the store
                    LongIdGenerationEntry idEntry = _store.Get<LongIdGenerationEntry>(_generatorId);
                    if (idEntry == null)
                    {
                        //don't have an id?  make a new one and save it to the store
                        idEntry = new LongIdGenerationEntry(_generatorId, 1);
                        idEntry.DateCreated = DateTime.UtcNow;
                        _store.SaveItem(idEntry);
                    }

                    //set our local id
                    this._lastId = idEntry.NextId;

                    //reserve those id blocks 
                    idEntry.NextId += _rangeSize;
                    _store.SaveItem(idEntry);  //TODO: store should implement concurrency/versioning, but if the range is large enough we're prob good 

                    this._upperLimit = idEntry.NextId - 1;
                    
                    return;
                }
                catch { }

                retryCount++;
                // update failed, go back around the loop
            }
            throw new Exception(string.Format(
              "Failed to update the Store after {0} attempts",
              retryCount));
        }
        #endregion
    }



    public static class LongIdBrokerExtensions
    {
        public static Polyface IsIdBroker(this Polyface root,
          string generatorId,
          IStore store,
          int rangeSize = 1000,
          int maxRetries = 25)
        {
            Condition.Requires(root).IsNotNull();
            var obj = new LongIdBroker(generatorId, store, rangeSize, maxRetries);
            root.Is(obj);
            return root;
        }
        public static LongIdBroker AsIdBroker(this Polyface root)
        {
            Condition.Requires(root).IsNotNull();
            var rv = root.As<LongIdBroker>();
            return rv;
        }




    }
}
