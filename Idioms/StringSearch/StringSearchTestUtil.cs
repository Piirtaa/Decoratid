﻿using Decoratid.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoratid.Extensions;
using Gma.DataStructures.StringSearch;

namespace Decoratid.Idioms.StringSearch
{


    public static class StringSearchTestUtil
    {
        public static void Test()
        {
            var testcase = StringSearchTestCase.New(50, 10, 100000);
            Stopwatch stopWatch = new Stopwatch();

            //searcher
            StringSearcher searcher = new StringSearcher();
            stopWatch.Start();
            testcase.TestDictionary.WithEach(x =>
            {
                searcher.Add(x, x);
            });
            stopWatch.Stop();
            Debug.WriteLine("searcher init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Reset();
            stopWatch.Start();
            var searchermatches = searcher.FindMatches(testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("searcher elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, searchermatches.Count())
                );

            stopWatch.Reset();
            stopWatch.Start();
            //trie
            Trie trie = new Trie();
            testcase.TestDictionary.WithEach(x =>
            {
                trie.Add(x, x);
            });
            stopWatch.Stop();
            Debug.WriteLine("trie init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Reset();
            stopWatch.Start();
            var matches = trie.FindMatches(testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("forwardonly elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches.Count())
                );

            stopWatch.Reset();
            stopWatch.Start();
            var matches1b = TrieLogic.FindMatchesUsingForwardOnlyCursor2(trie, testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("forwardonly2 elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches1b.Count())
                );

            stopWatch.Reset();
            stopWatch.Start();
            var matches2 = trie.HasPositionalSearch().FindMatches(testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("seekahead elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches2.Count())
                );

            stopWatch.Reset();
            stopWatch.Start();
            var matches3 = trie.HasPositionalSearch().NonOverlapping().FindMatches(testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("seekaheadnonoverlapped elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches3.Count())
                );

            stopWatch.Reset();
            stopWatch.Start();
            var matches4 = trie.HasPositionalSearch().Paralleling().FindMatches(testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("seekaheadparallel elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches4.Count())
                );

            //stopWatch.Reset();
            //stopWatch.Start();
            //InlineTrie inline_trie = new InlineTrie();
            //stopWatch.Start();
            //testcase.TestDictionary.WithEach(x =>
            //{
            //    inline_trie.Add(x, x);
            //});
            //stopWatch.Stop();
            //Debug.WriteLine("inline init elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            //stopWatch.Reset();
            //stopWatch.Start();
            //var inline_matches = inline_trie.FindMatches(testcase.TestSearchText);
            //stopWatch.Stop();
            //Debug.WriteLine("inline elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            //Debug.WriteLine(
            //    string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, inline_matches.Count)
            //    );

            ////trie
            //ITrie<string> gma_trie =  new PatriciaTrie<string>();
            //stopWatch.Start();
            //testcase.TestDictionary.WithEach(x =>
            //{
            //    gma_trie.Add(x, x);
            //});
            //stopWatch.Stop();
            //Debug.WriteLine("init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

            //stopWatch.Reset();
            //stopWatch.Start();
            //var gma_matches = gma_trie.Retrieve(testcase.TestSearchText);
            //stopWatch.Stop();
            //Debug.WriteLine("elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            //Debug.WriteLine(
            //    string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, gma_matches.Count())
            //    );
            //eeeksoft 
            stopWatch.Reset();
            stopWatch.Start();
            var eeeksoft_searcher = new Decoratid.Idioms.StringSearch.EeekSoft.StringSearch(testcase.TestDictionary.ToArray());
            stopWatch.Stop();
            Debug.WriteLine("eeeksoft init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Reset();
            stopWatch.Start();
            var eeeksoft_matches = eeeksoft_searcher.FindAll(testcase.TestSearchText);
            stopWatch.Stop();
            Debug.WriteLine("eeeksoft elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, eeeksoft_matches.Length)
                );

            //pduniho
            //stopWatch.Reset();
            //stopWatch.Start();
            //var pduniho_graph = new Decoratid.Idioms.StringSearch.PDuniho.StateGraph<char, int>();
            //for (int istr = 0; istr < testcase.TestDictionary.Count; istr++)
            //{
            //    pduniho_graph.Add(testcase.TestDictionary[istr], istr);
            //}
            //stopWatch.Stop();
            //Debug.WriteLine("init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

            //stopWatch.Reset();
            //stopWatch.Start();
            //var pduniho_matches = pduniho_graph.RgobjFromCollectionParallel(testcase.TestSearchText);
            //stopWatch.Stop();
            //Debug.WriteLine("elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            //Debug.WriteLine(
            //    string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, pduniho_matches.Length)
            //    );

            //pnikiforovs
            stopWatch.Reset();
            stopWatch.Start();
            var pnikiforovs_trie = new Decoratid.Idioms.StringSearch.PNikiforovs.Trie();
            testcase.TestDictionary.WithEach(item =>
            {
                pnikiforovs_trie.Add(item);
            });
            pnikiforovs_trie.Build();
            stopWatch.Stop();
            Debug.WriteLine("pnikiforovs init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

            stopWatch.Reset();
            stopWatch.Start();
            string[] pnikiforovs_matches = pnikiforovs_trie.Find(testcase.TestSearchText).ToArray();
            stopWatch.Stop();
            Debug.WriteLine("pnikiforovs elapsed ms {0}", stopWatch.ElapsedMilliseconds);
            Debug.WriteLine(
                string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, pnikiforovs_matches.Length)
                );
        }

      
        //public static void TestDecorationy()
        //{
        //    var dict = StringSearchTestCase.GenerateTestDictionary(1000);
        //    var testlines = StringSearchTestCase.GenerateDecorationySearchText(dict, 20,  100000);
        //    Stopwatch stopWatch = new Stopwatch();

        //    //searcher
        //    StringSearcher searcher = new StringSearcher();
        //    stopWatch.Start();
        //    testcase.TestDictionary.WithEach(x =>
        //    {
        //        searcher.Add(x, x);
        //    });
        //    stopWatch.Stop();
        //    Debug.WriteLine("searcher init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    var searchermatches = searcher.FindMatches(testcase.TestSearchText);
        //    stopWatch.Stop();
        //    Debug.WriteLine("searcher elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    Debug.WriteLine(
        //        string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, searchermatches.Count())
        //        );

        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    //trie
        //    Trie trie = new Trie();
        //    testcase.TestDictionary.WithEach(x =>
        //    {
        //        trie.Add(x, x);
        //    });
        //    stopWatch.Stop();
        //    Debug.WriteLine("trie init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    var matches = trie.FindMatches(testcase.TestSearchText);
        //    stopWatch.Stop();
        //    Debug.WriteLine("forwardonly elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    Debug.WriteLine(
        //        string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches.Count())
        //        );

        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    var matches1b = ForwardOnlyCursorTrieLogic.FindMatches2(trie, testcase.TestSearchText);
        //    stopWatch.Stop();
        //    Debug.WriteLine("forwardonly2 elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    Debug.WriteLine(
        //        string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches1b.Count())
        //        );

        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    var matches2 = trie.UseSeekAhead().FindMatches(testcase.TestSearchText);
        //    stopWatch.Stop();
        //    Debug.WriteLine("seekahead elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    Debug.WriteLine(
        //        string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches2.Count())
        //        );

        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    var matches3 = trie.UseSeekAhead().NonOverlapping().FindMatches(testcase.TestSearchText);
        //    stopWatch.Stop();
        //    Debug.WriteLine("seekaheadnonoverlapped elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    Debug.WriteLine(
        //        string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches3.Count())
        //        );

        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    var matches4 = trie.UseSeekAhead().Paralleling().FindMatches(testcase.TestSearchText);
        //    stopWatch.Stop();
        //    Debug.WriteLine("seekaheadparallel elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    Debug.WriteLine(
        //        string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, matches4.Count())
        //        );

        //    //stopWatch.Reset();
        //    //stopWatch.Start();
        //    //InlineTrie inline_trie = new InlineTrie();
        //    //stopWatch.Start();
        //    //testcase.TestDictionary.WithEach(x =>
        //    //{
        //    //    inline_trie.Add(x, x);
        //    //});
        //    //stopWatch.Stop();
        //    //Debug.WriteLine("inline init elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    //stopWatch.Reset();
        //    //stopWatch.Start();
        //    //var inline_matches = inline_trie.FindMatches(testcase.TestSearchText);
        //    //stopWatch.Stop();
        //    //Debug.WriteLine("inline elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    //Debug.WriteLine(
        //    //    string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, inline_matches.Count)
        //    //    );

        //    ////trie
        //    //ITrie<string> gma_trie =  new PatriciaTrie<string>();
        //    //stopWatch.Start();
        //    //testcase.TestDictionary.WithEach(x =>
        //    //{
        //    //    gma_trie.Add(x, x);
        //    //});
        //    //stopWatch.Stop();
        //    //Debug.WriteLine("init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

        //    //stopWatch.Reset();
        //    //stopWatch.Start();
        //    //var gma_matches = gma_trie.Retrieve(testcase.TestSearchText);
        //    //stopWatch.Stop();
        //    //Debug.WriteLine("elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    //Debug.WriteLine(
        //    //    string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, gma_matches.Count())
        //    //    );
        //    //eeeksoft 
        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    var eeeksoft_searcher = new Decoratid.Idioms.StringSearch.EeekSoft.StringSearch(testcase.TestDictionary.ToArray());
        //    stopWatch.Stop();
        //    Debug.WriteLine("eeeksoft init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    var eeeksoft_matches = eeeksoft_searcher.FindAll(testcase.TestSearchText);
        //    stopWatch.Stop();
        //    Debug.WriteLine("eeeksoft elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    Debug.WriteLine(
        //        string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, eeeksoft_matches.Length)
        //        );

        //    //pduniho
        //    //stopWatch.Reset();
        //    //stopWatch.Start();
        //    //var pduniho_graph = new Decoratid.Idioms.StringSearch.PDuniho.StateGraph<char, int>();
        //    //for (int istr = 0; istr < testcase.TestDictionary.Count; istr++)
        //    //{
        //    //    pduniho_graph.Add(testcase.TestDictionary[istr], istr);
        //    //}
        //    //stopWatch.Stop();
        //    //Debug.WriteLine("init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

        //    //stopWatch.Reset();
        //    //stopWatch.Start();
        //    //var pduniho_matches = pduniho_graph.RgobjFromCollectionParallel(testcase.TestSearchText);
        //    //stopWatch.Stop();
        //    //Debug.WriteLine("elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    //Debug.WriteLine(
        //    //    string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, pduniho_matches.Length)
        //    //    );

        //    //pnikiforovs
        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    var pnikiforovs_trie = new Decoratid.Idioms.StringSearch.PNikiforovs.Trie();
        //    testcase.TestDictionary.WithEach(item =>
        //    {
        //        pnikiforovs_trie.Add(item);
        //    });
        //    pnikiforovs_trie.Build();
        //    stopWatch.Stop();
        //    Debug.WriteLine("pnikiforovs init elapsed ms {0}", stopWatch.ElapsedMilliseconds);

        //    stopWatch.Reset();
        //    stopWatch.Start();
        //    string[] pnikiforovs_matches = pnikiforovs_trie.Find(testcase.TestSearchText).ToArray();
        //    stopWatch.Stop();
        //    Debug.WriteLine("pnikiforovs elapsed ms {0}", stopWatch.ElapsedMilliseconds);
        //    Debug.WriteLine(
        //        string.Format("expected percent {0}.  found {1}", testcase.ExpectedPercentMatch, pnikiforovs_matches.Length)
        //        );
        //}
    }
}
