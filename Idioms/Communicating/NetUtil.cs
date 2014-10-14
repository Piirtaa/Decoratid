using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Decoratid.Idioms.Communicating
{
    public static class NetUtil
    {
        public static string GetLocalMachineName()
        {
            return Dns.GetHostName();
        }
        public static List<IPAddress> GetLocalIPAddresses()
        {
            return GetIPAddresses(GetLocalMachineName());
        }
        public static List<IPAddress> GetIPAddresses(string hostName)
        {
            List<IPAddress> returnValue = new List<IPAddress>();

            IPHostEntry he = Dns.GetHostEntry(hostName);
            IPAddress[] addrs = he.AddressList;

            if (addrs != null)
            {
                foreach (IPAddress each in addrs)
                {
                    if (each.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        returnValue.Add(each);
                        break;
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Gets a free TCP port for a specific IP address.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <returns>The port number.</returns>
        public static int GetFreeTcpPort(IPAddress address)
        {
            TcpListener tcpListener = new TcpListener(address, 0);
            tcpListener.Start();
            int port = (tcpListener.LocalEndpoint as IPEndPoint).Port;
            tcpListener.Stop();
            return port;
        }

        /// <summary>
        /// Gets a free UDP port for a specific IP address.
        /// </summary>
        /// <param name="address">An IP address.</param>
        /// <returns>The port number.</returns>
        public static int GetFreeUdpPort(IPAddress address)
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                IPEndPoint endPoint = new IPEndPoint(address, 0);
                socket.Bind(endPoint);
                return (socket.LocalEndPoint as IPEndPoint).Port;
            }
        }

        /// <summary>
        /// Verifies if a TCP port is being used by a listener.
        /// </summary>
        /// <param name="address">IP address to verify.</param>
        /// <param name="port">A port number to verify.</param>
        /// <returns>True if the port is being used by a listener.</returns>
        public static bool IsTcpPortInUseByListener(IPAddress address, int port)
        {
            IPGlobalProperties globalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var list = globalProperties.GetActiveTcpListeners();
            return list.Any(a => a.Port == port && a.Address.Equals(address));
        }

        /// <summary>
        /// Verifies if a UDP port is being used by a listener.
        /// </summary>
        /// <param name="address">IP address to verify.</param>
        /// <param name="port">A port number to verify.</param>
        /// <returns>True if the port is being used by a listener.</returns>
        public static bool IsUdpPortInUseByListener(IPAddress address, int port)
        {
            IPGlobalProperties globalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var list = globalProperties.GetActiveUdpListeners();
            return list.Any(a => a.Port == port && a.Address.Equals(address));
        }
    }
}
