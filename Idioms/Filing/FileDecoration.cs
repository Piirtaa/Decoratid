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
        public FileDecoration(IFileable decorated, string filePath)
            : base(decorated)
        {
            Condition.Requires(filePath).IsNotNullOrEmpty();
            this.FilePath = filePath;
        }
        #endregion

        #region ISerializable
        protected FileDecoration(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.FilePath = info.GetString("FilePath");
        }
        protected override void ISerializable_GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FilePath", this.FilePath);
            base.ISerializable_GetObjectData(info, context);
        }
        #endregion

        #region Properties
        public string FilePath { get; private set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IFileable> ApplyThisDecorationTo(IFileable decorated)
        {
            return new FileDecoration(decorated, this.FilePath);
        }
        #endregion

        #region Overrides
        public override void Read()
        {
            var data = File.ReadAllText(this.FilePath);
            this.Decorated.Parse(data);
        }
        public override void Write()
        {
            var data = this.Decorated.GetValue();
            File.WriteAllText(this.FilePath, data);
        }

        #endregion
    }

    public static class FileDecorationExtensions
    {
        public static FileDecoration Filing(this IFileable decorated, string filePath)
        {
            Condition.Requires(decorated).IsNotNull();
            return new FileDecoration(decorated, filePath);
        }
    }
}
