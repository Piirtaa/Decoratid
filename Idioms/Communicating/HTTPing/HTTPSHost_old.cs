//using CuttingEdge.Conditions;
//using Decoratid.Idioms.Serviceable;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using HttpServer;
//using HttpListener = HttpServer.HttpListener;
//using System.IO;
//using System.Security.Cryptography.X509Certificates;
//using Decoratid.Utils;
//using Decoratid.Extensions;
//using Org.BouncyCastle.Crypto;

//namespace Decoratid.Idioms.Communicating.HTTPing
//{
//    public class HTTPSHost_old : ServiceBase
//    {
//        #region Declarations
//        private readonly HttpListener _listener;
//        private readonly Func<IHttpClientContext, IHttpRequest, string> _responderMethod;
//        private X509Certificate2 _cert;
//        private X509Certificate2 _caCert;
//        #endregion

//        #region Ctor
//        public HTTPSHost_old(EndPoint ep, Func<IHttpClientContext, IHttpRequest, string> method)
//            : base()
//        {
//            Condition.Requires(ep).IsNotNull();

//            //validate the ep is within the current ip list
//            var ips = NetUtil.GetLocalIPAddresses();
//            Condition.Requires(ips).Contains(ep.IPAddress);
//            this.EndPoint = ep;
           
//            _responderMethod = method;

//            var dat = Environment.SpecialFolder.ApplicationData;

//            //build  CA cert 
//            string issuerName = "CN=tree";
//            string certName = "CN=branch";
//            int certDurationInMins = 1051200;
            
//            //setup the directory to save the certs 
//            string dirname = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
//            string path = Path.Combine(dirname, ".mono");
//            path = Path.Combine(path, "httplistener");

//            AsymmetricKeyParameter myCAprivateKey = null;
//            //generate a root CA cert and obtain the privateKey
//            _caCert = SelfSignedCertUtil.GenerateCACertificate(issuerName, certDurationInMins, ref myCAprivateKey);

//            string cert_file = Path.Combine(path, String.Format("{0}.cer", ep.Port));
//            if (!File.Exists(cert_file))
//                return;
//            string pvk_file = Path.Combine(path, String.Format("{0}.pvk", ep.Port));
//            SelfSignedCertUtil.CreateCertPFX(_caCert, 
//            AsymmetricKeyParameter myCAprivateKey = null;
//            //generate a root CA cert and obtain the privateKaey
//            _caCert = SelfSignedCertUtil.GenerateCACertificate(issuerName, certDurationInMins, ref myCAprivateKey);
//            //add CA cert to store
//            SelfSignedCertUtil.AddCertToStore(_caCert, StoreName.My, StoreLocation.LocalMachine);
//            //generate cert based on the CA cert privateKey
//            _cert = SelfSignedCertUtil.GenerateSelfSignedCertificate(certName, certDurationInMins, issuerName, myCAprivateKey);
//            //add cert to store
//            SelfSignedCertUtil.AddCertToStore(_cert, StoreName.My, StoreLocation.LocalMachine);

//            //validate we can load it
//            var cert2 = SelfSignedCertUtil.GetCert(certName, StoreName.My, StoreLocation.LocalMachine);
           
//            _listener = HttpListener.Create(ep.IPAddress, ep.Port);
//            _listener.RequestReceived += OnRequest;
//        }
//        #endregion

//        #region Properties
//        public EndPoint EndPoint { get; set; }
//        #endregion

//        #region Overrides
//        protected override void initialize()
//        {
//            _listener.Start(5);
//        }
//        protected override void start()
//        {
       
//        }
//        protected override void stop()
//        {
//            if (this._listener != null)
//            {
//                _listener.Stop();
//            }
//        }
//        protected override void DisposeManaged()
//        {
//            //remove the certs
//            SelfSignedCertUtil.RemoveCertFromStore(_cert, StoreName.My, StoreLocation.CurrentUser);
//            SelfSignedCertUtil.RemoveCertFromStore(_caCert, StoreName.My, StoreLocation.CurrentUser);

//            base.DisposeManaged();
//        }

//        private void OnRequest(object source, RequestEventArgs args)
//        {

//            IHttpClientContext context = (IHttpClientContext)source;
//            IHttpRequest request = args.Request;

//            var result = this._responderMethod(context, request);

//            context.Respond(result);


//        }
//        #endregion

//        #region Static Methods
//        public static HTTPSHost_old New(EndPoint ep, Func<IHttpClientContext, IHttpRequest, string> method)
//        {
//            HTTPSHost_old host = new HTTPSHost_old(ep, method);
//            return host;
//        }
//        public static List<string> DumpHTTPRequest(IHttpRequest req)
//        {
//            List<string> rv = new List<string>();

//            rv.Add(req.HttpVersion);
//            rv.Add(req.Method);
//            rv.Add(req.Uri.ToString());
//            rv.Add("Headers");
//            foreach (var each in req.Headers)
//                rv.Add(each.ToString());
//            rv.Add("Cookies");
//            foreach (var cookie in req.Cookies)
//            {
//                rv.Add(cookie.ToString());
//            }
//            rv.Add("Body");

//            var bodyArr = req.Body.ReadStreamFully(1024);
//            var body = Encoding.Default.GetString(bodyArr);
//            rv.Add(body);

//            return rv;
//        }
//        #endregion

//    }
//}
