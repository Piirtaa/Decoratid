using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Crypto
{
    /// <summary>
    /// singleton container of applevel keychain
    /// </summary>
    public class MyKeyChain : KeyChain
    {
        #region Declarations
        public const string KEYCHAIN_STORE_PATH = "AppKeyChain.txt";
        public const string KEYCHAIN_KEYFILE_PATH = "AppKeyFile.txt";

        private readonly object _stateLock = new object(); //the explicit object we thread lock on
        private static MyKeyChain _instance = new MyKeyChain();
        #endregion

        #region Ctor
        static MyKeyChain()
        {
        }
        private MyKeyChain(): base(KEYCHAIN_STORE_PATH, KEYCHAIN_KEYFILE_PATH)
        {

        }
        #endregion

        #region Properties
        public static MyKeyChain Instance { get { return _instance; } }
        #endregion
    }
}
