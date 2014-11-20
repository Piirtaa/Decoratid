using CuttingEdge.Conditions;
using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Core.Storing
{
    public class NamedNaturalInMemoryStore : NaturalInMemoryStore, IHasId<string>
    {
        #region Ctor
        public NamedNaturalInMemoryStore(string id)
            : base()
        {
            Condition.Requires(id).IsNotNullOrEmpty();
            this.Id = id;
        }
        #endregion

        #region IHasId
        public string Id { get; set; }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Static Fluent Methods
        public static NamedNaturalInMemoryStore New(string id)
        {
            return new NamedNaturalInMemoryStore(id);
        }
        #endregion
    }
}
