using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using System;

namespace Decoratid.Messaging.StoreProtocol
{
    /// <summary>
    /// Contains the state of a unit of work (req/resp) handled by the store protocol.
    /// The "store protocol" is just an exchange of stores (eg. request is a store and response is a store) 
    /// </summary>
    /// <remarks>
    /// We use a Decoratid id here - facilitates tagging.
    /// We use IEventingStore on the ResponseStore rather than IStore so that we have hooks for decoration
    ///     (or injection) and eventing.
    /// </remarks>
    public class StoreProtocolUnitOfWork : IHasId<DecoratidId>
    {
        #region Ctor
        public StoreProtocolUnitOfWork(IStore requestStore)
        {
            Condition.Requires(requestStore).IsNotNull();
            this.RequestStore = requestStore;
            this.ResponseStore = new InMemoryStore().DecorateWithEvents();
            this.Id = new DecoratidId();
        }
        #endregion

        #region Properties
        public IStore RequestStore { get; protected set; }
        public IEventingStore ResponseStore { get; protected set; }
        public Exception Error { get; protected set; }
        #endregion

        #region IHasId
        public DecoratidId Id { get; private set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Methods
        public void SetError(Exception ex)
        {
            Condition.Requires(ex).IsNotNull();
            this.Error = ex;
        }
        #endregion
    }
}