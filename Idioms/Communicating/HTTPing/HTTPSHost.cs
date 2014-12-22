using CuttingEdge.Conditions;
using Decoratid.Idioms.Serviceable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HttpServer;
using HttpListener = HttpServer.HttpListener;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Decoratid.Utils;
using Decoratid.Extensions;

namespace Decoratid.Idioms.Communicating.HTTPing
{
    public class HTTPSHost : ServiceBase
    {
        #region Declarations
        private readonly HttpListener _listener;
        private readonly Func<IHttpClientContext, IHttpRequest, string> _responderMethod;
        private X509Certificate2 _cert;
        #endregion

        #region Ctor
        public HTTPSHost(EndPoint ep, Func<IHttpClientContext, IHttpRequest, string> method)
            : base()
        {
            Condition.Requires(ep).IsNotNull();

            //validate the ep is within the current ip list
            var ips = NetUtil.GetLocalIPAddresses();
            Condition.Requires(ips).Contains(ep.IPAddress);
            this.EndPoint = ep;
           
            _responderMethod = method;

            _cert = SelfSignedCertUtil.GenerateAndRegisterSelfSignedCertificate("CN=branch", 60, "CN=tree");
            //_cert = new X509Certificate2("../../certInProjectFolder.p12", "yourCertPassword");
            _listener = HttpListener.Create(ep.IPAddress, ep.Port);
            _listener.RequestReceived += OnRequest;
        }
        #endregion

        #region Properties
        public EndPoint EndPoint { get; set; }
        #endregion

        #region Overrides
        protected override void initialize()
        {
            //create the root cert



            _listener.Start(5);
        }
        protected override void start()
        {
       
        }
        protected override void stop()
        {
            if (this._listener != null)
            {
                _listener.Stop();
            }
        }
        protected override void DisposeManaged()
        {
            base.DisposeManaged();
        }

        private void OnRequest(object source, RequestEventArgs args)
        {

            IHttpClientContext context = (IHttpClientContext)source;
            IHttpRequest request = args.Request;

            var result = this._responderMethod(context, request);

            context.Respond(result);


        }
        #endregion

        #region Static Methods
        public static HTTPSHost New(EndPoint ep, Func<IHttpClientContext, IHttpRequest, string> method)
        {
            HTTPSHost host = new HTTPSHost(ep, method);
            return host;
        }
        public static List<string> DumpHTTPRequest(IHttpRequest req)
        {
            List<string> rv = new List<string>();

            rv.Add(req.HttpVersion);
            rv.Add(req.Method);
            rv.Add(req.Uri.ToString());
            rv.Add("Headers");
            foreach (var each in req.Headers)
                rv.Add(each.ToString());
            rv.Add("Cookies");
            foreach (var cookie in req.Cookies)
            {
                rv.Add(cookie.ToString());
            }
            rv.Add("Body");

            var bodyArr = req.Body.ReadStreamFully(1024);
            var body = Encoding.Default.GetString(bodyArr);
            rv.Add(body);

            return rv;
        }
        #endregion

    }
}
