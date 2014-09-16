using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Storidiom.Thingness.Idioms.Store
{
    /// <summary>
    /// Yeah, that's right.  A store that, itself, can be stored as a stored item.  Controls its own serialization.
    /// </summary>
    [Serializable]
    public class StoredStore : ContextualAsId<string, IStore>, ISerializable
    {
        #region Ctor
        public StoredStore(string id, IStore store)
        {
            Condition.Requires(id).IsNotNullOrEmpty();
        }
        #endregion

        #region ISerializable
        protected StoredStore(SerializationInfo info, StreamingContext context)
        {
           // StoreSerializationSurrogate.SetObjectDataHelper(this, info, context, null, null);
        }
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //StoreSerializationSurrogate.GetObjectDataHelper(this, info, context, null);
        }
        #endregion
    }
}
