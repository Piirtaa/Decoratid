using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Crypto;
using Decoratid.Extensions;
using Decoratid.Thingness;
using Decoratid.Utils;
using ServiceStack.Text;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph;
using Decoratid.Idioms.ObjectGraph.Values;

namespace Decoratid.Storidioms.StreamBacked
{
    /// <summary>
    /// a stream backed store that encrypts 
    /// </summary>
    public interface IEncryptedStreamBackedStore : IDecoratedStore, IDisposable
    {
        SymmetricCipherPair KeyAndEntropy { get; }
    }

    /// <summary>
    /// Decorates with encryption on the stream.  Must decorate a BackingStreamDecoration - as it injects encryption into
    /// the stream scrub hooks. Uses Rijndael256
    /// </summary>
    public class BackingEncryptedStreamDecoration : DecoratedStoreBase, IEncryptedStreamBackedStore, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        protected BackingEncryptedStreamDecoration() : base() { }
        public BackingEncryptedStreamDecoration(BackingStreamDecoration decorated, SymmetricCipherPair keyAndEntropy)
            : base(decorated)
        {
            Condition.Requires(keyAndEntropy).IsNotNull();
            this.KeyAndEntropy = keyAndEntropy;
            this.WireEncryptionStrategy();
        }
        #endregion

        #region Properties
        BackingStreamDecoration BackingDecorated { get { return this.Decorated as BackingStreamDecoration; } }
        public SymmetricCipherPair KeyAndEntropy { get; protected set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return new BackingEncryptedStreamDecoration(store as BackingStreamDecoration, this.KeyAndEntropy);
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<BackingEncryptedStreamDecoration>();
            hydrationMap.RegisterDefault("KeyAndEntropy", x => x.KeyAndEntropy, (x, y) => { x.KeyAndEntropy = y as SymmetricCipherPair; });
            return hydrationMap;
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return this.GetHydrationMap().DehydrateValue(this, uow);
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
            this.GetHydrationMap().HydrateValue(this, text, uow);
        }
        #endregion

        #region Methods
        /// <summary>
        /// injects encryption into the data scrubbers of the BackingDecoration
        /// </summary>
        private void WireEncryptionStrategy()
        {
            this.BackingDecorated.EncodeWriteDataStrategy = (x) => { return this.KeyAndEntropy.Encrypt(x); };
            this.BackingDecorated.DecodeReadDataStrategy = (x) => { return this.KeyAndEntropy.Decrypt(x); };
        }
        #endregion
    }
}
