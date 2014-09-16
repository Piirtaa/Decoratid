using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;
using ServiceStack.Text;
using Decoratid.Extensions;
using System.IO;
using System.Security.AccessControl;
using System.Security;
using Decoratid.Thingness.File;
using Decoratid.Utils;

namespace Decoratid.Idioms
{
    /// <summary>
    /// wraps a file and exposes methods to manipulate the file, but keeps it locked so only this class instance can manipulate 
    /// the file - it's otherwise locked from all other access.  
    /// </summary>
    public class LockedFile : DisposableBase
    {
        #region Declarations
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public LockedFile(string filePath)
        {
            Condition.Requires(filePath).IsNotNullOrEmpty();

            this.FilePath = filePath;
            FileSecurity fs = new FileSecurity();

            // Create a file using the FileStream class.
            this.Stream = FileUtil.GetLockedStream(this.FilePath);
            this.Stream.Lock(0, this.Stream.Length);
        }
        #endregion

        #region Properties
        private FileStream Stream { get;  set; }
        public string FilePath { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// we return secure string
        /// </summary>
        /// <returns></returns>
        public SecureString Read()
        {
            //secure string
            try
            {
                //read the file
                this.Stream.Position = 0;
                byte[] readBytes = this.Stream.ReadFully();
            
                //if there are bytes to read we have a json object to parse
                if (readBytes != null && readBytes.Length > 0)
                {
                    var text = Encoding.ASCII.GetString(readBytes);
                    return SecureStringUtil.ToSecureString(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return null;
        }
        public void Write(string text)
        {   
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
        }
        #endregion

        #region Overrides
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
}
