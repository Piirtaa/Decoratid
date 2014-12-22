using Decoratid.Core.Logical;
using Decoratid.Idioms.Testing;
using System.Text;
using System.Net.Http;
using System;
using HttpServer;
using HttpListener = HttpServer.HttpListener;

namespace Decoratid.Idioms.Communicating.HTTPing
{
    public class HTTPSHostTest : TestOf<Nothing>
    {
        public HTTPSHostTest()
            : base(LogicOf<Nothing>.New((x) =>
            {
                var ep = EndPoint.NewFreeLocalEndPointSpecifyingPort(80);
                var host = HTTPSHost.New(ep,

                (HttpServer.IHttpClientContext context, HttpServer.IHttpRequest req) =>
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

                });
                host.Initialize();
                host.Start();

                string uri = "https://" + ep.IPAddress + ":" + ep.Port;

                Console.ReadLine();

                //now hit it with some requests
                using (var client = new HttpClient())
                {
                    var getdat = HTTPClient.Get(client, uri);
                    HTTPClient.Delete(client, uri);
                    var postdat = HTTPClient.Post(client, new StringContent("yo"), uri);
                    var putdat = HTTPClient.Put(client, new StringContent("yo"), uri);
                }
            }))
        {
        }
    }
}
