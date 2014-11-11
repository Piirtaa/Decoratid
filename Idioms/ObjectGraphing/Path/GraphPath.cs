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
        public const string DELIM = ".";

        #region Ctor
        private GraphPath()
        {
            this.Segments = new List<IGraphSegment>();
        }
        #endregion

        #region Properties
        public List<IGraphSegment> Segments { get; private set; }
        #endregion

        #region Calculated Properties
        public bool IsRoot
        {
            get
            {
                if (object.ReferenceEquals(this.CurrentSegment, this.Segments.First()))
                    return true;

                return false;
            }
        }
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

        #endregion

        #region Methods
        public GraphPath AddSegment(IGraphSegment segment)
        {
            Condition.Requires(segment).IsNotNull();

            this.Segments.Add(segment);

            return this;
        }
        /// <summary>
        /// only works on 
        /// </summary>
        /// <param name="name"></param>
        public void ChangeCurrentSegmentPath(string path)
        {
            if (this.CurrentSegment is GraphSegment)
            {
                ((GraphSegment)this.CurrentSegment).SetPath(path);
            }
        }
        #endregion

        #region Implicit Conversion to string
        public static implicit operator GraphPath(string text)
        {
            var stringlist = NaturalStringableList.New().Delimit(PREFIX, DELIM, SUFFIX);
            stringlist.Parse(text);

            List<IGraphSegment> list = new List<IGraphSegment>();
            stringlist.WithEach(seg =>
            {
                if (seg.StartsWith(EnumeratedItemSegment.PREFIX))
                {
                    EnumeratedItemSegment iSeg = seg;
                    list.Add(iSeg);
                }
                else
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
            var segs = path.Segments.GetRange(0, path.Segments.Count);
            segs.WithEach(x =>
            {
                rv.AddSegment(x);
            });
            return rv;
        }
        public static GraphPath New(List<IGraphSegment> paths)
        {
            Condition.Requires(paths).IsNotEmpty();
            var rv = new GraphPath();
            var segs = paths.GetRange(0, paths.Count);
            segs.WithEach(x =>
            {
                rv.AddSegment(x);
            });
            return rv;
        }
        #endregion
    }
}
