using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decoratid.Core
{
    /// <summary>
    /// provides base class implementation of IDisposable, to be inherited from for simplification of disposable idiom
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        #region Declarations
        private bool _isDisposed = false;
        private readonly object _stateLock = new object();
        #endregion

        #region Ctor
        public DisposableBase()
        {

        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
            // Do not make this method virtual.
            // A derived class should not be able to override this method.

            Dispose(true);

            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// IDisposable helper
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            lock (this._stateLock)
            {
                // Check to see if Dispose has already been called.
                if (!this._isDisposed)
                {
                    // If disposing equals true, dispose all managed 
                    // and unmanaged resources.
                    if (disposing)
                    {
                        try
                        {
                            this.DisposeManaged();
                        }
                        catch 
                        {
                        }
                    }
                    // Release unmanaged resources. If disposing is false, 
                    // only the following code is executed.
                    try
                    {
                        this.DisposedUnmanaged();
                    }
                    catch 
                    {
                    }

                }
                this._isDisposed = true;
            }
        }
        protected virtual void DisposeManaged()
        {

        }
        protected virtual void DisposedUnmanaged()
        {

        }
        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method 
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        /// <summary>
        /// Destructor
        /// </summary>
        ~DisposableBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion
    }
}
