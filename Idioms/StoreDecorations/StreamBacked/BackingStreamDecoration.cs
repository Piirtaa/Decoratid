using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Extensions;
using Decoratid.Thingness;
using ServiceStack.Text;
using Decoratid.Core.Logical;
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;
using Decoratid.Idioms.ObjectGraph;

namespace Decoratid.Core.Storing.Decorations.StreamBacked
{
    /// <summary>
    /// when the store is written to the backing stream
    /// </summary>
    public enum BackingPoint
    {
        None,
        /// <summary>
        /// On every commit
        /// </summary>
        OnCommit,
        /// <summary>
        /// When the supplied condition is true.  Is tested on each poll.
        /// </summary>
        OnPollingCondition
    }

    /// <summary>
    /// a store that backs up to a stream (either on each commit, or on a condition)
    /// </summary>
    public interface IStreamBackedStore : IDecoratedStore, IDisposable
    {
        Stream Stream { get; }
        /// <summary>
        /// if we want to encode the data we are writing, we provide a strategy here.  
        /// </summary>
        /// <remarks>
        /// It's an injectable behaviour, not overrideable - why? because it's easier to locate
        /// and adjust this behaviour within a decoration tree, than to deal with the inheritance hassles. 
        /// </remarks>
        Func<string, string> EncodeWriteDataStrategy { get; set; }
        /// <summary>
        /// if we want to decode an encoded stream we are reading, we provide a strategy here
        /// </summary>
        /// <remarks>
        /// It's an injectable behaviour, not overrideable - why? because it's easier to locate
        /// and adjust this behaviour within a decoration tree, than to deal with the inheritance hassles. 
        /// </remarks>
        Func<string, string> DecodeReadDataStrategy { get; set; }

        /// <summary>
        /// When we flush to the stream
        /// </summary>
        BackingPoint BackingPoint { get; }
        /// <summary>
        /// If we have a conditional backing criteria, we set it here.  
        /// </summary>
        ICondition BackingCondition { get; }
    }


    /// <summary>
    /// dumps all commits to a backing store.
    /// Store must implement IGetAllableStore as this is called when we backup.
    /// Uses Serialization.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class BackingStreamDecoration : DecoratedStoreBase, IStreamBackedStore, IHasHydrationMap
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        protected BackingStreamDecoration() : base(){}
        /// <summary>
        /// ctor for any classes who want to layer functionality on an existing backing stream decoration
        /// </summary>
        /// <param name="decorated"></param>
        protected BackingStreamDecoration(BackingStreamDecoration decorated)
            : base(decorated)
        {
            this.BackingPoint = StreamBacked.BackingPoint.None;
        }
        /// <summary>
        /// Loads from the backing file.  Writes on every commit
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="filePath"></param>
        public BackingStreamDecoration(IStore decorated, Stream stream, bool loadStream = true)
            : base(decorated)
        {
            Condition.Requires(stream).IsNotNull();

            this.BackingPoint = StreamBacked.BackingPoint.OnCommit;

            this.Stream = stream;

            if(loadStream)
                this.LoadFromStream();
        }
        /// <summary>
        /// Loads from the backing file.  Writes on a custom polling condition
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="stream"></param>
        /// <param name="backingCondition"></param>
        public BackingStreamDecoration(IStore decorated, Stream stream, ICondition backingCondition, bool loadStream = true)
            : base(decorated)
        {
            Condition.Requires(stream).IsNotNull();
            Condition.Requires(backingCondition).IsNotNull();

            this.Stream = stream;
            if (loadStream)
                this.LoadFromStream();

            this.InitBackingCondition(backingCondition);
        }
        #endregion

        #region IHasHydrationMap
        public virtual IHydrationMap GetHydrationMap()
        {
            var hydrationMap = new HydrationMapValueManager<BackingStreamDecoration>();
            hydrationMap.RegisterDefault("BackgroundJob", x => x.BackgroundJob, (x, y) => { x.BackgroundJob = y as BackgroundHost; });
            hydrationMap.RegisterDefault("BackingCondition", x => x.BackingCondition, (x, y) => { x.BackingCondition = y as ICondition; });
            hydrationMap.RegisterDefault("BackingPoint", x => x.BackingPoint, (x, y) => { x.BackingPoint = (BackingPoint)y; });
            hydrationMap.RegisterDefault("EncodeWriteDataStrategy", x => x.EncodeWriteDataStrategy, (x, y) => { x.EncodeWriteDataStrategy = y as Func<string, string>; });
            hydrationMap.RegisterDefault("DecodeReadDataStrategy", x => x.DecodeReadDataStrategy, (x, y) => { x.DecodeReadDataStrategy = y as Func<string, string>; });
            return hydrationMap;
        }
        #endregion

        #region IDecorationHydrateable
        public override string DehydrateDecoration(IGraph uow = null)
        {
            return this.GetHydrationMap().DehydrateValue(this, uow);
        }
        public override void HydrateDecoration(string text, IGraph uow = null)
        {
            this.GetHydrationMap().HydrateValue(this, text, uow);
        }
        #endregion

        #region Properties
        public BackgroundHost BackgroundJob { get; protected set; }
        public Stream Stream { get; protected set; }
        public BackingPoint BackingPoint { get; protected set; }
        public ICondition BackingCondition { get; protected set; }
        /// <summary>
        /// if we want to encode the data we are writing, we provide a strategy here
        /// </summary>
        public Func<string, string> EncodeWriteDataStrategy { get; set; }
        /// <summary>
        /// if we want to decode an encoded stream we are reading, we provide a strategy here
        /// </summary>
        public Func<string, string> DecodeReadDataStrategy { get; set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            BackingStreamDecoration returnValue = null;
            switch (this.BackingPoint)
            {
                case StreamBacked.BackingPoint.None:
                    break;
                case StreamBacked.BackingPoint.OnCommit:
                    returnValue = new BackingStreamDecoration(store, this.Stream);
                    returnValue.DecodeReadDataStrategy = this.DecodeReadDataStrategy;
                    returnValue.EncodeWriteDataStrategy = this.EncodeWriteDataStrategy;
                    break;
                case StreamBacked.BackingPoint.OnPollingCondition:
                    returnValue = new BackingStreamDecoration(store, this.Stream, this.BackingCondition);
                    returnValue.DecodeReadDataStrategy = this.DecodeReadDataStrategy;
                    returnValue.EncodeWriteDataStrategy = this.EncodeWriteDataStrategy;
                    break;
            }
            return returnValue;
        }
        #endregion

        #region Methods
        public virtual void SaveToStream()
        {
            string data = StoreSerializer.SerializeStore(this,null, this.EncodeWriteDataStrategy);
            this.WriteStream(data);
        }
        public virtual void LoadFromStream()
        {
            //read it
            var text = this.ReadStream();

            //if it's empty, exit
            if (string.IsNullOrEmpty(text))
                return;

            var store = StoreSerializer.DeserializeStore(text, null, this.DecodeReadDataStrategy);

            var list = store.GetAll();

            //commit this list back using the wrapped store (and bypassing this layer's interception)
            CommitBag bag = new CommitBag();
            list.WithEach(x =>
            {
                bag.MarkItemSaved(x);
            });
            this.Decorated.Commit(bag);
        }
        #endregion

        #region Overrides
        public override void Commit(ICommitBag bag)
        {
            this.Decorated.Commit(bag);

            if (this.BackingPoint == StreamBacked.BackingPoint.OnCommit)
            {
                this.SaveToStream();
            }
            //else if (this.BackingPoint == StreamBacked.BackingPoint.OnPollingCondition)
            //{
            //    this.CheckCondition();
            //}
        }
        protected override void DisposeManaged()
        {
            if (this.BackgroundJob != null)
                this.BackgroundJob.Dispose();

            this.SaveToStream();
            base.DisposeManaged();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// this method is called by the background process
        /// </summary>
        protected void CheckCondition()
        {
            if (this.BackingPoint != StreamBacked.BackingPoint.OnPollingCondition)
                return;

            var status = this.BackingCondition.Evaluate();

            if (status.GetValueOrDefault())
                this.SaveToStream();
        }
        protected void InitBackingCondition(ICondition backingCondition)
        {
            this.BackingPoint = StreamBacked.BackingPoint.OnPollingCondition;
            this.BackingCondition = backingCondition;
            this.BackgroundJob = new BackgroundHost();
            //this.BackgroundJob.BackgroundIntervalMSecs = 5000;
            this.BackgroundJob.BackgroundAction = Logic.New(() => { this.CheckCondition(); });
            this.BackgroundJob.IsEnabled = true;
        }

        /// <summary>
        /// reads the stream.  attempts to move cursor to position 0 first (if seekable stream).  uses ASCII encoding - as does WriteStream
        /// </summary>
        /// <returns></returns>
        protected string ReadStream()
        {
            //move to start of stream
            if (this.Stream.CanSeek)
            {
                this.Stream.Position = 0;
            }

            var readBytes = this.Stream.ReadFully();

            //if there are bytes to read we have a json object to parse
            if (readBytes != null && readBytes.Length > 0)
            {
                var text = Encoding.ASCII.GetString(readBytes);
                return text;
            }
            return null;
        }
        /// <summary>
        /// writes to the stream.  attempts to move cursor to position 0 first (if seekable stream).  uses ASCII encoding - as does ReadStream
        /// </summary>
        /// <param name="text"></param>
        protected void WriteStream(string text)
        {
            byte[] messageByte = Encoding.ASCII.GetBytes(text);

            try
            {
                //move to start of stream
                if (this.Stream.CanSeek)
                {
                    this.Stream.Position = 0;
                }

                //using (StreamWriter writer = new StreamWriter(this.Stream, Encoding.UTF8, 512, true))
                //{
                //    writer.Write(text);
                //}

                this.Stream.Write(messageByte, 0, messageByte.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion


    }
}
