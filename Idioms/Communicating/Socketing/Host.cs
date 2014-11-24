using Decoratid.Core.Logical;
using Decoratid.Core.ValueOfing;
using Decoratid.Extensions;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Decoratid.Idioms.Communicating.Socketing
{


    public class Host : EndPointHostBase, IIPFilteringEndPointHost
    {
        #region Declarations
        private readonly object _stateLock = new object();
        TcpListener _listener = null;
        #endregion

        #region Ctor
        public Host(EndPoint ep, LogicOfTo<string, string> logic= null, Func<System.Net.IPEndPoint, bool> validateClientEndPointStrategy = null)
            : base(ep, logic)
        {
            this.ValidateClientEndPointStrategy = validateClientEndPointStrategy;

            this.Initialize();
            this.Start();
        }
        #endregion

        #region Properties
        public bool IsListening { get; private set; }
        public Func<System.Net.IPEndPoint, bool> ValidateClientEndPointStrategy { get; private set; }
        #endregion

        #region Overrides
        protected override void initialize()
        {

        }
        protected override void start()
        {
            Action action = () => this.StartListening();
            action.EnqueueToThreadPool();

            //sleep a bit to make sure the thread has started
            Thread.Sleep(1000);
        }
        protected override void stop()
        {
            this.StopListening();
        }
        #endregion

        #region Helpers
        private void StartListening()
        {
            try
            {
                lock (this._stateLock)
                {
                    _listener = new TcpListener(this.EndPoint.IPAddress, this.EndPoint.Port);
                    _listener.Start();
                    this.IsListening = true;
                }

                while (this.IsListening)
                {
                    TcpClient client = _listener.AcceptTcpClient();

                    // queue a request to take care of the client
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessClient), client);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void StopListening()
        {
            lock (this._stateLock)
            {
                this.IsListening = false;

                if (this._listener != null)
                {
                    try
                    {
                        this._listener.Stop();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private bool ValidateClientEndPoint(System.Net.IPEndPoint ep)
        {
            if (this.ValidateClientEndPointStrategy == null)
                return true;

            return this.ValidateClientEndPointStrategy(ep);
        }
        private void ProcessClient(object state)
        {
            System.Net.Sockets.TcpClient client = state as System.Net.Sockets.TcpClient;

            try
            {
                if (this.ValidateClientEndPoint((System.Net.IPEndPoint)client.Client.RemoteEndPoint))
                {
                    //get the stream 
                    using (NetworkStream ns = client.GetStream())
                    {
                        var input = ProtocolUtil.Read(ns);

                        var logic = this.Logic.Perform(input) as LogicOfTo<string, string>; //not biasing logic

                        // Write a message to the client.
                        ProtocolUtil.Write(ns, logic.Result);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (client != null)
                    {
                        client.Close();
                    }
                }
                catch { }
            }

        }
        #endregion

        //#region Validation
        //public override bool Validate()
        //{
        //    bool returnValue = false;
        //    Func<string, object, string> oldStrategy = this.ServerStrategy;
        //    Func<string, object, string> echoStrategy = (req, state) =>
        //    {
        //        return req;
        //    };
        //    SimpleTCPClient client = null;
        //    //AutoResetEvent mre = new AutoResetEvent(false);
        //    //Exception trappedEx = null;

        //    try
        //    {
        //        lock (this._stateLock)
        //        {
        //            //swap out the old strategy with the echo strategy
        //            this.ServerStrategy = echoStrategy;

        //            //recycle
        //            this.Recycle();

        //            //grab a client and attempt to send a message 
        //            string data = string.Format("Validate for {0}", this.GetType().Name);

        //            client = new SimpleTCPClient(this.Address, this.Port);

        //            //sync first
        //            string resp = client.Send(data);

        //            if (resp != data) { throw new Exception("Sync echo failed"); }

        //            data = data + "1";
        //            resp = client.Send(data);

        //            if (resp != data) { throw new Exception("Sync echo failed"); }

        //            ////async
        //            //client.SendAsync(data, (socketState) =>
        //            //{
        //            //    if (socketState.ReceiveData.ToString() != data)
        //            //    {
        //            //        returnValue = false;
        //            //        trappedEx = new Exception("Async echo failed");
        //            //    }
        //            //    mre.Set();
        //            //});

        //            //mre.WaitOne();

        //            //if (trappedEx != null)
        //            //{
        //            //    throw trappedEx;
        //            //}

        //            //test the throttle
        //        }
        //        returnValue = true;
        //    }
        //    catch
        //    {
        //        returnValue = false;
        //    }
        //    finally
        //    {
        //        //swap back the old strategy
        //        this.ServerStrategy = oldStrategy;

        //        if (client != null)
        //        {
        //            client.Dispose();
        //        }

        //        //if (mre != null)
        //        //{
        //        //    mre.Dispose();
        //        //}
        //    }
        //    return returnValue;
        //}
        //#endregion

        #region Static Methods
        public static Host New(EndPoint ep, LogicOfTo<string, string> logic = null, Func<System.Net.IPEndPoint, bool> validateClientEndPointStrategy = null)
        {
            Host host = new Host(ep, logic, validateClientEndPointStrategy);
            return host;
        }
        public static Host NewEchoing(EndPoint ep, Func<System.Net.IPEndPoint, bool> validateClientEndPointStrategy = null)
        {
            Host host = new Host(ep, LogicOfTo<string, string>.New((x) => { return x; }), validateClientEndPointStrategy);
            return host;
        }
        #endregion

    }
}
