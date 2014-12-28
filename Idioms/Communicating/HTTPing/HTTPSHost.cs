using CuttingEdge.Conditions;
using Decoratid.Idioms.Serviceable;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using uhttpsharp.RequestProviders;
using uhttpsharp;
using uhttpsharp.Handlers;
using uhttpsharp.Listeners;
using System.Net.Sockets;
using System.Net;

namespace Decoratid.Idioms.Communicating.HTTPing
{
    public class HTTPSHost : ServiceBase
    {
        #region Declarations
        private readonly uhttpsharp.HttpServer _server = null;
        private X509Certificate _sslCert = null;
        //private readonly HttpListener _listener = null;
        private readonly Func<HttpListenerRequest, string> _responderMethod;
        private string _cerPath;
        private string _pvkPath;
        #endregion

        #region Ctor
        public HTTPSHost(EndPoint ep, Func<HttpListenerRequest, string> method, string path)
        {
            Condition.Requires(ep).IsNotNull();

            //validate the ep is within the current ip list
            var ips = NetUtil.GetLocalIPAddresses();
            Condition.Requires(ips).Contains(ep.IPAddress);

            this.EndPoint = ep;

            ////validate  the platform
            //if (!HttpListener.IsSupported)
            //    throw new NotSupportedException(
            //        "Needs Windows XP SP2, Server 2003 or later.");

            //// A responder method is required
            //if (method == null)
            //    throw new ArgumentException("method");
            //_responderMethod = method;

            ////generate a cert
            //AsymmetricCipherKeyPair kp;
            //var x509 = CertificateGenerator.GenerateCertificate("Subject", out kp);
          
            ////setup the directory to save the certs 
            //string dirname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //if (!Directory.Exists(dirname))
            //    Directory.CreateDirectory(dirname);
            
            //string certpath = Path.Combine(dirname, ".mono");
            //if (!Directory.Exists(certpath))
            //    Directory.CreateDirectory(certpath);
            
            //certpath = Path.Combine(certpath, "httplistener");
            //if (!Directory.Exists(certpath))
            //    Directory.CreateDirectory(certpath);

            //this._cerPath = Path.Combine(certpath, String.Format("{0}.cer", ep.Port));
            //this._pvkPath = Path.Combine(certpath, String.Format("{0}.pvk", ep.Port));
            
            //// save it
            //string Alias = "foo";
            //string Pwd = "bar";
            //CertificateGenerator.SaveToFile(x509, kp, _pvkPath, Alias, Pwd);

            //X509Certificate2 x509_2 = new X509Certificate2(_pvkPath, Pwd);
            //CertUtil.SaveAsCER(x509_2, this._cerPath);

            //CertUtil.WireSSLCert(x509_2, ep);

            //this._listener = new HttpListener();

            //string prefix = "http://" + this.EndPoint.IPAddress.ToString() + ":" + this.EndPoint.Port + "/" + path;
            //if (!prefix.EndsWith("/"))
            //    prefix = prefix + "/";
            //_listener.Prefixes.Add(prefix);


            using (var httpServer = new uhttpsharp.HttpServer(new HttpRequestProvider()))
            {
                // Normal port 80 :
                httpServer.Use(new TcpListenerAdapter(new TcpListener(IPAddress.Loopback, 80)));

                // Ssl Support :
                var serverCertificate = X509Certificate.CreateFromCertFile(@"TempCert.cer");
                httpServer.Use(new ListenerSslDecorator(new TcpListenerAdapter(new TcpListener(IPAddress.Loopback, 443)), serverCertificate));

                // Request handling : 
                httpServer.Use((context, next) =>
                {
                    Console.WriteLine("Got Request!");
                    return next();
                });

                // Handler classes : 
                httpServer.Use(new TimingHandler());
                httpServer.Use(new HttpRouter().With(string.Empty, new IndexHandler())
                                                .With("about", new AboutHandler()));
                httpServer.Use(new FileHandler());
                httpServer.Use(new ErrorHandler());

                httpServer.Start();

                Console.ReadLine();
            }

            using (var httpServer = new HttpServer(new HttpRequestProvider()))
            {
                httpServer.Use(new TcpListenerAdapter(new TcpListener(IPAddress.Any, 82)));
                //httpServer.Use(new ListenerSslDecorator(new TcpListenerAdapter(new TcpListener(IPAddress.Loopback, 443)), serverCertificate));

                //httpServer.Use(new SessionHandler<DateTime>(() => DateTime.Now));
                httpServer.Use(new ExceptionHandler());
                httpServer.Use(new CompressionHandler(DeflateCompressor.Default, GZipCompressor.Default));
                httpServer.Use(new ControllerHandler(new BaseController(), new JsonModelBinder(), new JsonView()));
                httpServer.Use(new HttpRouter().With(string.Empty, new IndexHandler())
                    .With("about", new AboutHandler())
                    .With("Assets", new AboutHandler())
                    .With("strings", new RestHandler<string>(new StringsRestController(), JsonResponseProvider.Default)));

                httpServer.Use(new ClassRouter(new MySuperHandler()));
                httpServer.Use(new TimingHandler());

                httpServer.Use(new MyHandler());
                httpServer.Use(new FileHandler());
                httpServer.Use(new ErrorHandler());
                httpServer.Use((context, next) =>
                {
                    Console.WriteLine("Got Request!");
                    return next();
                });

                httpServer.Start();
                Console.ReadLine();
            }
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
            //delete the cert files
            if (File.Exists(this._pvkPath))
                File.Delete(this._pvkPath);
            if (File.Exists(this._cerPath))
                File.Delete(this._cerPath);

            base.DisposeManaged();
        }
        #endregion

        #region Static Methods
        public static HTTPSHost New(EndPoint ep, Func<HttpListenerRequest, string> method, string path)
        {
            HTTPSHost host = new HTTPSHost(ep, method, path);
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
            if (post != null)
                rv.Add(post);
            return rv;
        }
        #endregion
    }

}

