using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Thingness.Idioms.Store.Broker
{
    /// <summary>
    /// wraps a connection string with metadata to assist with brokering
    /// </summary>
    [Serializable]
    public class StoreConnection : IHasId<string>
    {
        #region Ctor
        public StoreConnection()
        {
        }
        #endregion

        #region IHasId
        /// <summary>
        /// the unique identifier of this connection
        /// </summary>
        public string Id { get; set; }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Properties
        /// <summary>
        /// the type of the connection.  Eg. Mongo, SQL Server
        /// </summary>
        public string ConnectionType { get; set; }
        /// <summary>
        /// the actual connection string 
        /// </summary>
        public string ConnectionString { get; set; }
        #endregion
    }
}
