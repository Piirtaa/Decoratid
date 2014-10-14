using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Utils;
using ServiceStack.Text;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Decoratid.Idioms.Filing
{
    /// <summary>
    /// grabs the file stream and doesn't let go until it is disposed.
    /// </summary>
    [Serializable]
    public class LockingFileDecoration : FileableDecorationBase, IFileBacked
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public LockingFileDecoration(IFileable decorated, string filePath, FileBackingOptions options = FileBackingOptions.Override)
            : base(decorated)
        {
            Condition.Requires(filePath).IsNotNullOrEmpty();
            this.FilePath = filePath;
            this.Options = options;

            // Create a file using the FileStream class.
            this.Stream = FileUtil.GetLockedStream(this.FilePath);
            this.Stream.Lock(0, this.Stream.Length);
        }
        #endregion

        #region ISerializable
        protected LockingFileDecoration(SerializationInfo info, StreamingContext context)
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
        private FileStream Stream { get; set; }
        FileBackingOptions Options { get; set; }
        public string FilePath { get; private set; }
        #endregion

        #region IDecoratedStore
        public override IDecorationOf<IFileable> ApplyThisDecorationTo(IFileable decorated)
        {
            return new LockingFileDecoration(decorated, this.FilePath);
        }
        #endregion

        #region Overrides
        public override string Read()
        {
            if (this.Options == FileBackingOptions.PreExtend)
                this.Decorated.Read();

            string rv = null;

            try
            {
                //read the file
                this.Stream.Position = 0;
                byte[] readBytes = this.Stream.ReadFully();

                //if there are bytes to read we have a json object to parse
                if (readBytes != null && readBytes.Length > 0)
                {
                    rv = Encoding.ASCII.GetString(readBytes);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (this.Options == FileBackingOptions.PostExtend)
                this.Decorated.Read();

            return rv;
        }
        public override void Write(string text)
        {
            if (this.Options == FileBackingOptions.PreExtend)
                this.Decorated.Write(text);

            byte[] messageByte = Encoding.ASCII.GetBytes(text);

            lock (this._stateLock)
            {
                this.Stream.Unlock(0, this.Stream.Length);
                try
                {
                    // Write the bytes to the file.
                    this.Stream.Position = 0;
                    this.Stream.Write(messageByte, 0, messageByte.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    this.Stream.Lock(0, this.Stream.Length);
                }
            }

            if (this.Options == FileBackingOptions.PostExtend)
                this.Decorated.Write(text);
        }
        protected override void DisposeManaged()
        {
            if (this.Stream != null)
            {
                try
                {
                    this.Stream.Unlock(0, this.Stream.Length);
                }
                catch { }

                try
                {
                    this.Stream.Close();
                    this.Stream.Dispose();
                }
                catch { }
            }
            base.DisposeManaged();
        }
        #endregion
    }

    public static class LockingFileDecorationExtensions
    {
        public static LockingFileDecoration LockingFiling(this IFileable decorated, string filePath, FileBackingOptions options = FileBackingOptions.Override)
        {
            Condition.Requires(decorated).IsNotNull();
            return new LockingFileDecoration(decorated, filePath, options);
        }
    }
}
