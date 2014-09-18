using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandbox.Store.ConcurrencyVersioning
{
    [Serializable]
    public class IncrementalVersionOfInt : IIncrementalVersionOf<int>
    {
        #region Ctor
        public IncrementalVersionOfInt() { }
        public IncrementalVersionOfInt(int versionNumber)
        {
            this.VersionNumber = versionNumber;
        }
        #endregion

        #region ISerializable
        // The special constructor is used to deserialize values. 
        public IncrementalVersionOfInt(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            this.VersionNumber = info.GetInt32("VersionNumber");
        }
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("VersionNumber", this.VersionNumber, typeof(Int32));
        }
        #endregion

        #region Properties
        public int VersionNumber { get; set; }
        #endregion

        #region IVersionInfoOf
        public int Compare(int x, int y)
        {
            return x.CompareTo(y);
        }

        public int CompareTo(object obj)
        {
            return this.VersionNumber.CompareTo(obj);
        }

        public int CompareTo(int other)
        {
            return this.VersionNumber.CompareTo(other);
        }

        public bool Equals(int other)
        {
            return this.VersionNumber.Equals(other);
        }

        public void Increment()
        {
            lock (this)
            {
                this.VersionNumber = this.VersionNumber + 1;
            }
        }        
        public string GetVersionText()
        {
            return this.VersionNumber.ToString();
        }
        #endregion





    }
}
