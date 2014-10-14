using CuttingEdge.Conditions;
using Decoratid.Core.Conditional;
using Decoratid.Core.Decorating;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Decoratid.Idioms.Filing
{
    /// <summary>
    /// implements fileable with a file
    /// </summary>
    public interface IFileBacked : IFileable, IDisposable
    {
        string FilePath { get; }
    }

    /// <summary>
    /// enum indicating whether the decorated functionality is to be extended or completely overridden
    /// </summary>
    public enum FileBackingOptions
    {
        /// <summary>
        /// the decorated fileable is ignored
        /// </summary>
        Override,
        /// <summary>
        /// the decorated fileable is called before the filebacked(ie. overriding) implementation
        /// </summary>
        PreExtend,
        /// <summary>
        /// the decorated fileable is called after the filebacked(ie. overriding) implementation
        /// </summary>
        PostExtend
    }

    /// <summary>
    /// fileable done with a backing file
    /// </summary>
    /// 
    [Serializable]
    public class FileDecoration : FileableDecorationBase, IFileBacked
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public FileDecoration(IFileable decorated, string filePath, FileBackingOptions options = FileBackingOptions.Override)
            : base(decorated)
        {
            Condition.Requires(filePath).IsNotNullOrEmpty();
            this.FilePath = filePath;
            this.Options = options;
        }
        #endregion

        #region ISerializable
        protected FileDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Options = (FileBackingOptions)info.GetValue("Options", typeof(FileBackingOptions));
            this.FilePath = info.GetString("FilePath");
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Options", this.Options);
            info.AddValue("FilePath", this.FilePath);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion
        #region Properties
        FileBackingOptions Options { get; set; }
        public string FilePath { get; private set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IFileable> ApplyThisDecorationTo(IFileable decorated)
        {
            return new FileDecoration(decorated, this.FilePath);
        }
        #endregion

        #region Overrides
        public override string Read()
        {
            if (this.Options == FileBackingOptions.PreExtend)
                this.Decorated.Read();

            var data = File.ReadAllText(this.FilePath);

            if (this.Options == FileBackingOptions.PostExtend)
                this.Decorated.Read();

            return data;
        }
        public override void Write(string text)
        {
            if (this.Options == FileBackingOptions.PreExtend)
                this.Decorated.Write(text);

            File.WriteAllText(text, this.FilePath);

            if (this.Options == FileBackingOptions.PostExtend)
                this.Decorated.Write(text);
        }

        #endregion
    }
}
