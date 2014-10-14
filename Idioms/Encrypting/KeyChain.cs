using CuttingEdge.Conditions;
using Decoratid.Core;
using Decoratid.Core.Identifying;
using Decoratid.Storidioms.StoreOf;
using System;
using System.IO;

namespace Decoratid.Idioms.Encrypting
{
    /// <summary>
    /// An item (eg. id and cipherpair) in a keychain
    /// </summary>
    public class KeyChainItem : IHasId<string>
    {
        #region Ctor
        public KeyChainItem()
        {
        }
        public KeyChainItem(string id, SymmetricCipherPair keyAndEntropy)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            Condition.Requires(keyAndEntropy).IsNotNull();

            this.Id = id;
            this.KeyAndEntropy = keyAndEntropy;
        }
        #endregion

        #region IHasId
        /// <summary>
        /// the unique identifier of this connection
        /// </summary>
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Properties
        public SymmetricCipherPair KeyAndEntropy { get; set; }
        #endregion
    }

    /// <summary>
    /// a container of keys for symmetric encryption that is backed by an encrypted locked file store, which itself
    /// is reads the file's encryption key from a CipherPairFile.  In practice the CipherPairFile could be on a removable
    /// USB, minimizing exposure of the cipher pair - Once the CipherPairFile is loaded the usb can be removed and our
    /// cipherpair is held in a secure string in memory.
    /// </summary>
    public class KeyChain : DisposableBase
    {
        #region Ctor
        /// <summary>
        /// specify the file that will store the keys (encrypted), as well as the keyfile used that has the symmetric key and IV
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="cipherPairFilePath"></param>
        public KeyChain(string filePath, string cipherPairFilePath)
        {
            Condition.Requires(filePath).IsNotNullOrEmpty();
            Condition.Requires(cipherPairFilePath).IsNotNullOrEmpty();

            this.FilePath = filePath;
            this.InitCipherPairFile(cipherPairFilePath);
            this.KeyStore = new EncryptedLockedDownFileStore(this.CipherPairFile.Pair, filePath).DecorateWithExactlyIsOf<KeyChainItem>();
        }
        #endregion

        #region Properties
        protected IStoreOfExactly<KeyChainItem> KeyStore { get; set; }
        protected string FilePath { get; set; }
        protected CipherPairFile CipherPairFile { get; set; }
        #endregion

        #region Overrides
        protected override void DisposeManaged()
        {
            this.CipherPairFile.Dispose();
            ((IDisposable)this.KeyStore).Dispose();
            base.DisposeManaged();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// loads the cipher pair file and initializes cipher pair.  if the file doesn't exist, one is generated and saved at the 
        /// specified path.
        /// </summary>
        /// <param name="cipherPairFilePath"></param>
        private void InitCipherPairFile(string cipherPairFilePath)
        {
            //if the keyfile exists, we use that.  if not we create one
            if (File.Exists(cipherPairFilePath))
            {
                this.CipherPairFile = new CipherPairFile(cipherPairFilePath);
            }
            else
            {
                this.CipherPairFile = CipherPairFile.CreateRandomKeyFile(cipherPairFilePath);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// looks in the keychain for the entry by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SymmetricCipherPair GetKeyAndEntropy(string id)
        {
            var val = this.KeyStore.GetById(id);
            if (val == null)
                return null;

            return val.KeyAndEntropy;
        }
        /// <summary>
        /// generates a new key of the provided size, and stores it at the given id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="byteSize"></param>
        /// <returns></returns>
        public KeyChainItem AddNewRandomKey(string id)
        {
            KeyChainItem returnValue = new KeyChainItem(id, SymmetricCipherPair.AutogenerateRijndael());
            this.KeyStore.SaveItem(returnValue);

            return returnValue;
        }
        #endregion

        #region Encryption Methods
        public string Encrypt(string id, string text)
        {
            var key = this.GetKeyAndEntropy(id);
            Condition.Requires(key).IsNotNull("No entry found for the provided id");
            return key.Encrypt(text);
        }
        public string Decrypt(string id, string text)
        {
            var key = this.GetKeyAndEntropy(id);
            Condition.Requires(key).IsNotNull("No entry found for the provided id");
            return key.Decrypt(text);
        }
        #endregion
    }
}
