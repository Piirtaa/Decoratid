using CuttingEdge.Conditions;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.Encrypting
{

    [Serializable]
    public class SymmetricCipherPair : CipherPair
    {
        static StringGenerator _stringGen = new StringGenerator();

        #region Ctor
        public SymmetricCipherPair(byte[] cipher, byte[] iv, SymmetricAlgorithm alg)
            : base(cipher, iv)
        {
            Condition.Requires(alg).IsNotNull();
            this.AlgorithmType = alg.GetType();
        }
        #endregion

        #region Properties
        public Type AlgorithmType { get; set; }
        public int KeySize { get { return Cipher.Length * 8; } } //should be 8 times the cipher
        #endregion

        #region Symmetric Encryption Methods
        public string Encrypt(string value)
        {
            SymmetricAlgorithm algorithm = Activator.CreateInstance(this.AlgorithmType) as SymmetricAlgorithm;
            algorithm.KeySize = this.KeySize;

            string rv = null;

            using (ICryptoTransform transform = algorithm.CreateEncryptor(this.Cipher, this.IV))
            {
                using (MemoryStream buffer = new MemoryStream())
                {
                    using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                        {
                            writer.Write(value);
                        }
                    }

                    rv = Convert.ToBase64String(buffer.ToArray());
                }
            }
            return rv;
        }

        public string Decrypt(string text)
        {
            SymmetricAlgorithm algorithm = Activator.CreateInstance(this.AlgorithmType) as SymmetricAlgorithm;
            algorithm.KeySize = this.KeySize;

            string rv = null;

            using (ICryptoTransform transform = algorithm.CreateDecryptor(this.Cipher, this.IV))
            {
                using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
                {
                    using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                        {
                            rv = reader.ReadToEnd();
                        }
                    }
                }
            }
            return rv;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// given a symmetric crypto alg, generates seed data with which to generate a key and IV.  Uses the alg's 
        /// max key size
        /// </summary>
        /// <param name="algorithm"></param>
        /// <returns></returns>
        public static SymmetricCipherPair Autogenerate(SymmetricAlgorithm algorithm)
        {
            var pwText = _stringGen.Generate(48, 64);
            var ivText = _stringGen.Generate(48, 64);

            //set to the max keysize of the alg
            algorithm.KeySize = algorithm.LegalKeySizes.Last().MaxSize;
            return GenerateKeyIV(algorithm, pwText, ivText);
        }
        /// <summary>
        /// given a symmetric crypto alg and some seed data, generates a key and IV 
        /// </summary>
        /// <param name="algorithm"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static SymmetricCipherPair GenerateKeyIV(SymmetricAlgorithm algorithm, string password, string salt)
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));

            byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);

            SymmetricCipherPair cp = new SymmetricCipherPair(rgbKey, rgbIV, algorithm);
            return cp;
        }
        #endregion

        #region Provider-specific Helpers
        public static SymmetricCipherPair AutogenerateRijndael()
        {
            return Autogenerate(new RijndaelManaged());
        }
        public static SymmetricCipherPair AutogenerateTripleDES()
        {
            return Autogenerate(new TripleDESCryptoServiceProvider());
        }
        public static SymmetricCipherPair AutogenerateAES()
        {
            return Autogenerate(new AesManaged());
        }
        public static SymmetricCipherPair GenerateRijndaelKeyIV(string password, string salt)
        {
            var alg = new RijndaelManaged();
            alg.KeySize = alg.LegalKeySizes.Last().MaxSize;
            return GenerateKeyIV(alg, password, salt);
        }
        public static SymmetricCipherPair GenerateTripleDESKeyIV(string password, string salt)
        {
            var alg = new TripleDESCryptoServiceProvider();
            alg.KeySize = alg.LegalKeySizes.Last().MaxSize;
            return GenerateKeyIV(alg, password, salt);
        }
        public static SymmetricCipherPair GenerateAESKeyIV(string password, string salt)
        {
            var alg = new AesManaged();
            alg.KeySize = alg.LegalKeySizes.Last().MaxSize;
            return GenerateKeyIV(alg, password, salt);
        }
        #endregion
    }

    public static partial class Extensions
    {
        public static Func<string, string> GetSymmetricEncodingStrategy(this SymmetricCipherPair cp)
        {
            return (x) => { return cp.Encrypt(x); };
        }

        public static Func<string, string> GetSymmetricDecodingStrategy(this SymmetricCipherPair cp)
        {
            return (x) => { return cp.Decrypt(x); };
        }
    }
}
