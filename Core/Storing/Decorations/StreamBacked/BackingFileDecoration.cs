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
using Decoratid.Idioms.Decorating;
using Decoratid.Idioms.ObjectGraph.Values;

namespace Decoratid.Idioms.Storing.Decorations.StreamBacked
{
    public interface IFileBackedStore : IStreamBackedStore, IDisposable
    {
        string FilePath { get; }
    }

    /// <summary>
    /// dumps all commits to a backing store.
    /// Store must implement IGetAllableStore as this is called when per commit (we're saving everything on every commit).  
    /// Uses JSON Serialization.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class BackingFileDecoration : BackingStreamDecoration, IFileBackedStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        protected BackingFileDecoration() : base() { }
        /// <summary>
        /// Loads from the backing file
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="filePath"></param>
        public BackingFileDecoration(IStore decorated, string filePath)
            : base(decorated,  System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), false)
        {
            Condition.Requires(filePath).IsNotNullOrEmpty();
            this.FilePath = filePath;
            this.LoadFromStream();
        }
        public BackingFileDecoration(IStore decorated, string filePath, ICondition flushCondition)
            : base(decorated, System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), flushCondition, false)
        {
            Condition.Requires(filePath).IsNotNullOrEmpty();
            this.FilePath = filePath;
            this.LoadFromStream();
        }
        #endregion

        #region Properties
        public string FilePath { get; private set; }
        #endregion

        #region IHasHydrationMap
        public override IHydrationMap GetHydrationMap()
        {
            //get the inherited map
            var baseMap = base.GetHydrationMap();

            var map = new HydrationMapValueManager<BackingFileDecoration>();
            map.RegisterDefault("FilePath", x => x.FilePath, (x, y) => { x.FilePath = y as string; });
            map.Maps.AddRange(baseMap.Maps);
            return map;
        }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            BackingFileDecoration returnValue = null;
            switch (this.BackingPoint)
            {
                case StreamBacked.BackingPoint.None:
                    break;
                case StreamBacked.BackingPoint.OnCommit:
                    returnValue = new BackingFileDecoration(store, this.FilePath);
                    returnValue.DecodeReadDataStrategy = this.DecodeReadDataStrategy;
                    returnValue.EncodeWriteDataStrategy = this.EncodeWriteDataStrategy;
                    break;
                case StreamBacked.BackingPoint.OnPollingCondition:
                    returnValue = new BackingFileDecoration(store, this.FilePath, this.BackingCondition);
                    returnValue.DecodeReadDataStrategy = this.DecodeReadDataStrategy;
                    returnValue.EncodeWriteDataStrategy = this.EncodeWriteDataStrategy;
                    break;
            }
            return returnValue;
        }
        #endregion


        #region Overrides
        public override void LoadFromStream()
        {
            //init stream if it's closed
            this.InitStream();

            base.LoadFromStream();

            this.Stream.Close();
        }
        public override void SaveToStream()
        {
            this.InitStream();

            base.SaveToStream();

            this.Stream.Close();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// initializes the stream, closing/disposing first
        /// </summary>
        protected void InitStream()
        {
            if (this.Stream != null)
            {
                this.Stream.Close();
                this.Stream.Dispose();
            }

            this.Stream = System.IO.File.Open(this.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        #endregion
    }
}
