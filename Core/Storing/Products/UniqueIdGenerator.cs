using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;

namespace Decoratid.Idioms.Storing.Products
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
    public class UniqueIdGenerator
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
        public UniqueIdGenerator(
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
        
        private void UpdateFromSyncStore()
        {
            int retryCount = 0;
            // maxRetries + 1 because the first run isn't a 're'try.
            while (retryCount < _maxRetries + 1)
            {
                try
                {
                    LongIdGenerationEntry idEntry = _store.Get<LongIdGenerationEntry>(_generatorId);
                    if (idEntry == null)
                    {
                        idEntry = new LongIdGenerationEntry(_generatorId, 1);
                        idEntry.DateCreated = DateTime.UtcNow;
                        _store.SaveItem(idEntry);
                    }

                    //increment and save
                    this._lastId = idEntry.NextId;

                    idEntry.NextId += _rangeSize;
                    _store.SaveItem(idEntry);  //TODO: store should implement concurrency/versioning 

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
}
