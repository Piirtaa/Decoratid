﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.HasBitsing
{
    public static class BitTweakerTester
    {
        /// <summary>
        /// for a given set, runs the filter and outputs the run time. attempts single thread, parallel, and custom threadpool
        /// of 1, 2, 4, 8 threads to compare results
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="filter"></param>
        /// <param name="remarks"></param>
        public static void TestFilterTimes(List<IHasBits> testData, Func<IHasBits, bool> filter, string remarks)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Tests starting for {0}.  {1} items" + Environment.NewLine, remarks, testData.Count);

            Tuple<string, long> bestPerf;

            var sw = Stopwatch.StartNew();
            var newList = testData.FilterList(filter);
            sw.Stop();
            sb.AppendFormat("regular. elapsed ms {0}.  matched {1}" + Environment.NewLine, sw.ElapsedMilliseconds, newList.Count);
            bestPerf = new Tuple<string, long>("regular", sw.ElapsedMilliseconds); 
            
            sw.Reset();
            sw.Start();
            newList = testData.FilterList_UsingParallel(filter);
            sw.Stop();
            sb.AppendFormat("parallel. elapsed ms {0}.  matched {1}" + Environment.NewLine, sw.ElapsedMilliseconds, newList.Count);
            if (sw.ElapsedMilliseconds < bestPerf.Item2)
                bestPerf = new Tuple<string, long>("parallel", sw.ElapsedMilliseconds);

            //newList.ForEach(x =>
            //{
            //    Debug.WriteLine("regular parallel match {0}", x.Bits.ToString());
            //});

            List<int> workerCounts = new List<int> { 1, 2, 4, 8 };
            foreach (var each in workerCounts)
            {
                sw.Reset();
                sw.Start();
                newList = testData.FilterList_UsingWorkers(filter, each);
                sw.Stop();
                sb.AppendFormat("{2} workers. elapsed ms {0}.  matched {1}" + Environment.NewLine, sw.ElapsedMilliseconds, newList.Count, each);
                if (sw.ElapsedMilliseconds < bestPerf.Item2)
                    bestPerf = new Tuple<string, long>(each + " workers", sw.ElapsedMilliseconds);
                //newList.ForEach(x =>
                //{
                //    Debug.WriteLine("match {0}", x.Bits.ToString());
                //});
            }

            sb.AppendFormat("best perf {0} {1}" + Environment.NewLine, bestPerf.Item1, bestPerf.Item2);

            Debug.Write(sb.ToString());
        }

        /// <summary>
        /// tests different hasbits implementations and different filters to perform the same operation
        /// </summary>
        public static void TestHasBits()
        {
            var records = MockBitArrays.GenerateRandomBitArrays(1000000);
            var lastRecord = records.Last();

            List<IHasBits> hasBitsInt = new List<IHasBits>();
            List<IHasBits> hasBitsBoolArray = new List<IHasBits>();
            List<IHasBits> hasBitsBitVector = new List<IHasBits>();
            List<IHasBits> hasBitsBitArray = new List<IHasBits>();
            NaturalIntHasBits last1 = new NaturalIntHasBits(lastRecord);
            NaturalBoolArrayHasBits last2 = new NaturalBoolArrayHasBits(lastRecord);
            NaturalBitVectorHasBits last3 = new NaturalBitVectorHasBits(lastRecord);
            NaturalBitArrayHasBits last4 = new NaturalBitArrayHasBits(lastRecord);

            records.ForEach(x =>
            {
                hasBitsInt.Add(new NaturalIntHasBits(x));
                hasBitsBoolArray.Add(new NaturalBoolArrayHasBits(x));
                hasBitsBitVector.Add(new NaturalBitVectorHasBits(x));
                hasBitsBitArray.Add(new NaturalBitArrayHasBits(x));
            });

            Func<IHasBits, bool> findLastFilter_XOR = (x) =>
            {
                return x.Bits.AreEquivalent_XORCompare(lastRecord);
            };
            Func<IHasBits, bool> findLastFilter_BitByBit = (x) =>
            {
                return x.Bits.AreEquivalent_BitByBit(lastRecord);
            };
            Func<IHasBits, bool> findLastFilter_IntCompare = (x) =>
            {
                return x.Bits.AreEquivalent_IntCompare(lastRecord);
            };

            TestFilterTimes(hasBitsInt, findLastFilter_XOR, "int | XOR");
            TestFilterTimes(hasBitsBoolArray, findLastFilter_XOR, "boolarray | XOR");
            TestFilterTimes(hasBitsBitVector, findLastFilter_XOR, "bitvector | XOR");
            TestFilterTimes(hasBitsBitArray, findLastFilter_XOR, "bitarray | XOR");
            TestFilterTimes(hasBitsInt, findLastFilter_BitByBit, "int | bit by bit");
            TestFilterTimes(hasBitsBoolArray, findLastFilter_BitByBit, "boolarray | bit by bit");
            TestFilterTimes(hasBitsBitVector, findLastFilter_BitByBit, "bitvector | bit by bit");
            TestFilterTimes(hasBitsBitArray, findLastFilter_BitByBit, "bitarray | bit by bit");
            TestFilterTimes(hasBitsInt, findLastFilter_IntCompare, "int | int compare");
            TestFilterTimes(hasBitsBoolArray, findLastFilter_IntCompare, "boolarray | int compare");
            TestFilterTimes(hasBitsBitVector, findLastFilter_IntCompare, "bitvector | int compare");
            TestFilterTimes(hasBitsBitArray, findLastFilter_IntCompare, "bitarray | int compare");

            //apparently bitarray intcompare is the fastest arrangement

            //now try the AND
            //Func<IHasBits, bool> findLastFilter_AND = (x) =>
            //{
            //    var newBits = x.AND(last1);

            //    return newBits.Bits.AreEquivalent_IntCompare(lastRecord);
            //};


        }
        //public static void TestLookupTime()
        //{

        //    int numRecords = 10000000;
        //    Random rnd = new Random();

        //    //create the data to look for
        //    var needleInHaystack = rnd.Next(int.MaxValue);
        //    var needleBA = needleInHaystack.GetBitArrayFromInt();

        //    //create a bunch of data to match against
        //    List<Tuple<BitArray, int>> records = new List<Tuple<BitArray, int>>();

        //    for (int i = 0; i < numRecords; i++)
        //    {
        //        var itemVal = rnd.Next(int.MaxValue);

        //        while (itemVal == needleInHaystack)
        //            itemVal = rnd.Next(int.MaxValue);

        //        var ba = itemVal.GetBitArrayFromInt();

        //        records.Add(new Tuple<BitArray, int>(ba, i));
        //    }
        //    records.Add(new Tuple<BitArray, int>(needleBA, numRecords)); //put the match at the bottom

        //    //test Int-based tweaking
        //    List<Tuple<int, int>> testInts = new List<Tuple<int, int>>();
        //    records.ForEach(x =>
        //    {
        //        testInts.Add(new Tuple<int, int>(x.Item1.GetIntFromBitArray(), x.Item2));
        //    });


        //    Parallel.For(0, maxIndex, () => new List<StringSearchMatch>(),
        //        (x, loop, subList) =>
        //        {
        //            int grasp;
        //            var list = (this.Decorated as IByPositionStringSearcher).FindMatchesAtPosition(x, text, out grasp);
        //            subList.AddRange(list);
        //            return subList;
        //        },
        //        (x) => { rv.AddRange(x); }
        //    );

        //    //get some tweakers to test against
        //    List<IHasBitsing> tweakers = new List<IHasBitsing>();
        //    tweakers.Add(new IntBitTweaker(needleBA));
        //    tweakers.Add(new FixedArrayBitTweaker(needleBA));
        //    tweakers.Add(new BitVectorTweaker(needleBA));

        //    foreach (IHasBitsing tweaker in tweakers)
        //    {
        //        var sw = Stopwatch.StartNew();




        //        sw.Stop();
        //        Debug.WriteLine("elapsed ms {0}", sw.ElapsedMilliseconds);

        //    }

        //}
    }
}
