using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Storidioms;
using Decoratid.Storidioms.StoreOf;
using System;

namespace Decoratid.Messaging.StoreProtocol
{
    // The "store protocol" is just an exchange of stores (eg. request is a store and response is a store), serialized as strings 

    /// <summary>
    /// a StoreProtocol handler performs some logic on the unit of work
    /// </summary>
    public interface IStoreProtocolHandler : IHasId<string>
    {
        void Handle(StoreProtocolUnitOfWork uow);
    }

    /// <summary>
    /// the implementation of IEndPointLogic that implements the Store Protocol.
    /// </summary>
    /// <remarks>
    /// Contains a store of IStoreProtocolHandlers.  Upon receipt of a request, a 
    /// unit of work is created, and each handler touches the uow.
    /// </remarks>
    [Serializable]
    public class StoreProtocolEndpointLogic 
    {
        #region Ctor
        public StoreProtocolEndpointLogic(IStoreOf<IStoreProtocolHandler> handlerStore)
        {
            Condition.Requires(handlerStore).IsNotNull();
            this.HandlerStore = handlerStore;
        }
        #endregion

        #region Properties
        public IStoreOf<IStoreProtocolHandler> HandlerStore { get; protected set; }
        #endregion

        #region IEndPointLogic
        public string HandleRequest(string request)
        {
            //decode the request store
            var  store = StoreSerializer.DeserializeStore(request, null);

            //create a unit of work
            StoreProtocolUnitOfWork uow = new StoreProtocolUnitOfWork(store);

            //get all the handlers that will be touching the unit of work
            var handlers = HandlerStore.GetAll();
            Condition.Requires(handlers).IsNotEmpty();

            //touch the unit of work
            foreach (var x in handlers)
            {
                try
                {
                    x.Handle(uow);
                }
                catch (Exception ex)
                {
                    uow.SetError(ex);
                    break;
                }
            }

            //encode the response
            var responseText = StoreSerializer.SerializeStore(uow.ResponseStore, null);
            return responseText;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// creates a new instance of StoreProtocolEndpointLogic
        /// </summary>
        /// <param name="encodingStrategy"></param>
        /// <param name="decodingStrategy"></param>
        /// <param name="handlerStore"></param>
        /// <returns></returns>
        public static StoreProtocolEndpointLogic New(IStoreOf<IStoreProtocolHandler> handlerStore)
        {
            return new StoreProtocolEndpointLogic(handlerStore);
        }
        /// <summary>
        /// creates a new instance of StoreProtocolEndpointLogic with only a single handler, with no req/resp store encoding
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static StoreProtocolEndpointLogic NewPlaintextSingleHandler(IStoreProtocolHandler handler)
        {
            var store = NaturalInMemoryStore.New().IsOf<IStoreProtocolHandler>();
            store.SaveItem(handler);
            var logic = new StoreProtocolEndpointLogic(store);
            return logic;
        }

        #endregion
    }
}
