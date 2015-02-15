using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OMI
{
    class Program
    {
        IDatastructure DS = null;
        List<KeyValuePair<int, object>> testData;
        List<int> deletionKeys;

        static int nrOfSearches  = 100000;
        static int nrOfKeys      = 1000000;
        static int upperKeyBound = 5000000;
        static int nrOfDeletions = 10000;

        static void Main(string[] args)
        {
            Program p = new Program();
            Stopwatch sw = new Stopwatch();
            IDatastructure DS = null;

            while (true)
            {
                p.seedTestData();
                switch (Console.ReadLine().ToUpper()[0])
                {
                    case 'X':
                        return;
                    case 'T':
                        DS = new AVLTree();                        
                        break;
                    case 'L':
                        DS = new List();
                        break;
                    case 'H':
                        DS = new HashTable(nrOfKeys);
                        break;
                    default:
                        DS = new MinMaxHeap(nrOfKeys);
                        break;
                }
                p.DS = DS;

                sw.Restart();
                DS.Build(p.testData);
                sw.Stop();
                Console.WriteLine("Created {0} items in the {1} in {2} ms",
                                    p.testData.Count,
                                    DS.GetType().Name,
                                    sw.ElapsedMilliseconds);

                sw.Restart();
                p.Search();
                sw.Stop();
                Console.WriteLine("Found {0} items in the {1} in {2} ms",
                                    nrOfSearches,
                                    DS.GetType().Name,
                                    sw.ElapsedMilliseconds);

                p.seedDeletionKeys();
                sw.Restart();
                p.Delete();
                sw.Stop();
                Console.WriteLine("Deleted {0} items in the {1} in {2} ms",
                                    nrOfDeletions,
                                    DS.GetType().Name,
                                    sw.ElapsedMilliseconds);
            }
        }

        private void Search()
        {
            Random rnd = new Random();
            bool found = true;
            for (int s = 0; s < nrOfSearches; s++)
            {
                var kvp = testData[rnd.Next(testData.Count)];
                found = kvp.Equals(DS.Search(kvp.Key));
                if (!found)
                {
                    Console.WriteLine("Defective search, plzz fix");
                    break;
                }
            }
        }

        private void Delete()
        {
            foreach (int key in deletionKeys)
            {
                if (!DS.TryDelete(key))
                {
                    Console.WriteLine("Delete function defective, plzz fix");
                    break;
                }
            }
        }

        private void seedDeletionKeys()
        {
            KeyValuePair<int, object>[] list = new KeyValuePair<int, object>[nrOfDeletions];
            testData.CopyTo(0, list, 0, nrOfDeletions);
            deletionKeys = new List<int>(list.Select(x => x.Key));
        }

        private void seedTestData()
        {
            var HS = new HashSet<KeyValuePair<int, object>>();
            Random r = new Random();
            while (HS.Count < nrOfKeys)
            {
                int key = r.Next(0, upperKeyBound);
                string value = key.ToString();
                HS.Add(new KeyValuePair<int, object>(key, value));
            }
            testData = new List<KeyValuePair<int,object>>(HS);
        }


    }
}
