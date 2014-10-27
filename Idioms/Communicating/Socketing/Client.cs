using CuttingEdge.Conditions;
using Decoratid.Core;
using System;
using System.Net.Sockets;
using System.Text;

namespace Decoratid.Idioms.Communicating.Socketing
{
    public class Client : DisposableBase, IEndPointClient
    {
        #region Ctor
        public Client(EndPoint ep)
            : base()
        {
            Condition.Requires(ep).IsNotNull();
            this.EndPoint = ep;
        }
        #endregion

        #region Client
        public EndPoint EndPoint { get; protected set; }
        public string Send(string data)
        {
            TcpClient client = null;
            StringBuilder received = new StringBuilder();

            try
            {
                client = new TcpClient();
                client.Connect(this.EndPoint.IPAddress, this.EndPoint.Port);
                
                // Get the bytes to send for the message
                byte[] bytes = Encoding.ASCII.GetBytes(data);

                // get the stream to talk to the server on
                using (NetworkStream ns = client.GetStream())
                {
                    ProtocolUtil.Write(ns, data);

                    var resp = ProtocolUtil.Read(ns);

                    return resp;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region Static Methods
        public static Client New(EndPoint ep)
        {
            Client client = new Client(ep);
            return client;
        }
        #endregion
    }
}
