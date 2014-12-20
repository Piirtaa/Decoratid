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

                    sb.AppendLine(req.RawUrl + "<BR>");
                    sb.AppendLine(req.HttpMethod + "<BR>");

                    //echo headers
                    sb.AppendLine("Headers<BR>");
                    foreach (var key in req.Headers.AllKeys)
                    {
                        var val = req.Headers[key];
                        sb.AppendLine(key + "&nbsp;&nbsp;" + val + "<BR>");
                    }

                    //echo req vars
                    sb.AppendLine("Query string<BR>");
                    foreach (var key in req.QueryString.AllKeys)
                    {
                        var val = req.QueryString[key];
                        sb.AppendLine(key + "&nbsp;&nbsp;" + val + "<BR>");
                    }

                    //echo post body
                    sb.AppendLine("Body<BR>");

                    var body = HTTPHost.GetRequestPostData(req);
                    sb.AppendLine(body + "<BR>");
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

    //    // Display the MIME types that can be used in the response. 
    //string[] types = request.AcceptTypes;
    //if (types != null)
    //{
    //    Console.WriteLine("Acceptable MIME types:");
    //    foreach (string s in types)
    //    {
    //        Console.WriteLine(s);
    //    }
    //}
    //// Display the language preferences for the response.
    //types = request.UserLanguages;
    //if (types != null)
    //{
    //    Console.WriteLine("Acceptable natural languages:");
    //    foreach (string l in types)
    //    {
    //        Console.WriteLine(l);
    //    }
    //}

    //// Display the URL used by the client.
    //Console.WriteLine("URL: {0}", request.Url.OriginalString);
    //Console.WriteLine("Raw URL: {0}", request.RawUrl);
    //Console.WriteLine("Query: {0}", request.QueryString);

    //// Display the referring URI.
    //Console.WriteLine("Referred by: {0}", request.UrlReferrer);

    ////Display the HTTP method.
    //Console.WriteLine("HTTP Method: {0}", request.HttpMethod);
    ////Display the host information specified by the client;
    //Console.WriteLine("Host name: {0}", request.UserHostName);
    //Console.WriteLine("Host address: {0}", request.UserHostAddress);
    //Console.WriteLine("User agent: {0}", request.UserAgent);
}
