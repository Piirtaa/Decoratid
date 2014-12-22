using Decoratid.Core.Logical;
using Decoratid.Idioms.Testing;
using System.Text;
using System.Net.Http;
using System;

namespace Decoratid.Idioms.Communicating.HTTPing
{
    public class HTTPHostTest : TestOf<Nothing>
    {
        public HTTPHostTest()
            : base(LogicOf<Nothing>.New((x) =>
            {
                var ep = EndPoint.NewFreeLocalEndPointSpecifyingPort(80);
                var host = HTTPHost.New(ep, (req) =>
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<HTML><BODY>");
                    var lines = HTTPHost.DumpHTTPRequest(req);
                    foreach (var eachline in lines)
                    {
                        sb.AppendLine(eachline + "<BR>");
                    }
                    sb.AppendLine("</BODY></HTML>");

                    return sb.ToString();

                }, "test/");
                host.Initialize();
                host.Start();

                string uri = "http://" + ep.IPAddress + ":" + ep.Port + "/test/";

                //Console.ReadLine();

                //now hit it with some requests
                using (var client = new HttpClient())
                {
                    var getdat =HTTPClient.Get(client, uri);
                    HTTPClient.Delete(client, uri);
                    var postdat = HTTPClient.Post(client, new StringContent("yo"), uri);
                    var putdat = HTTPClient.Put(client, new StringContent("yo"), uri);
                }
            }))
        {
        }
    }

}
