using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using ServiceStack.Text;
using Decoratid.Extensions;
using System.IO;
using Decoratid.Idioms.Storing.Decorations;
using Decoratid.Idioms.Storing.Decorations.StreamBacked;
using Decoratid.Idioms.Storing.Core;
using Decoratid.Crypto;
using Decoratid.Idioms.Core.Conditional;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Idioms.Storing.Products
{
    /// <summary>
    /// An in-memory store that backs to a file
    /// </summary>
    public class FileStore : DecoratedStoreBase, IStore
    {
        #region Declarations
        protected readonly object _stateLock = new object();

        protected JsonObject _JSONObject = null;
        #endregion

        #region Ctor
        public FileStore(string filePath, ICondition backingCondition = null)
            : base(
            NaturalInMemoryStore.New()
            .DecorateWithBackingFile(filePath)
            )
        {
        }
        #endregion

        #region Properties
        public string FilePath { get { return this.FindDecoratorOf<BackingFileDecoration>(true).FilePath; } }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var dec = store.DecorateWithBackingFile(this.FilePath);
            return dec;
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
    /// <summary>
    /// An encoded in memory store that backs to a file that only the creating thread can access
    /// </summary>
    public class LockedDownFileStore : DecoratedStoreBase, IStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public LockedDownFileStore(string filePath, ICondition backingCondition = null)
            : base(
            NaturalInMemoryStore.New()
            .DecorateWithBackingLockedFile(filePath, backingCondition)
            )
        {
        }
        #endregion

        #region Properties
        public string FilePath { get { return this.FindDecoratorOf<BackingLockedFileDecoration>(true).FilePath; } }
        public ICondition BackingCondition { get { return this.FindDecoratorOf<BackingLockedFileDecoration>(true).BackingCondition; } }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            var dec = store.DecorateWithBackingLockedFile(this.FilePath, this.BackingCondition);
            return dec;
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
    /// <summary>
    /// An encrypted in memory store that backs to an encrypted, locked down file.
    /// </summary>
    public class EncryptedLockedDownFileStore : DecoratedStoreBase, IStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public EncryptedLockedDownFileStore(SymmetricCipherPair keyAndEntropy, string filePath, ICondition backingCondition = null)
            : base(
            NaturalInMemoryStore.New()
            .DecorateWithBackingLockedFile(filePath, backingCondition)
            .DecorateWithBackingEncryption(keyAndEntropy)
            )
        {
        }
        #endregion

        #region Properties
        public string FilePath { get { return this.FindDecoratorOf<BackingLockedFileDecoration>(true).FilePath; } }
        public ICondition BackingCondition { get { return this.FindDecoratorOf<BackingLockedFileDecoration>(true).BackingCondition; } }
        public SymmetricCipherPair KeyAndEntropy { get { return this.FindDecoratorOf<BackingEncryptedStreamDecoration>(true).KeyAndEntropy as SymmetricCipherPair; } }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            return store.DecorateWithBackingLockedFile(this.FilePath, this.BackingCondition)
                .DecorateWithBackingEncryption(this.KeyAndEntropy);
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
