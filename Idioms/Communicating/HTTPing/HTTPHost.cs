using CuttingEdge.Conditions;
using Decoratid.Idioms.Serviceable;
using Mono.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Decoratid.Idioms.Communicating.HTTPing
{
    public class HTTPHost : ServiceBase
    {
        #region Declarations
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;
        #endregion

        #region Ctor
        public HTTPHost(EndPoint ep, Func<HttpListenerRequest, string> method, string path)
        {
            Condition.Requires(ep).IsNotNull();

            //validate the ep is within the current ip list
            var ips = NetUtil.GetLocalIPAddresses();
            Condition.Requires(ips).Contains(ep.IPAddress);

            this.EndPoint = ep;

            //validate  the platform
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            string prefix = "http://" + this.EndPoint.IPAddress.ToString() + ":" + this.EndPoint.Port + "/" + path;
            if (!prefix.EndsWith("/"))
                prefix = prefix + "/";
            _listener.Prefixes.Add(prefix);

            _responderMethod = method;

        }
        #endregion

        #region Properties
        public EndPoint EndPoint { get; set; }
        #endregion

        #region Overrides
        protected override void initialize()
        {
            _listener.Start();
        }
        protected override void start()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }
        protected override void stop()
        {
            if (this._listener != null)
            {
                _listener.Stop();
                _listener.Close();
            }
        }
        protected override void DisposeManaged()
        {
            base.DisposeManaged();
        }
        #endregion

        #region Static Methods
        public static HTTPHost New(EndPoint ep, Func<HttpListenerRequest, string> method, string path)
        {
            HTTPHost host = new HTTPHost(ep, method, path);
            return host;
        }
        public static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            using (System.IO.Stream body = request.InputStream) // here we have data
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        public static List<string> DumpHTTPRequest(HttpListenerRequest req)
        {
            List<string> rv = new List<string>();

            rv.Add(req.ProtocolVersion.ToString());
            rv.Add(req.HttpMethod);
            rv.Add(req.Url.ToString());
            rv.Add(req.RemoteEndPoint.ToString());
            rv.Add("Headers");
            foreach (var key in req.Headers.AllKeys)
                rv.Add(req.Headers[key]);
            rv.Add("Cookies");
            foreach (var cookie in req.Cookies)
            {
                rv.Add(cookie.ToString());
            }
            rv.Add("Body");
            var post = GetRequestPostData(req);
            if(post != null)
                rv.Add(post);
            return rv;
        }
        #endregion
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        WebServer ws = new WebServer(SendResponse, "http://localhost:8080/test/");
    //        ws.Run();
    //        Console.WriteLine("A simple webserver. Press a key to quit.");
    //        Console.ReadKey();
    //        ws.Stop();
    //    }

    //    public static string SendResponse(HttpListenerRequest request)
    //    {
    //        return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
    //    }
    //}
}

