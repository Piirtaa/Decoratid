using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Store.Decorations.Common;
using Sandbox.Extensions;
using Sandbox.Conditions;

namespace Sandbox.Store.Decorations.Security
{


    /// <summary>
    /// marker interface indicating a store contains a user's info.  It essentially encapsulates the identity of something. 
    /// Implements IHasId - equivalence between IUserInfoStores via IHasId is used to determine if 2 users are the same.
    /// </summary>
    public interface IUserInfoStore : ISearchableStore, IHasId
    {
        //is really a query you want to ask of a store to determine if it has access. 
        //store needs to know if a store is itself.  it needs a query only it knows that should return a result from the store
        /**
         * the idea is - a user credential being a custom store from which to query for info to sufficiently resolve one's access requirements.
         *          these access rules are themselves stored in the granting user store.
         *          
         * 
         * RequestingUserStore (RUS) asks GrantingUserStore (GUS) for access to item X
         *  GUS examines all rules for item X (both item specific rules and general rules)
         *  GUS orders all rules by dependency
         *  GUS evaluates each rule...if all true, RUS gets access
         *      To evaluate a rule involving RUS, GUS will attempt to run queries against RUS
         *         
         *      For example:
         *      
         *          Let's say I'm GUS and I've set up a rule that says RUS has access to X
         *          - I need to be able to determine RUS is actually RUS.  I've kept a query
         *          I used to ask "the identity question" of RUS.  Presumably RUS knows
         *          to not messup this identity question data - indeed it would be ideal if
         *          RUS allowed others, such as GUS, to write to their store - if only in a particular
         *          reserved area.  In this reserved area, let's call it the ReservedCredentialHive - RCH,
         *          RUS will write data only it will know.  Presumably it will be so hard to find the key to this 
         *          data that the data can remain somewhat fresh.  (ALL RCH ENTRIES SHOULD BE TOKENS WITH EXPIRY).
         *          Anyway, this is somewhat moot, as the process to re-identify a store is the same, and has 
         *          known triggers.
         *          
         *          To reiterate:  There is a process wherein a GUS writes records to RUS stores at the same time
         *          it is writing a rule that restricts RUS.  Thus there needs to be a way for 2 stores to communicate
         *          to one another - a central registry / discovery service / identity store / bus.  GUS needs to 
         *          send a message to RUS saying "store this cred data" - it has to know how to connect to RUS in other words.
         *          This connection info / connecting string has to be avail.
         *          
         *          Therefore THERE MUST BE a store connection string / factory discovery service so entries can be exchanged.
         *          
         *          GUS needs OWNER rights over records it writes in RUS's RCH.  To ENFORCE this GUS will
         *          symmetrically encrypt (using its internal key) their own copy of the RCH - so anyone who 
         *          sees the data will have no idea what it is.  Guid - encrypted key/value pair.
         *          
         *          GUS writes to RUS' RCH
         *          when RUS requests something, GUS sees RUS's public Id, finds its entry in RUS' RCH, and queries
         *          RUS for the matching data.
         *          
         *          Terminology:
         *          IIdentifiableStore : IStore
         *              -string StoreId
         *              -IStore XSIRStore, a publicly accessible store
         *              
         *              Notes:
         *                  SIR records are stored here as SIR records (and inject their sec rules)
         *                  
         *          GUS/RUS - granting / requesting user store : IIdentifiableStore
         *          
         *          Data Structures:
         *          Store Identity Record (SIR):
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
         * 

         *          
         *          Where are SIR and XSIR stored?
         *              In an "Identifiable Store", SIR is stored in the SIRS (SIR store) (which can also be the store itself)elf XIR is stored in the XIRS(XIR Store)
         *              .  To ensure a SIR item is stored securely (ie. only the creating
         *              store can see this data), the SIR data structure itself injects rules (grant access to store, deny access
         *              to everyone, in that dependency order).
         * 
         * Steps to generate an , 
         *          
         * 
         *          1.  GUS gets RUS's SID from DS (Discovery Store)
         *          2.  GUS gets RUS's PAH (Public Access Hive) connection from DS
         *          3.a)    GUS creates "auth secret" for RUS:
         *                  RUS's SID, Secret, Secret's Key, Symmetric Crypto Key, Expiry, RUS PAH connection
         *            b)    GUS saves "auth secret" in GUS's ASS (Auth Secret Store)
         *            c)    GUS saves "PAH secret" (Secret's Key, Encrypted Secret) in RUS's PAH
         *            
         *              Note:  at no point in the communication (ie. the Save in RUS's PAH) is the secret actually
         *              being sent over.  Rather it's completely encrypted.
         *          4.
         *              
         *          Steps to test an access rule:
         *          1.  RUS requests X from GUS
         *          2.  GUS looks for "auth secret" for RUS in GUS's ASS (Auth Secret Store) with RUS's SID
         *              -Can't find?  Kack!!         
         *          3.  GUS queries RUS's PAH for the "PAH secret" (should do this in one consistent way)
         *              -Can't find?  Kack!!
         *              -Found it?  
         *                  We have confirmed RUS.
         *                      Regenerate Identity
         *          
         *          
         *          
         *          
         *          
         *             
         *  
         * 
         * 
         */ 
    }

    /// <summary>
    /// extends IStoredItemAuditPoint to include the actor information (eg. who did the action).  This relates to the IHasStoredItemAuditTrail
    /// stuff.
    /// </summary>
    public interface ISecuredStoredItemAuditPoint : IStoredItemAuditPoint
    {
        IUserInfoStore Actor { get; }
    }

    /// <summary>
    /// a secure store is one that is sent credentials (in the form of a IUserStore)to do any of the regular store activities
    /// </summary>
    public interface ISecuredStore : IDecoratedStore
    {
        /// <summary>
        /// the store that contains instances of StoreAccessRules that this store is decorated by 
        /// </summary>
        IStore RuleStore { get; }
    }


}
