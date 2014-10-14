using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Thingness;
using Decoratid.Utils;
using ServiceStack.Text;
using Decoratid.Extensions;
using CuttingEdge.Conditions;
using Decoratid.Thingness.File;

namespace Decoratid.Crypto
{
    /// <summary>
    /// "Locked File" that stores a cipher pair (serialized).  It is plaintext.
    /// </summary>
    public class CipherPairFile : DisposableBase
    {
        //TODO: change this implementation to be a StoreOfA<SecureString>.  explore that whole StoreOfA area

        #region Ctor
        /// <summary>
        /// this will load the cipher pair (json serialized) from file
        /// </summary>
        /// <param name="filePath"></param>
        public CipherPairFile(string filePath)
            : base()
        {
            this.LockedFile = new LockedFile(filePath);

            this.Load();
        }
        /// <summary>
        /// this will write the cipher pair (json serialized) to file
        /// </summary>
        /// <param name="filePath"></param>
        public CipherPairFile(string filePath, SymmetricCipherPair pair)
            : base()
        {
            Condition.Requires(pair).IsNotNull();
            this.LockedFile = new LockedFile(filePath);

            this.Save();
        }
        #endregion

        #region Properties
        public LockedFile LockedFile { get; protected set; }
        public SymmetricCipherPair Pair { get; protected set; }
        #endregion

        #region Methods
        /// <summary>
        /// loads the file and hydrates the cipher pair. expects a json serialized value
        /// </summary>
        private void Load()
        {
            var secureData = this.LockedFile.Read();
            var data = SecureStringUtil.ToInsecureString(secureData);
            this.Pair = JsonSerializer.DeserializeFromString<SymmetricCipherPair>(data);
            Condition.Requires(this.Pair).IsNotNull();
        }
        /// <summary>
        /// writes the cipher pair to file, serialized as json
        /// </summary>
        private void Save()
        {
            this.LockedFile.Write(this.Pair.JSONSerialize());
        }
        protected override void DisposeManaged()
        {
            this.LockedFile.Dispose();
        }
        #endregion

        #region Static Methods
        public static CipherPairFile CreateRandomKeyFile(string filePath)
        {
            var cp = SymmetricCipherPair.AutogenerateRijndael();
            CipherPairFile kf = new CipherPairFile(filePath, cp);
            kf = new CipherPairFile(filePath);
            return kf;
        }
        #endregion
    }
}
