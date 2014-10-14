using CuttingEdge.Conditions;
using Decoratid.Utils;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Decoratid.Idioms.Encrypting
{
    /// <summary>
    /// placeholder class for cipher and iv byte arrays
    /// </summary>
    [Serializable]
    public class CipherPair
    {
        #region Ctor
        public CipherPair()
        {
        }

        public CipherPair(byte[] cipher, byte[] iv)
        {
            this.Cipher = cipher;
            this.IV = iv;
        }
        #endregion

        #region Properties
        public Byte[] Cipher { get; set; }
        public Byte[] IV { get; set; }
        public int KeySize { get { return Cipher.Length * 8; } } //should be 8 times the cipher
        #endregion
    }

}
