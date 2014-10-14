using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Core;
using System.Runtime.Serialization;
using Decoratid.Core.Decorating;
using Decoratid.Idioms.Stringing;

namespace Decoratid.Idioms.Encrypting
{
    [Serializable]
    public class EncryptedStringableDecoration : StringableDecorationBase, IHasSymmetricCipherPair
    {
        #region Ctor
        public EncryptedStringableDecoration(IStringable decorated, SymmetricCipherPair cipherPair)
            : base(decorated)
        {
            Condition.Requires(cipherPair).IsNotNull();
            this.CipherPair = cipherPair;
        }
        #endregion

        #region ISerializable
        protected EncryptedStringableDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region IHasSymmetricCipherPair
        public SymmetricCipherPair CipherPair {get; private set;}
        #endregion

        #region Overrides
        public override string GetValue()
        {
            //get the core value
            var val = this.Decorated.GetValue();
            //encrypt it
            var eVal = this.CipherPair.Encrypt(val);

            //return the encrypted value
            return eVal;
        }
        public override void Parse(string text)
        {
            //decrypt provided value
            var val = this.CipherPair.Decrypt(text);
            //parse it 
            this.Decorated.Parse(val);
        }
        public override IDecorationOf<IStringable> ApplyThisDecorationTo(IStringable thing)
        {
            return new EncryptedStringableDecoration(thing, this.CipherPair);
        }
        #endregion

    }

    public static class EncryptedStringableDecorationExtensions
    {
        public static EncryptedStringableDecoration Encrypt(this IStringable thing, SymmetricCipherPair cipherPair)
        {
            Condition.Requires(thing).IsNotNull();
            return new EncryptedStringableDecoration(thing, cipherPair);
        }
    }
}
