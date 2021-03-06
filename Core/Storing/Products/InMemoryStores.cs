﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Core.Conditional;
using Decoratid.Crypto;
using Decoratid.Core.Storing;
using Decoratid.Core.Storing.Decorations;
using Decoratid.Core.Storing.Decorations.StoreOf;
using Decoratid.Core.Storing.Decorations.StreamBacked;
using Decoratid.Thingness;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Core.Storing.Products
{
    /// <summary>
    /// an in memory store of the given type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InMemoryStoreOf<T> : DecoratedStoreBase, IStore
        where T: IHasId
    {
        public InMemoryStoreOf()
        :base(NaturalInMemoryStore.New().DecorateWithIsOf<T>())
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
