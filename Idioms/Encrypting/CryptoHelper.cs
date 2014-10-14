using CuttingEdge.Conditions;
using Decoratid.Thingness;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Encrypting
{
    public static class CryptoHelper
    {
        #region Generation Helpers
        /// <summary>
        /// creates a crypto-grade random byte array of the given size
        /// </summary>
        /// <param name="byteSize"></param>
        /// <returns></returns>
        public static byte[] GenerateRandomByteArray(int byteSize)
        {
            byte[] entropy = new byte[byteSize];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }
            return entropy;
        }

        #endregion

    }
}
