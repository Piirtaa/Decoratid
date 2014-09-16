//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading.Tasks;
//using CuttingEdge.Conditions;
//using Decoratid.Crypto;
//using Decoratid.Idioms.Logics;

//namespace Decoratid.Idioms.Storing.Core
//{
//    [Serializable]
//    public class EncryptedInMemoryStore : EncodedInMemoryStore//, ISerializable
//    {
//        public EncryptedInMemoryStore(SymmetricCipherPair cp)
//            : base()
//        {
//            Condition.Requires(cp).IsNotNull();
//            this.KeyAndEntropy = cp;

//            //wire up the strategies
//            this.EncodeWriteDataStrategy = LogicOfTo<string, string>.New((x) => { return this.KeyAndEntropy.Encrypt(x); });
//            this.DecodeReadDataStrategy = LogicOfTo<string, string>.New((x) => { return this.KeyAndEntropy.Decrypt(x);});
//        }

//        //#region ISerializable
//        //protected EncryptedInMemoryStore(SerializationInfo info, StreamingContext context)
//        //    : base(info, context)
//        //{
//        //    //this._Core = (T)info.GetValue("_Core", typeof(T));
//        //    //this._Decorated = (T)info.GetValue("_Decorated", typeof(T));
//        //    this.KeyAndEntropy = (CipherPair)info.GetValue("_KeyAndEntropy", typeof(CipherPair));
//        //}
//        //public override void GetObjectData(SerializationInfo info, StreamingContext context)
//        //{
//        //    base.GetObjectData(info, context);
//        //    info.AddValue("_KeyAndEntropy", this.KeyAndEntropy, typeof(CipherPair));
//        //    //info.AddValue("_Core", this._Core, this._Core.GetType());
//        //    //info.AddValue("_Decorated", this._Decorated, this._Decorated.GetType());
//        //}
//        //#endregion

//        public SymmetricCipherPair KeyAndEntropy { get; protected set; }

//        #region Static Fluent Methods
//        public static EncryptedInMemoryStore New(SymmetricCipherPair cp)
//        {
//            return new EncryptedInMemoryStore(cp);
//        }
//        #endregion
//    }
//}
