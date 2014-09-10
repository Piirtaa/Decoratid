using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness.Idioms.Conditions;
using Decoratid.Extensions;
using Decoratid.Thingness;
using Decoratid.Utils;
using ServiceStack.Text;
using Decoratid.Thingness.Decorations;
using Decoratid.Thingness.Idioms.ObjectGraph.Values;

namespace Decoratid.Thingness.Idioms.Store.Decorations.StreamBacked
{
    /// <summary>
    /// the same thing as backingfiledecoration - this backs to a file.  it differs in that it keeps the file handle to itself
    /// for its lifetime, preventing all other threads from accessing the file.
    /// </summary>
    public class BackingLockedFileDecoration : BackingStreamDecoration, IFileBackedStore
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        protected BackingLockedFileDecoration() : base() { }
        /// <summary>
        /// Loads from the backing file
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="filePath"></param>
        public BackingLockedFileDecoration(IStore decorated, string filePath)
            : base(decorated, FileUtil.GetLockedStream(filePath), false)
        {
            Condition.Requires(filePath).IsNotNullOrEmpty();
            this.FilePath = filePath;
            this.LoadFromStream();
        }
        public BackingLockedFileDecoration(IStore decorated, string filePath, ICondition flushCondition)
            : base(decorated, FileUtil.GetLockedStream(filePath), flushCondition, false)
        {
            Condition.Requires(filePath).IsNotNullOrEmpty();
            this.FilePath = filePath;
            this.LoadFromStream();
        }
        #endregion

        #region Properties
        public string FilePath { get; private set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IStore> ApplyThisDecorationTo(IStore store)
        {
            BackingLockedFileDecoration returnValue = null;
            switch (this.BackingPoint)
            {
                case StreamBacked.BackingPoint.None:
                    break;
                case StreamBacked.BackingPoint.OnCommit:
                    returnValue = new BackingLockedFileDecoration(store, this.FilePath);
                    returnValue.DecodeReadDataStrategy = this.DecodeReadDataStrategy;
                    returnValue.EncodeWriteDataStrategy = this.EncodeWriteDataStrategy;
                    break;
                case StreamBacked.BackingPoint.OnPollingCondition:
                    returnValue = new BackingLockedFileDecoration(store, this.FilePath, this.BackingCondition);
                    returnValue.DecodeReadDataStrategy = this.DecodeReadDataStrategy;
                    returnValue.EncodeWriteDataStrategy = this.EncodeWriteDataStrategy;
                    break;
            }
            return returnValue;
        }
        #endregion

        #region IHasHydrationMap
        public override IHydrationMap GetHydrationMap()
        {
            //get the inherited map
            var baseMap = base.GetHydrationMap();

            var map = new HydrationMapValueManager<BackingLockedFileDecoration>();
            map.RegisterDefault("FilePath", x => x.FilePath, (x, y) => { x.FilePath = y as string; });
            map.Maps.AddRange(baseMap.Maps);
            return map;
        }
        #endregion

        #region Stream Overrides
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

            this.Stream = FileUtil.GetLockedStream(this.FilePath);
        }
        #endregion

        #region Overrides
        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            if (this.Stream != null)
            {
                this.Stream.Close();
                this.Stream.Dispose();
            }
        }
        #endregion
    }
}
