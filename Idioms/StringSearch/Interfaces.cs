using Decoratid.Core.Identifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.StringSearch
{
    /// <summary>
    /// the basic definition of a string searcher.  you give it some strings(with lookup values), AKA "Words" and 
    /// search using this dictionary for word matches within the provided text.
    /// </summary>
    public interface IStringSearcher 
    {
        void Add(string word, object value);
        List<StringSearchMatch> FindMatches(string text);
    }

    /// <summary>
    /// able to do a string search for matches beginning at the provided index.
    /// </summary>
    /// <remarks>
    /// This alg allows us to slice up search work by index, opening the option of 
    /// parallelization, seekahead, non-overlappingness
    /// </remarks>
    public interface IByPositionStringSearcher : IStringSearcher
    {
        /// <summary>
        /// string search for matches beginning at the provided index.
        /// returns list to account for situation where matches share a common suffix.
        /// </summary>
        List<StringSearchMatch> FindMatchesAtPosition(int idx, string text, out int graspLengthOUT);
    }








}
