using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Encrypting
{
    /// <summary>
    /// uses the DPAPI to encrypt data
    /// </summary>
    public static class ProtectedDataUtil
    {
        public static CipherPair Encrypt(string text)
        {
            // Data to protect. Convert a string to a byte[] using Encoding.UTF8.GetBytes().
            byte[] plaintext = System.Text.Encoding.Unicode.GetBytes(text);

            // Generate additional entropy (will be used as the Initialization vector)
            byte[] entropy = CryptoHelper.GenerateRandomByteArray(20);

            byte[] ciphertext = ProtectedData.Protect(plaintext, entropy,
                DataProtectionScope.CurrentUser);

            CipherPair cp = new CipherPair(ciphertext, entropy);
            return cp;
        }

        public static string Decrypt(CipherPair cp)
        {
            byte[] plaintext = ProtectedData.Unprotect(cp.Cipher, cp.IV, DataProtectionScope.CurrentUser);
            string val = System.Text.Encoding.Unicode.GetString(plaintext);
            return val;
        }
    }
}
