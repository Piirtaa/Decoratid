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
        public HTTPSHost(EndPoint ep, Func<IHttpClientContext,IHttpRequest, string> method)
        {
            Condition.Requires(ep).IsNotNull();

            //validate the ep is within the current ip list
            var ips = NetUtil.GetLocalIPAddresses();
            Condition.Requires(ips).Contains(ep.IPAddress);

            this.EndPoint = ep;


            
            _responderMethod = method;

            _cert = SelfSignedCertUtil.GenerateAndRegisterSelfSignedCertificate("yo", 60, "bossCheese");

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

            //IHttpClientContext context = (IHttpClientContext)source;
            //IHttpRequest request = args.Request;

            //// Here we create a response object, instead of using the client directly.
            //// we can use methods like Redirect etc with it,
            //// and we dont need to keep track of any headers etc.
            //IHttpResponse response = request.CreateResponse(context);

            //byte[] body = Encoding.UTF8.GetBytes("Hello secure you!");
            //response.Body.Write(body, 0, body.Length);
            //response.Send();


            //// Respond is a small convenience function that let's you send one string to the browser.
            //// you can also use the Send, SendHeader and SendBody methods to have total control.
            //if (request.Uri.AbsolutePath == "/hello")
            //    context.Respond("Hello to you too!");

            //else if (request.UriParts.Length == 1 && request.UriParts[0] == "goodbye")
            //{
            //    IHttpResponse response = request.CreateResponse(context);
            //    StreamWriter writer = new StreamWriter(response.Body);
            //    writer.WriteLine("Goodbye to you too!");
            //    writer.Flush();
            //    response.Send();
            //}
        }
        #endregion

        #region Static Methods
        public static HTTPSHost New(EndPoint ep, Func<IHttpClientContext, IHttpRequest, string> method)
        {
            HTTPSHost host = new HTTPSHost(ep, method);
            return host;
        }
        #endregion

    }
}
