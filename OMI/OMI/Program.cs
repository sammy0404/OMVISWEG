using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using OfficeOpenXml;
using System.Drawing;
using System.Threading;

namespace OMI
{
    class Program
    {
        IDatastructure DS = null;
		static ExcelPackage package;
		static ExcelWorksheet workbook;

        static int nrOfSearches  = 100000;
        static int nrOfKeys      = 1000000;
        static int upperKeyBound = 5000000;
        static int nrOfDeletions = 10000;

		static int[] dataSetSizes = {10000, 100000, 1000000};

        static void Main(string[] args)
        {
			package = new ExcelPackage(new MemoryStream());
			workbook = package.Workbook.Worksheets.Add("Worksheet1");


			Test();
			List<KeyValuePair<int, object>> testData = null;
			List<int> deletionKeys = null;
            Program p = new Program();
            Stopwatch sw = new Stopwatch();
            IDatastructure DS = null;

            while (true)
            {
                seedTestData(nrOfKeys);
                Console.WriteLine("Press T,L,H or I to test DS or X to terminate.");
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
                    case 'I':
                        DS = new IntervalHiep();
                        break;
                    default:
                        return;
                }
                p.DS = DS;

                sw.Restart();
                DS.Build(testData);
                sw.Stop();
                Console.WriteLine("Created {0} items in the {1} in {2} ms",
                                    testData.Count,
                                    DS.GetType().Name,
                                    sw.ElapsedMilliseconds);
                int min = int.MinValue;
                int i = 0;
                while (i < nrOfKeys/2)
                {
                    int newMin = DS.GetMin().Key;
                    if (newMin < min)
                        min++;
                    min = newMin;
                    i++;
                }

                int max = int.MaxValue;
                int j = 0;
                while (j < nrOfKeys/2)
                {
                    int newMax = DS.GetMax().Key;
                    if (newMax > max)
                        max++;
                    max = newMax;
                    j++;
                }
                sw.Restart();
                p.Search(testData);
                sw.Stop();
                Console.WriteLine("Found {0} items in the {1} in {2} ms",
                                    nrOfSearches,
                                    DS.GetType().Name,
                                    sw.ElapsedMilliseconds);

                seedDeletionKeys(testData);
                sw.Restart();
                p.Delete(deletionKeys);
                sw.Stop();
                Console.WriteLine("Deleted {0} items in the {1} in {2} ms",
                                    nrOfDeletions,
                                    DS.GetType().Name,
                                    sw.ElapsedMilliseconds);
            }
        }

		public static void Test()
		{
			////////////////////////////////// INIT

			package.Workbook.Properties.Title = "OMI, Groep 1B, Statistiek";
			package.Workbook.Properties.Author = "Gerben Aalvanger, William Kos, Erik Visser en Sam van der Wal";

			int repeatTestSize = 1; // 30
			int repeatSetSize = 1;  // 5

			Color setSizeColor = Color.FromArgb(190, 140, 0);
			Color DSNameColor = Color.FromArgb(84, 130, 53);
			Color timeColor = Color.FromArgb(0, 112, 192);
			Color actionColor = Color.FromArgb(255, 0, 0);

			Action<List<KeyValuePair<int, object>>> kvpAction = null;
			Action<Tuple<IDatastructure, List<int>>> dsAction = null;
			int functionOffset = 0;

			////////////////////////////////// BUILD FUNCTION

			int setSizeIndex = 2;
			foreach (int setSize in dataSetSizes) // [10k, 100k, 1kk]
			{				
				IDatastructure[] dataStructures = { new List(), new AVLTree(), new IntervalHiep(), new HashTable(setSize) };

				foreach (int setnr in Enumerable.Range(1, repeatSetSize)) // [1..5]
				{
					List<KeyValuePair<int, object>> testData = seedTestData(setSize); // generate random set

					foreach (int inputOrder in Enumerable.Range(0, 3)) //0 = random, 1 = sorted, 2 = reverse sorted.
					{
						if (inputOrder == 1)
							testData.Sort((x, y) => x.Key.CompareTo(y.Key));
						else if (inputOrder == 2)
							testData.Reverse();

						int inputOrderOffset = (dataSetSizes.Count() + 1) * inputOrder;
						int DSIndex = 2;
						int DSoffset = (setnr - 1) * (dataStructures.Count() + 1);
						workbook.Cells[setSizeIndex + inputOrderOffset + functionOffset, 1 + DSoffset].Value = setSize;
						workbook.Cells[setSizeIndex + inputOrderOffset + functionOffset, 1 + DSoffset].Style.Font.Color.SetColor(setSizeColor);
						foreach (IDatastructure DS in dataStructures) // [List, Tree, Hiep, Table]
						{
							long ms = 0;

							for (int t = 0; t < repeatTestSize; t++) // [0..29]
							{
								IDatastructure ds;
								if (DS.GetType() == typeof(HashTable))
									ds = (IDatastructure)Activator.CreateInstance(DS.GetType(), setSize);
								else
									ds = (IDatastructure)Activator.CreateInstance(DS.GetType());
								kvpAction = new Action<List<KeyValuePair<int, object>>>(ds.Build);
								ms += measureTime(kvpAction, testData);
							}

							workbook.Cells[1 + inputOrderOffset + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
							workbook.Cells[1 + inputOrderOffset + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
							workbook.Cells[setSizeIndex + inputOrderOffset + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
							workbook.Cells[setSizeIndex + inputOrderOffset + functionOffset, DSIndex + DSoffset].Value = ms / repeatTestSize;
							Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, ms / repeatTestSize, kvpAction.Method.Name, setSize);
							DSIndex++;
						}
						workbook.Cells[1 + inputOrderOffset + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
						workbook.Cells[1 + inputOrderOffset + functionOffset, DSoffset + 1].Value = "Build" + setnr;
					}
				}
				setSizeIndex++;
			}

			////////////////////////////////// SEARCH FUNCTION

			functionOffset += (dataSetSizes.Count() + 1) * 3 + 1;
			setSizeIndex = 2;
			foreach (int setSize in dataSetSizes) // [10k, 100k, 1kk]
			{
				foreach (int setnr in Enumerable.Range(1, repeatSetSize)) // [1..5]
				{
					IDatastructure[] dataStructures = { new List(), new AVLTree(), new IntervalHiep(), new HashTable(setSize) };
					List<KeyValuePair<int, object>> testData = seedTestData(setSize); // generate random set
					List<int> searchKeys = testData.GetRange(0, testData.Count() / 2).Select(x => x.Key).ToList(); // get half of keys for searching

					int DSIndex = 2;
					int DSoffset = (setnr - 1) * (dataStructures.Count() + 1);
					workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Value = setSize;
					workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Style.Font.Color.SetColor(setSizeColor);
					foreach (IDatastructure DS in dataStructures) // [List, Tree, Hiep, Table]
					{
						long ms = 0;
						DS.Build(testData);
						for (int t = 0; t < repeatTestSize; t++) // [0..29]
						{							
							dsAction = new Action<Tuple<IDatastructure, List<int>>>(Search2);
							ms += measureTime(dsAction, new Tuple<IDatastructure, List<int>>(DS, searchKeys));
						}

						workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
						workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
						workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Value = ms / repeatTestSize;
						workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
						Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, ms / repeatTestSize, dsAction.Method.Name, setSize);
						DSIndex++;
					}
					workbook.Cells[1 + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
					workbook.Cells[1 + functionOffset, DSoffset + 1].Value = "Search" + setnr;
				}

				setSizeIndex++;
			}

			////////////////////////////////// INSERT FUNCTION

			// insert

			////////////////////////////////// DELETE FUNCTION

			// deletion

			////////////////////////////////// GETMIN FUNCTION

			// getMin

			////////////////////////////////// GETMAX FUNCTION

			// getMax

			////////////////////////////////// EXTRACTMIN FUNCTION

			// extractMin

			////////////////////////////////// EXTRACTMAX FUNCTION

			// extractMax

			////////////////////////////////// END OF TEST

			saveAsExcelSheet();
			Console.WriteLine("\nTest is klaar");
		}

		public static void saveAsExcelSheet()
		{
			bool saved = false;
			while(!saved)
			try
			{
				var outputFile = new FileStream("book.xlsx", FileMode.Create);
				package.SaveAs(outputFile);
				saved = true;
			}
			catch
			{
				Console.WriteLine("Please close the file so it can be overwritten, press any key to try again");
				Console.ReadLine();
			}
		}

		private static void timeoutCheck(object timeout)
		{
			timeout = true;
		}

		public static long measureTime<I,O>(Func<I,O> function, I input, out O output)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			output = function(input);
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

		public static long measureTime<I>(Action<I> function, I input)
		{
            Thread t = new Thread(new ParameterizedThreadStart(doWork<I>));
			Stopwatch sw = new Stopwatch();
			sw.Start();
            t.Start(Tuple.Create(function, input));
            t.Join(60000); // 1 minute timeout
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

        public static void doWork<I>(object tuple)
        {
            var FuncPut = (Tuple<Action<I>,I>)tuple;
            FuncPut.Item1(FuncPut.Item2);
        }

		private void Search(List<KeyValuePair<int, object>> testData)
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

		private static void Search2(Tuple<IDatastructure,List<int>> DSnKeys)
		{
			foreach (int key in DSnKeys.Item2)
				DSnKeys.Item1.Search(key);
		}

		private void Delete(List<int> deletionKeys)
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

		private static List<int> seedDeletionKeys(List<KeyValuePair<int, object>> testData)
        {
            KeyValuePair<int, object>[] list = new KeyValuePair<int, object>[nrOfDeletions];
            testData.CopyTo(0, list, 0, nrOfDeletions);
            return new List<int>(list.Select(x => x.Key));
        }

		private static List<KeyValuePair<int, object>> seedTestData(int size)
        {
            var HS = new HashSet<KeyValuePair<int, object>>();
            Random r = new Random();
            while (HS.Count < size)
            {
                int key = r.Next(0, upperKeyBound);
                string value = key.ToString();
                HS.Add(new KeyValuePair<int, object>(key, value));
            }
            return new List<KeyValuePair<int, object>>(HS);
        }



		public static bool saved { get; set; }
	}
}
