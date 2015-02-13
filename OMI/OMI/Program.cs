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
        List<KeyValuePair<int, object>> testData = new List<KeyValuePair<int, object>>();
        static int nrOfSearches  = 100000;
        static int nrOfKeys      = 1000000;
        static int upperKeyBound = 5000000;

        static void Main(string[] args)
        {
            Program p = new Program();
            Stopwatch sw = new Stopwatch();

            p.seedTestData();
            IDatastructure DS = null;
            switch (Console.ReadLine()[0])
            {
                case 'T':
                    DS = new AVLTree();
                    break;
                case 'L':
                    DS = new List();
                    break;
                case 'H':
                    DS = new HashTable(60);
                    break;
                default:
                    DS = new MinMaxHeap();
                    break;
            }

            sw.Start();
            DS.Build(p.testData);
            sw.Stop();
            Console.WriteLine("Created {0} items in the {1} in {2} ms",
                                p.testData.Count,
                                DS.GetType().Name,
                                sw.ElapsedMilliseconds);
            
            Random rnd = new Random();
			bool found = true;
			sw.Restart();
            for (int s = 0; s < nrOfSearches; s++)
            {
                var kvp = p.testData[rnd.Next(p.testData.Count)];
                found = kvp.Equals(DS.Search(kvp.Key));
				if (!found)
				{
					Console.WriteLine("Defective search, plzz fix");
					break;
				}
					
                //Console.WriteLine(found + " " + kvp.Value);
                //Console.WriteLine("minimum: " + DS.ExtractMin().Value + "; maximum: " + DS.ExtractMax().Value);
            }
            sw.Stop();
            Console.WriteLine("Found {0} items in the {1} in {2} ms",
                                nrOfSearches,
                                DS.GetType().Name,
                                sw.ElapsedMilliseconds);
            Console.Read();
        }

        private void seedTestData()
        {
            // Ik ga ervan uit dat we distinct keys gebruiken.
            HashSet<KeyValuePair<int, object>> HS = new HashSet<KeyValuePair<int, object>>();
            Random r = new Random();
            while (HS.Count < nrOfKeys)
            {
                int key = r.Next(0, upperKeyBound);
                string value = key.ToString();
                HS.Add(new KeyValuePair<int, object>(key, value));
            }
            testData.AddRange(HS);
        }
    }
}
