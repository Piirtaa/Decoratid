﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Crypto;
using Decoratid.Thingness.Idioms.Store.CoreStores;
using Decoratid.Thingness.Idioms.Store.Decorations;
using Decoratid.Thingness.Idioms.Store.Decorations.StoreOf;
using Decoratid.Thingness.Idioms.Store.Decorations.StreamBacked;
using Decoratid.Thingness;
using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.ObjectGraph;

namespace Decoratid.Thingness.Idioms.Store.Implementations
{
    /// <summary>
    /// an in memory store of the given type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InMemoryStoreOf<T> : DecoratedStoreBase, IStore
        where T: IHasId
    {
        public InMemoryStoreOf()
        :base(InMemoryStore.New().DecorateWithIsOf<T>())
        {

        }


        #region IDecoratedStore
        /// <summary>
        /// throw not implemented exception on "decorated in-memory stores"
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return string.Empty;
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {

        }
        #endregion
    }

}
