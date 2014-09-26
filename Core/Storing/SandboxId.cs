using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Decoratid.Thingness;

namespace Decoratid.Idioms.Storing
{

    /// <summary>
    /// The default Id scheme
    /// </summary>
    /// <remarks>
    /// Consists of:
    /// Date - DateCreated
    /// Guid - System generated
    /// int - version number - not used for equivalence
    /// string - tag - not used for equivalence
    /// string - random 
    /// </remarks>
    public class DecoratidId : IHasId<Guid>, IHasDateCreated
    {
        #region Declarations
        private static IRandomStringGenerator _gen = new StringGenerator();
        private readonly DateTime _dateCreated;
        private readonly Guid _id;
        private int _version;
        private readonly string _tag;
        private readonly string _random;
        #endregion

        #region Ctor
        public DecoratidId()
        {
            this._dateCreated = DateTime.UtcNow;
            this._id = Guid.NewGuid();
            this._version = 0;
            this._tag = null;
            this._random = _gen.Generate(50);
        }

        /// <summary>
        /// creates a new id
        /// </summary>
        /// <param name="tag"></param>
        public DecoratidId(string tag = null)
        {
            this._dateCreated = DateTime.UtcNow;
            this._id = Guid.NewGuid();
            this._version = 0;
            this._tag = tag;
            this._random = _gen.Generate(50);
        }
        private DecoratidId(List<string> delimited)
        {
            Condition.Requires(delimited).IsNotNull();

            this._dateCreated = DateTime.Parse(delimited[0]);
            this._id = Guid.Parse(delimited[1]);
            this._version = int.Parse(delimited[2]);
            this._tag = delimited[3];
            this._random = delimited[4];
        }
        #endregion

        #region Properties
        public DateTime DateCreated { get { return this._dateCreated; } }
        public Guid Id { get { return this._id; } }
        public int Version { get { return this._version; } }
        public string Tag { get { return this._tag; } }
        public string Random { get { return this._random; } }
        object IHasId.Id
        {
            get { return this.Id; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// increments the version number
        /// </summary>
        public void IncrementVersion()
        {
            this._version++;
        }
        #endregion

        #region Overrides
        private string GetIdString()
        {
            List<string> data = new List<string>(){
                this.DateCreated.ToString(),
                this.Id.ToString(),
                this.Random
            };

            return string.Join("|", data.ToArray());
        }
        public override string ToString()
        {
            List<string> data = new List<string>(){
                this.DateCreated.ToString(),
                this.Id.ToString(),
                this.Version.ToString(),
                this.Tag,
                this.Random
            };

            return string.Join("|", data.ToArray());
        }
        public override int GetHashCode()
        {
            return this.GetIdString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is DecoratidId))
                return false;

            DecoratidId sor = (DecoratidId)obj;

            return sor.GetIdString().Equals(this.GetIdString());
        }
        #endregion

        #region Conversions
        public static implicit operator DecoratidId(string obj)
        {
            return new DecoratidId(obj.Split('|').ToList());
        }
        public static implicit operator string(DecoratidId obj)
        {
            if (obj == null)
                return null;

            return obj.ToString();
        }
        #endregion
    }
}
