using Decoratid.Idioms.Communicating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    public static class SSLUtil
    {
        /// <summary>
        /// runs "netsh http add sslcert.."
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="ep"></param>
        public static void WireSSLCertToHttpSys(System.Security.Cryptography.X509Certificates.X509Certificate cert, EndPoint ep)
        {
            X509Certificate2 cert2 = new X509Certificate2(cert);
            var certHash = cert2.Thumbprint;
            var appid = EnvironmentUtil.GetRunningAssemblyGUID();

            string template = Environment.SystemDirectory + "\\netsh http add sslcert ipport={0}:{1} certhash={2} appid={3}";
            var cmd = string.Format(template, ep.IPAddress.ToString(), ep.Port, certHash, "{" + appid + "}");
            ProcessUtil.Do(cmd);
        }
        /// <summary>
        /// runs "netsh http add sslcert.."
        /// </summary>
        public static void WireSSLCertToHttpSys(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, EndPoint ep)
        {
            var certHash = cert.Thumbprint;
            var appid = EnvironmentUtil.GetRunningAssemblyGUID();

            string template = Environment.SystemDirectory + "\\netsh http add sslcert ipport={0}:{1} certhash={2} appid={3}";
            var cmd = string.Format(template, ep.IPAddress.ToString(), ep.Port, certHash, "{" + appid + "}");
            ProcessUtil.Do(cmd);
        }
    }
}
