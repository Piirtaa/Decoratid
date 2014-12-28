using CuttingEdge.Conditions;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Serviceable;
using Decoratid.Idioms.Testing;
using Decoratid.Utils;
using Mono.Net;
using Mono.Security.Authenticode;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Decoratid.Idioms.Communicating.HTTPing
{
    public class HTTPSHostTest : TestOf<Nothing>
    {
        public HTTPSHostTest()
            : base(LogicOf<Nothing>.New((x) =>
            {
                var ep = EndPoint.NewFreeLocalEndPointSpecifyingPort(80);

                var host = HTTPSHost.New(ep,
                (req) =>
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<HTML><BODY>");
                    var lines = HTTPSHost.DumpHTTPRequest(req);
                    foreach (var eachline in lines)
                    {
                        sb.AppendLine(eachline + "<BR>");
                    }
                    sb.AppendLine("</BODY></HTML>");

                    return sb.ToString();

                }, "test/");

                using (host)
                {
                    host.Initialize();
                    host.Start();

                    string uri = "https://" + ep.IPAddress + ":" + ep.Port;

                    //Console.ReadLine();

                    ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

                    //using (WebClient webClient = new WebClient())
                    //{
                    //    var stream = webClient.OpenRead(uri);
                    //    using (StreamReader sr = new StreamReader(stream))
                    //    {
                    //        var page = sr.ReadToEnd();
                    //    }
                    //}

                    //now hit it with some requests
                    using (var client = new HttpClient())
                    {
                        var getdat = HTTPClient.Get(client, uri);
                        HTTPClient.Delete(client, uri);
                        var postdat = HTTPClient.Post(client, new StringContent("yo"), uri);
                        var putdat = HTTPClient.Put(client, new StringContent("yo"), uri);
                    }
                }
            }))
        {
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            Console.WriteLine("X509Certificate [{0}] Policy Error: '{1}'",
                certificate.Subject,
                sslPolicyErrors.ToString());

            return false;
        }
    }


}
