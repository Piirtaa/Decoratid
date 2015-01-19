using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Idioms.BitTweaking
{
    public static class BitTweakerTester
    {
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

        public static void TestHasBits()
        {
            var records = BitMocker.GenerateRandomBitArrays(1000000);
            var lastRecord = records.Last();

            List<IHasBits> hasBits1 = new List<IHasBits>();
            List<IHasBits> hasBits2 = new List<IHasBits>();
            List<IHasBits> hasBits3 = new List<IHasBits>();
            List<IHasBits> hasBits4 = new List<IHasBits>();
            IntHasBits last1 = new IntHasBits(lastRecord);
            BoolArrayHasBits last2 = new BoolArrayHasBits(lastRecord);
            BitVectorHasBits last3 = new BitVectorHasBits(lastRecord);
            BitArrayHasBits last4 = new BitArrayHasBits(lastRecord);

            records.ForEach(x =>
            {
                hasBits1.Add(new IntHasBits(x));
                hasBits2.Add(new BoolArrayHasBits(x));
                hasBits3.Add(new BitVectorHasBits(x));
                hasBits4.Add(new BitArrayHasBits(x));
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

            TestFilterTimes(hasBits1, findLastFilter_XOR, "int | XOR");
            TestFilterTimes(hasBits2, findLastFilter_XOR, "boolarray | XOR");
            TestFilterTimes(hasBits3, findLastFilter_XOR, "bitvector | XOR");
            TestFilterTimes(hasBits4, findLastFilter_XOR, "bitarray | XOR");
            TestFilterTimes(hasBits1, findLastFilter_BitByBit, "int | bit by bit");
            TestFilterTimes(hasBits2, findLastFilter_BitByBit, "boolarray | bit by bit");
            TestFilterTimes(hasBits3, findLastFilter_BitByBit, "bitvector | bit by bit");
            TestFilterTimes(hasBits4, findLastFilter_BitByBit, "bitarray | bit by bit");
            TestFilterTimes(hasBits1, findLastFilter_IntCompare, "int | int compare");
            TestFilterTimes(hasBits2, findLastFilter_IntCompare, "boolarray | int compare");
            TestFilterTimes(hasBits3, findLastFilter_IntCompare, "bitvector | int compare");
            TestFilterTimes(hasBits4, findLastFilter_IntCompare, "bitarray | int compare");
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
        //    List<IBitTweaking> tweakers = new List<IBitTweaking>();
        //    tweakers.Add(new IntBitTweaker(needleBA));
        //    tweakers.Add(new FixedArrayBitTweaker(needleBA));
        //    tweakers.Add(new BitVectorTweaker(needleBA));

        //    foreach (IBitTweaking tweaker in tweakers)
        //    {
        //        var sw = Stopwatch.StartNew();




        //        sw.Stop();
        //        Debug.WriteLine("elapsed ms {0}", sw.ElapsedMilliseconds);

        //    }

        //}
    }
}
