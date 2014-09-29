using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Store.Decorations.Identity
{
    /*  
     * 
     * 
     */ 

    /// <summary>
    /// record that is kept by a store about another store that it has handshaked with.
    /// </summary>
    public class StoreIdentityRecord : IHasId<string>
    {

        #region IHasId
        public string Id
        {
            get;
            private set;
        }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        public string ExternalKey { get; set; }
        public string Secret { get; set; }
        public string Crypto
    }

             /*          Store Identity Record (SIR):
                            -RUS SID 
         *                  -Key - a Guid/difficult to guess string id
         *                  -Secret - a randomly generated string
         *                  -Symmetric Crypto Key 
         *                  -RUS XSIR Store connection
         *                  -Expiry
         *                  
             *          Exportable Store Identity Record (XSIR):    
         *                  -Key (matches the SIR Key)
         *                  -EncryptedSecret (generated with SIR crypto key)
         */ 
}
