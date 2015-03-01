using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Idioms.TokenParsing.HasConstantValue;
using Decoratid.Idioms.TokenParsing.HasId;
using Decoratid.Idioms.TokenParsing.HasPredecessor;
using Decoratid.Idioms.TokenParsing.HasPrefix;
using Decoratid.Idioms.TokenParsing.HasRouting;
using Decoratid.Idioms.TokenParsing.HasValidation;
using Decoratid.Idioms.TokenParsing.HasStartEnd;
using Decoratid.Idioms.TokenParsing.HasSuccessor;
using Decoratid.Idioms.TokenParsing.HasSuffix;
using Decoratid.Idioms.TokenParsing.HasTokenizerId;
using Decoratid.Idioms.TokenParsing.HasImplementation;
using Decoratid.Core.Conditional.Of;
using Decoratid.Core;
using Decoratid.Idioms.TokenParsing.CommandLine.Lexing;
using CuttingEdge.Conditions;
using Decoratid.Idioms.Ness;
using Decoratid.Storidioms.StoreOf;
using Decoratid.Core.Storing;

namespace Decoratid.Idioms.TokenParsing.CommandLine
{
    /// <summary>
    /// contains all the configurable elements of a Command Line parser
    /// </summary>
    public class CLConfig 
    {
        #region Ctor
        public CLConfig(NessManager nessManager = null, IStoreOf<NamedNaturalInMemoryStore> storeOfStores = null)
        {
            if (storeOfStores == null)
            {
                this.StoreOfStores = NaturalInMemoryStore.New().IsOf<NamedNaturalInMemoryStore>();

            }
            else
            {
                this.StoreOfStores = storeOfStores;
            }
            if (nessManager == null)
            {
                this.NessManager = NessManager.New();
            }
            else
            {
                this.NessManager = nessManager;
            }
        }
        #endregion

        #region Fluent Static
        public static CLConfig New(NessManager nessManager = null, IStoreOf<NamedNaturalInMemoryStore> storeOfStores = null)
        {
            return new CLConfig(nessManager, storeOfStores);
        }
        #endregion

        #region Properties
        public IStoreOf<NamedNaturalInMemoryStore> StoreOfStores { get; private set; }
        public NessManager NessManager { get; private set; }
        #endregion

      
    }
}
