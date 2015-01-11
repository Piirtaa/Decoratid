using CuttingEdge.Conditions;
using Decoratid.Core.Decorating;
using Decoratid.Core.Identifying;
using Decoratid.Core.Storing;
using Decoratid.Idioms.Stringing;
using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Decoratid.Idioms.Identifying
{

    /// <summary>
    /// defines some standard id scheme to use that is well-suited for distribution. 
    /// TODO: think about this a bit
    /// </summary>
    public interface IDistributedId : IHasId, IHasDateCreated, IHasGUID, IHasMachineName, IHasIP
    {
    }

    public class DistributedId : DecoratedHasIdBase, IDistributedId
    {
        #region Ctor
        public DistributedId(IHasId decorated, DateTime dateCreated, Guid guid, string localMachine, IPAddress addr) 
            :base(decorated.HasDateCreated(dateCreated).HasGUID(guid).HasMachineName(localMachine).HasIP(addr))
        {
        }
        private DistributedId()
            :base(AsId<string>.New("distributedid").HasNewDateCreated().HasAutomaticGUID().HasLocalMachineName().HasLocalIP())
        {

        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            List<string> data = new List<string>(){
                this.Id.ToString(),
                this.DateCreated.ToString(),
                this.GUID.ToString(),
                this.MachineName,
                this.IPAddress.ToString()
            };

            return LengthEncoder.LengthEncodeList(data.ToArray());
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is DistributedId))
                return false;

            DistributedId sor = (DistributedId)obj;

            return sor.ToString().Equals(this.ToString());
        }
        #endregion

        #region Decorations
        public DateTime DateCreated
        {
            get
            {
                return this.As<HasDateCreatedDecoration>(true).DateCreated;
            }
            set
            {
                this.As<HasDateCreatedDecoration>(true).DateCreated = value;
            }
        }

        public Guid GUID
        {
            get
            {
                return this.As<HasGUIDDecoration>(true).GUID;
            }
            set
            {
                this.As<HasGUIDDecoration>(true).GUID = value;
            }
        }

        public string MachineName
        {
            get
            {
                return this.As<HasMachineNameDecoration>(true).MachineName;
            }
            set
            {
                this.As<HasMachineNameDecoration>(true).MachineName = value;
            }
        }

        public System.Net.IPAddress IPAddress
        {
            get
            {
                return this.As<HasIPDecoration>(true).IPAddress;
            }
            set
            {
                this.As<HasIPDecoration>(true).IPAddress = value;
            }
        }
        #endregion

        #region Overrides
        public override IDecorationOf<IHasId> ApplyThisDecorationTo(IHasId thing)
        {
            return new DistributedId(thing, this.DateCreated, this.GUID, this.MachineName, this.IPAddress);
        }
        #endregion

        #region Static Fluent
        /// <summary>
        /// returns a new, automatically created id
        /// </summary>
        /// <returns></returns>
        public static DistributedId New()
        {
            return new DistributedId();
        }

        #endregion
    }
}
