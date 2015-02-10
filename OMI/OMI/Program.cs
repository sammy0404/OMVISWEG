using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI
{
    class Program
    {
        List<KeyValuePair<int, object>> testData = new List<KeyValuePair<int, object>>();

        static void Main(string[] args)
        {
            Program p = new Program();
            
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
                default:
                    DS = new MinMaxHeap();
                    break;
            }

            DS.Build(p.testData);
            Random rnd = new Random();
            for(int s = 0; s<10; s++)
            {
                var kvp = p.testData[rnd.Next(p.testData.Count)];
                bool found = kvp.Equals(DS.Search(kvp.Key));
                Console.WriteLine(found + " " + kvp.Value);
            }
            
            Console.Read();
        }

        private void seedTestData()
        {
            // Ik ga ervan uit dat we distinct keys gebruiken.
            HashSet<KeyValuePair<int, object>> HS = new HashSet<KeyValuePair<int, object>>();
            Random r = new Random();
            while(HS.Count < 25)
            {
                int key = r.Next(0, 1000);
                string value = key.ToString();
                HS.Add(new KeyValuePair<int, object>(key, value));
            }
            testData.AddRange(HS);
        }
    }
}
