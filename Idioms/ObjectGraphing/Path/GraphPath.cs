using CuttingEdge.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Decoratid.Idioms.Stringing;

namespace Decoratid.Idioms.ObjectGraphing.Path
{

    /// <summary>
    /// describes a set of contiguous segments defining a path to a node
    /// </summary>
    public sealed class GraphPath
    {
        public const string PREFIX = "";
        public const string SUFFIX = "";
        public const string DELIM = "|";
        
        #region Ctor
        private GraphPath()
        {
            this.Segments = new List<IGraphSegment>();
            this.Segments.Add(RootSegment.New());
        }
        #endregion

        #region Properties
        public List<IGraphSegment> Segments { get; private set; }
        #endregion

        #region Calculated Properties
        public string Path
        {
            get
            {
                var list = this.Segments.Select((x) => { return x.Path; });
                var stringlist = NaturalStringableList.New().Delimit(PREFIX, DELIM, SUFFIX);

                list.WithEach(item =>
                {
                    stringlist.Add(item);
                });

                var path = stringlist.GetValue();
                return path;
            }
        }
        public string ParentPath
        {
            get
            {
                var list = this.Segments.Select((x) => { return x.Path; }).ToList();

                //remove the last element
                list.RemoveAt(list.Count - 1);

                var stringlist = NaturalStringableList.New().Delimit(PREFIX, DELIM, SUFFIX);

                list.WithEach(item =>
                {
                    stringlist.Add(item);
                });

                var path = stringlist.GetValue();
                return path;
            }
        }
        public IGraphSegment CurrentSegment
        {
            get
            {
                return this.Segments.Last();
            }
        }
        public bool IsEnumeratedSegment
        {
            get { return this.CurrentSegment is EnumeratedItemSegment; }
        }
        /// <summary>
        /// if the current segment is enumerated returns the index, otherwise -1
        /// </summary>
        public int EnumeratedSegmentIndex
        {
            get
            {
                if (this.IsEnumeratedSegment)
                    return ((EnumeratedItemSegment)this.CurrentSegment).Index;

                return -1;
            }
        }
        /// <summary>
        /// if the current segment is enumerated returns the type, otherwise IList
        /// </summary>
        public EnumeratedSegmentType EnumeratedSegmentType
        {
            get
            {
                if (this.IsEnumeratedSegment)
                    return ((EnumeratedItemSegment)this.CurrentSegment).SegmentType;

                return EnumeratedSegmentType.None;
            }
        }
        #endregion

        #region Methods
        public GraphPath AddSegment(IGraphSegment segment)
        {
            Condition.Requires(segment).IsNotNull();

            if (segment is RootSegment)
                throw new InvalidOperationException();

            this.Segments.Add(segment);

            return this;
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator GraphPath(string text)
        {
            /*Note: this parsing scheme depends on unique prefixing of each node type, and unique delim at the graphpath level
             * 
             * 
             */ 
            Condition.Requires(text).IsNotNullOrEmpty();

            var stringlist = NaturalStringableList.New().Delimit(PREFIX, DELIM, SUFFIX);
            stringlist.Parse(text);

            List<IGraphSegment> list = new List<IGraphSegment>();
            stringlist.WithEach(seg =>
            {
                if (seg.StartsWith(RootSegment.PREFIX))
                {
                    RootSegment rSeg = seg;
                    list.Add(rSeg);
                }
                else if (seg.StartsWith(EnumeratedItemSegment.PREFIX))
                {
                    EnumeratedItemSegment iSeg = seg;
                    list.Add(iSeg);
                }
                else if (seg.StartsWith(GraphSegment.PREFIX))
                {
                    GraphSegment gSeg = seg;
                    list.Add(gSeg);
                }
            });
            var rv = GraphPath.New(list);
            return rv;
        }
        public static implicit operator string(GraphPath obj)
        {
            if (obj == null)
                return null;

            return obj.Path;
        }
        #endregion

        #region Fluent Static
        public static GraphPath New()
        {
            var rv = new GraphPath();
            return rv;
        }
        public static GraphPath New(GraphPath path)
        {
            Condition.Requires(path).IsNotNull();
            var rv = new GraphPath();
            var segs = path.Segments.GetRange(1, path.Segments.Count - 1);
            segs.WithEach(x =>
            {
                rv.AddSegment(x);
            });
            return rv;
        }
        public static GraphPath New(List<IGraphSegment> paths)
        {
            Condition.Requires(paths).IsNotEmpty();
            RootSegment rootSeg = paths[0] as RootSegment;
            var rv = new GraphPath();
            var segs = paths.GetRange(1, paths.Count - 1);
            segs.WithEach(x =>
            {
                rv.AddSegment(x);
            });
            return rv;
        }
        #endregion
    }
}
