using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Storing;
using Decoratid.Extensions;
using Decoratid.Core.Identifying;

namespace Decoratid.Idioms.Communicating
{
    /// <summary>
    /// an IP and port
    /// </summary>
    [Serializable]
    public class EndPoint : IHasId<string>
    {
        #region Ctor
        public EndPoint(IPAddress address, int port)
        {
            Condition.Requires(address).IsNotNull();
            this.IPAddress = address;
            this.Port = port;
        }
        #endregion

        #region Properties
        public IPAddress IPAddress { get; set; }
        private int _port;
        /// <summary>
        /// port number between 1 and 65535
        /// </summary>
        public int Port 
        { 
            get { return _port; } 
            set 
            { 
                Condition.Requires(value).IsGreaterOrEqual(1, "invalid port").IsLessOrEqual(65535, "invalid port");
                this._port = value;
            }
        }
        #endregion

        #region IHasId
        public string Id { get { return this.IPAddress.ToString() + ":" + this.Port.ToString(); } }
        object IHasId.Id { get { return this.Id; } }
        #endregion

        #region Static Methods
        public static EndPoint New(IPAddress address, int port)
        {
            return new EndPoint(address, port);
        }
        public static EndPoint NewFreeLoopbackEndPoint()
        {
            int port = NetUtil.GetFreeTcpPort(System.Net.IPAddress.Loopback);
            return new EndPoint(System.Net.IPAddress.Loopback, port);
        }
        public static EndPoint NewFreeLocalEndPoint()
        {
            var ip = NetUtil.GetLocalIPAddresses().FirstOrDefault();
            var port = NetUtil.GetFreeTcpPort(ip);
            return new EndPoint(ip, port);
        }
        public static EndPoint NewFreeLocalEndPoint(int adapterIndex)
        {
            var ip = NetUtil.GetLocalIPAddresses()[adapterIndex];
            var port = NetUtil.GetFreeTcpPort(ip);
            return new EndPoint(ip, port);
        }
        #endregion
    }
}
