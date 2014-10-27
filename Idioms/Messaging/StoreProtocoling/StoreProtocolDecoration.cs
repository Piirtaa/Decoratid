using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Logical;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Communicating;
using Decoratid.Idioms.ObjectGraphing.Values;
using Decoratid.Storidioms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.ValueOfing;

namespace Decoratid.Idioms.Messaging.StoreProtocoling
{
    /// <summary>
    /// decorates the host by giving it a store protocol implementation
    /// </summary>
    public class StoreProtocolDecoration : DecoratedEndPointHostBase, IHasStoreProtocolLogic
    {
        #region Ctor
        public StoreProtocolDecoration(IEndPointHost decorated, LogicOf<Tuple<IStore, IStore>> storeProtocolLogic, ValueManagerChainOfResponsibility valueManager = null)
            : base(decorated)
        {
            Condition.Requires(storeProtocolLogic).IsNotNull();
            this.StoreProtocolLogic = storeProtocolLogic;
            if (valueManager == null)
            {
                this.ValueManager = ValueManagerChainOfResponsibility.NewDefault();
            }
            else
            {
                this.ValueManager = valueManager;
            }

            //replace the logic
            this.Logic = LogicOfTo<string, string>.New((request) =>
            {
                return this.HandleStoreProtocolRequest(request);
            });
        }
        #endregion

        #region IHasStoreProtocolLogic
        public LogicOf<Tuple<IStore, IStore>> StoreProtocolLogic { get; private set; }
        public ValueManagerChainOfResponsibility ValueManager { get; private set; }
        #endregion

        #region Fluent Methods
        public void HasStoreProtocolLogic(LogicOf<Tuple<IStore, IStore>> logic)
        {
            Condition.Requires(logic).IsNotNull();
            this.StoreProtocolLogic = logic;
        }
        #endregion

        #region Helpers
        private string HandleStoreProtocolRequest(string request)
        {
            //decode the request store
            var requestStore = StoreSerializer.DeserializeStore(request, this.ValueManager);
            Condition.Requires(requestStore).IsNotNull();

            var responseStore = NaturalInMemoryStore.New();
            Tuple<IStore,IStore> uow = new Tuple<IStore,IStore>(requestStore, responseStore);

            this.StoreProtocolLogic.CloneAndPerform(uow.AsNaturalValue());

            //encode the response
            var responseText = StoreSerializer.SerializeStore(uow.Item2, this.ValueManager);
            return responseText;
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IEndPointHost> ApplyThisDecorationTo(IEndPointHost thing)
        {
            return new StoreProtocolDecoration(thing, this.StoreProtocolLogic);
        }
        #endregion

        #region Fluent Static
        public static StoreProtocolDecoration New(IEndPointHost decorated, LogicOf<Tuple<IStore, IStore>> storeProtocolLogic, ValueManagerChainOfResponsibility valueManager = null)
        {
            return new StoreProtocolDecoration(decorated, storeProtocolLogic, valueManager);
        }
        #endregion
    }

    public static class StoreProtocolDecorationExtensions
    {
        public static StoreProtocolDecoration StoreProtocoling(this IEndPointHost decorated, LogicOf<Tuple<IStore, IStore>> storeProtocolLogic, ValueManagerChainOfResponsibility valueManager = null)
        {
            var rv = new StoreProtocolDecoration(decorated, storeProtocolLogic, valueManager);
            return rv;
        }
    }
}
