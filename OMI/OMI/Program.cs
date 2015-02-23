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
        const int MINUTE = 60000;

        IDatastructure DS = null;
		static ExcelPackage package;
		static ExcelWorksheet workbook;

        static int nrOfSearches  = 100000;
        static int nrOfKeys      = 1000000;
        static int upperKeyBound = 5000000;
        static int nrOfDeletions = 10000;
        static int nrOfMinMax    = 10000;

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
                testData = seedTestData(nrOfKeys);
                Console.WriteLine("Press T,L,H or I to test DS or X to terminate.");
                try
                {
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
                }
                catch
                {
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
                p.SearchMain(testData);
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

			int repeatTestSize = 30; // 30
			int repeatSetSize = 5;  // 5

			Color setSizeColor = Color.FromArgb(190, 140, 0);
			Color DSNameColor = Color.FromArgb(84, 130, 53);
			Color timeColor = Color.FromArgb(0, 112, 192);
			Color actionColor = Color.FromArgb(255, 0, 0);

			Action<List<KeyValuePair<int, object>>> kvpAction = null;
			Action<Tuple<IDatastructure, List<int>>> dsAction = null;
            Action<Tuple<IDatastructure, List<KeyValuePair<int, object>>>> insertAction = null;
            Action<IDatastructure> DSAction = null;
            Action<Tuple<IDatastructure, int>> extractAction = null;

            List<long> runTimes = new List<long>();
            DateTime dt = DateTime.Now;
            long avgRunTime = 0;
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
                            runTimes.Clear();
							for (int t = 0; t < repeatTestSize; t++) // [0..29]
							{
								IDatastructure ds;
								if (DS.GetType() == typeof(HashTable))
									ds = (IDatastructure)Activator.CreateInstance(DS.GetType(), setSize);
								else
									ds = (IDatastructure)Activator.CreateInstance(DS.GetType());

                                kvpAction = new Action<List<KeyValuePair<int, object>>>(ds.Build);
								runTimes.Add(measureTime(kvpAction, testData));
							}

                            avgRunTime = getAvg(runTimes);
							workbook.Cells[1 + inputOrderOffset + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
							workbook.Cells[1 + inputOrderOffset + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
							workbook.Cells[setSizeIndex + inputOrderOffset + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                            workbook.Cells[setSizeIndex + inputOrderOffset + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
                            Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, kvpAction.Method.Name, setSize);
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
            dsAction = new Action<Tuple<IDatastructure, List<int>>>(Search);

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
                        runTimes.Clear();
						DS.Build(testData);                        
						for (int t = 0; t < repeatTestSize; t++) // [0..29]
							runTimes.Add(measureTime(dsAction, new Tuple<IDatastructure, List<int>>(DS, searchKeys)));

                        avgRunTime = getAvg(runTimes);
						workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
						workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
						workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                        Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, dsAction.Method.Name, setSize);
						DSIndex++;
					}
					workbook.Cells[1 + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
					workbook.Cells[1 + functionOffset, DSoffset + 1].Value = "Search" + setnr;
				}

				setSizeIndex++;
			}

            ////////////////////////////////// SEARCH MIN

            functionOffset += (dataSetSizes.Count() + 1) * 1 + 1;
            setSizeIndex = 2;
            extractAction = new Action<Tuple<IDatastructure, int>>(SearchMin);

            foreach (int setSize in dataSetSizes) // [10k, 100k, 1kk]
            {
                foreach (int setnr in Enumerable.Range(1, repeatSetSize)) // [1..5]
                {
                    IDatastructure[] dataStructures = { new List(), new AVLTree(), new IntervalHiep(), new HashTable(setSize) };
                    List<KeyValuePair<int, object>> testData = seedTestData(setSize); // generate random set
                    var min = testData.Min(x => x.Key);

                    int DSIndex = 2;
                    int DSoffset = (setnr - 1) * (dataStructures.Count() + 1);
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Value = setSize;
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Style.Font.Color.SetColor(setSizeColor);
                    foreach (IDatastructure DS in dataStructures) // [List, Tree, Hiep, Table]
                    {
                        runTimes.Clear();
                        DS.Build(testData);
                        for (int t = 0; t < repeatTestSize; t++) // [0..29]
                            runTimes.Add(measureTime(extractAction, new Tuple<IDatastructure, int>(DS, min)));

                        avgRunTime = getAvg(runTimes); 
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                        Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, extractAction.Method.Name, setSize);
                        DSIndex++;
                    }
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Value = "SearchMin" + setnr;
                }

                setSizeIndex++;
            }

            ////////////////////////////////// SEARCH MAX

            functionOffset += (dataSetSizes.Count() + 1) * 1 + 1;
            setSizeIndex = 2;
            extractAction = new Action<Tuple<IDatastructure, int>>(SearchMax);

            foreach (int setSize in dataSetSizes) // [10k, 100k, 1kk]
            {
                foreach (int setnr in Enumerable.Range(1, repeatSetSize)) // [1..5]
                {
                    IDatastructure[] dataStructures = { new List(), new AVLTree(), new IntervalHiep(), new HashTable(setSize) };
                    List<KeyValuePair<int, object>> testData = seedTestData(setSize); // generate random set
                    var max = testData.Max(x => x.Key);

                    int DSIndex = 2;
                    int DSoffset = (setnr - 1) * (dataStructures.Count() + 1);
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Value = setSize;
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Style.Font.Color.SetColor(setSizeColor);
                    foreach (IDatastructure DS in dataStructures) // [List, Tree, Hiep, Table]
                    {
                        runTimes.Clear();
                        DS.Build(testData);
                        for (int t = 0; t < repeatTestSize; t++) // [0..29]
                           runTimes.Add(measureTime(extractAction, new Tuple<IDatastructure, int>(DS, max)));
                        
                        avgRunTime = getAvg(runTimes); 
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                        Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, extractAction.Method.Name, setSize);
                        DSIndex++;
                    }
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Value = "SearchMax" + setnr;
                }

                setSizeIndex++;
            }

			////////////////////////////////// INSERT FUNCTION

            functionOffset += (dataSetSizes.Count() + 1) * 1 + 1;
            setSizeIndex = 2;
            insertAction = new Action<Tuple<IDatastructure, List<KeyValuePair<int, object>>>>(Insert);

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
                            runTimes.Clear();                          
                            for (int t = 0; t < repeatTestSize; t++) // [0..29]
                            {
                                IDatastructure ds;
                                if (DS.GetType() == typeof(HashTable))
                                    ds = (IDatastructure)Activator.CreateInstance(DS.GetType(), setSize);
                                else
                                    ds = (IDatastructure)Activator.CreateInstance(DS.GetType());
                                
                                runTimes.Add(measureTime(insertAction, new Tuple<IDatastructure,List<KeyValuePair<int,object>>>(ds,testData)));
                            }

                            avgRunTime = getAvg(runTimes); 
                            workbook.Cells[1 + inputOrderOffset + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
                            workbook.Cells[1 + inputOrderOffset + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
                            workbook.Cells[setSizeIndex + inputOrderOffset + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                            workbook.Cells[setSizeIndex + inputOrderOffset + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
                            Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, insertAction.Method.Name, setSize);
                            DSIndex++;
                        }
                        workbook.Cells[1 + inputOrderOffset + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
                        workbook.Cells[1 + inputOrderOffset + functionOffset, DSoffset + 1].Value = "Insert" + setnr;
                    }
                }
                setSizeIndex++;
            }

			////////////////////////////////// DELETE FUNCTION

            functionOffset += (dataSetSizes.Count() + 1) * 3 + 1;
            setSizeIndex = 2;
            dsAction = new Action<Tuple<IDatastructure, List<int>>>(Delete);

            foreach (int setSize in dataSetSizes) // [10k, 100k, 1kk]
            {
                foreach (int setnr in Enumerable.Range(1, repeatSetSize)) // [1..5]
                {
                    IDatastructure[] dataStructures = { new List(), new AVLTree(), new IntervalHiep(), new HashTable(setSize) };
                    List<KeyValuePair<int, object>> testData = seedTestData(setSize); // generate random set
                    List<int> deleteKeys = testData.GetRange(0, testData.Count() / 2).Select(x => x.Key).ToList(); // get half of keys for deleting

                    int DSIndex = 2;
                    int DSoffset = (setnr - 1) * (dataStructures.Count() + 1);
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Value = setSize;
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Style.Font.Color.SetColor(setSizeColor);
                    foreach (IDatastructure DS in dataStructures) // [List, Tree, Hiep, Table]
                    {
                        runTimes.Clear();                       
                        for (int t = 0; t < repeatTestSize; t++) // [0..29]
                        {
                            IDatastructure ds;
                            if (DS.GetType() == typeof(HashTable))
                                ds = (IDatastructure)Activator.CreateInstance(DS.GetType(), setSize);
                            else
                                ds = (IDatastructure)Activator.CreateInstance(DS.GetType());

                            ds.Build(testData);                            
                            runTimes.Add(measureTime(dsAction, new Tuple<IDatastructure, List<int>>(ds, deleteKeys)));
                        }

                        avgRunTime = getAvg(runTimes); 
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                        Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, dsAction.Method.Name, setSize);
                        DSIndex++;
                    }
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Value = "Delete" + setnr;
                }

                setSizeIndex++;
            }

			////////////////////////////////// GETMIN FUNCTION

            functionOffset += (dataSetSizes.Count() + 1) * 1 + 1;
            setSizeIndex = 2;
            DSAction = new Action<IDatastructure>(GetMin);

            foreach (int setSize in dataSetSizes) // [10k, 100k, 1kk]
            {
                foreach (int setnr in Enumerable.Range(1, repeatSetSize)) // [1..5]
                {
                    IDatastructure[] dataStructures = { new List(), new AVLTree(), new IntervalHiep(), new HashTable(setSize) };
                    List<KeyValuePair<int, object>> testData = seedTestData(setSize); // generate random set                   

                    int DSIndex = 2;
                    int DSoffset = (setnr - 1) * (dataStructures.Count() + 1);
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Value = setSize;
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Style.Font.Color.SetColor(setSizeColor);
                    foreach (IDatastructure DS in dataStructures) // [List, Tree, Hiep, Table]
                    {
                        runTimes.Clear();                     
                        DS.Build(testData);
                        for (int t = 0; t < repeatTestSize; t++) // [0..29]
                            runTimes.Add(measureTime(DSAction, DS));

                        avgRunTime = getAvg(runTimes); 
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                        Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, DSAction.Method.Name, setSize);
                        DSIndex++;
                    }
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Value = "GetMin" + setnr;
                }

                setSizeIndex++;
            }

			////////////////////////////////// GETMAX FUNCTION

            functionOffset += (dataSetSizes.Count() + 1) * 1 + 1;
            setSizeIndex = 2;
            DSAction = new Action<IDatastructure>(GetMax);

            foreach (int setSize in dataSetSizes) // [10k, 100k, 1kk]
            {
                foreach (int setnr in Enumerable.Range(1, repeatSetSize)) // [1..5]
                {
                    IDatastructure[] dataStructures = { new List(), new AVLTree(), new IntervalHiep(), new HashTable(setSize) };
                    List<KeyValuePair<int, object>> testData = seedTestData(setSize); // generate random set                   

                    int DSIndex = 2;
                    int DSoffset = (setnr - 1) * (dataStructures.Count() + 1);
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Value = setSize;
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Style.Font.Color.SetColor(setSizeColor);
                    foreach (IDatastructure DS in dataStructures) // [List, Tree, Hiep, Table]
                    {
                        runTimes.Clear();     
                        DS.Build(testData);
                        for (int t = 0; t < repeatTestSize; t++) // [0..29]
                            runTimes.Add(measureTime(DSAction, DS));

                        avgRunTime = getAvg(runTimes); 
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                        Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, DSAction.Method.Name, setSize);
                        DSIndex++;
                    }
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Value = "GetMax" + setnr;
                }
                setSizeIndex++;
            }

			////////////////////////////////// EXTRACTMIN FUNCTION

            functionOffset += (dataSetSizes.Count() + 1) * 1 + 1;
            setSizeIndex = 2;
            extractAction = new Action<Tuple<IDatastructure, int>>(ExtractMin);

            foreach (int setSize in dataSetSizes) // [10k, 100k, 1kk]
            {
                foreach (int setnr in Enumerable.Range(1, repeatSetSize)) // [1..5]
                {
                    IDatastructure[] dataStructures = { new List(), new AVLTree(), new IntervalHiep(), new HashTable(setSize) };
                    List<KeyValuePair<int, object>> testData = seedTestData(setSize); // generate random set
                    int extractAmount = testData.Count() / 2;

                    int DSIndex = 2;
                    int DSoffset = (setnr - 1) * (dataStructures.Count() + 1);
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Value = setSize;
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Style.Font.Color.SetColor(setSizeColor);
                    foreach (IDatastructure DS in dataStructures) // [List, Tree, Hiep, Table]
                    {
                        runTimes.Clear();            
                        for (int t = 0; t < repeatTestSize; t++) // [0..29]
                        {                           
                            IDatastructure ds;
                            if (DS.GetType() == typeof(HashTable))
                                ds = (IDatastructure)Activator.CreateInstance(DS.GetType(), setSize);
                            else
                                ds = (IDatastructure)Activator.CreateInstance(DS.GetType());

                            ds.Build(testData);                            
                           runTimes.Add(measureTime(extractAction, new Tuple<IDatastructure, int>(ds, extractAmount)));
                        }

                        avgRunTime = getAvg(runTimes); 
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                        Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, extractAction.Method.Name, setSize);
                        DSIndex++;
                    }
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Value = "ExtractMin" + setnr;
                }

                setSizeIndex++;
            }

			////////////////////////////////// EXTRACTMAX FUNCTION

            functionOffset += (dataSetSizes.Count() + 1) * 1 + 1;
            setSizeIndex = 2;
            extractAction = new Action<Tuple<IDatastructure, int>>(ExtractMax);

            foreach (int setSize in dataSetSizes) // [10k, 100k, 1kk]
            {
                foreach (int setnr in Enumerable.Range(1, repeatSetSize)) // [1..5]
                {
                    IDatastructure[] dataStructures = { new List(), new AVLTree(), new IntervalHiep(), new HashTable(setSize) };
                    List<KeyValuePair<int, object>> testData = seedTestData(setSize); // generate random set
                    int extractAmount = testData.Count() / 2;

                    int DSIndex = 2;
                    int DSoffset = (setnr - 1) * (dataStructures.Count() + 1);
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Value = setSize;
                    workbook.Cells[setSizeIndex + functionOffset, 1 + DSoffset].Style.Font.Color.SetColor(setSizeColor);
                    foreach (IDatastructure DS in dataStructures) // [List, Tree, Hiep, Table]
                    {
                        runTimes.Clear();
                        for (int t = 0; t < repeatTestSize; t++) // [0..29]
                        {                           
                            IDatastructure ds;
                            if (DS.GetType() == typeof(HashTable))
                                ds = (IDatastructure)Activator.CreateInstance(DS.GetType(), setSize);
                            else
                                ds = (IDatastructure)Activator.CreateInstance(DS.GetType());

                            ds.Build(testData);          
                            runTimes.Add(measureTime(extractAction, new Tuple<IDatastructure, int>(ds, extractAmount)));
                        }

                        avgRunTime = getAvg(runTimes); 
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Value = DS.GetType().Name;
                        workbook.Cells[1 + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(DSNameColor);
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Value = avgRunTime;
                        workbook.Cells[setSizeIndex + functionOffset, DSIndex + DSoffset].Style.Font.Color.SetColor(timeColor);
                        Console.WriteLine("{0} \t\t {1} ms \t {2} \t {3}", DS.GetType().Name, avgRunTime, extractAction.Method.Name, setSize);
                        DSIndex++;
                    }
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Style.Font.Color.SetColor(actionColor);
                    workbook.Cells[1 + functionOffset, DSoffset + 1].Value = "ExtractMax" + setnr;
                }

                setSizeIndex++;
            }

			////////////////////////////////// END OF TEST

			saveAsExcelSheet();
            var duration = DateTime.Now.Subtract(dt);
			Console.WriteLine("\nTest is klaar en heeft {0} dagen en {1} (h:m:s:ms) geduurd",duration.Days, duration.ToString("hh\\:mm\\:ss\\:fff"));
		}

        private static long getAvg(List<long> runTimes)
        {
            int[] lol = { 1, 3, 3 };
            lol.ToList();
            if (runTimes.Count() <= 10)
                return runTimes.Sum() / runTimes.Count();
            else
            {
                runTimes.Sort();
                return runTimes.GetRange(5, runTimes.Count() - 10).Sum() / (runTimes.Count() - 10);
            }
        }

		public static void saveAsExcelSheet()
		{
			bool saved = false;
			while(!saved)
			try
			{
				var outputFile = new FileStream("statistics.xlsx", FileMode.Create);
				package.SaveAs(outputFile);
				saved = true;
			}
			catch
			{
				Console.WriteLine("Please close the file so it can be overwritten, press any key to try again");
				Console.ReadLine();
			}
		}

		public static long measureTime<I>(Action<I> function, I input)
		{
            Thread t = new Thread(new ParameterizedThreadStart(doWork<I>));
			Stopwatch sw = new Stopwatch();
			sw.Start();
            t.Start(Tuple.Create(function, input));
            if (!t.Join(MINUTE * 30))
                t.Abort();
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}

        public static void doWork<I>(object tuple)
        {
            var FuncPut = (Tuple<Action<I>,I>)tuple;
            FuncPut.Item1(FuncPut.Item2);
        }

        private static void SearchMin(Tuple<IDatastructure,int> dsMin)
        {
            foreach (int x in Enumerable.Range(0, nrOfMinMax))
                dsMin.Item1.Search(dsMin.Item2);
        }

        private static void SearchMax(Tuple<IDatastructure,int> dsMax)
        {
            foreach (int x in Enumerable.Range(0, nrOfMinMax))
                dsMax.Item1.Search(dsMax.Item2);
        }

        private static void ExtractMin(Tuple<IDatastructure, int> tuple)
        {
            for (int t = 0; t < tuple.Item2; t++)
                tuple.Item1.ExtractMin();
        }

        private static void ExtractMax(Tuple<IDatastructure, int> tuple)
        {
            for (int t = 0; t < tuple.Item2; t++)
                tuple.Item1.ExtractMax();
        }

        private static void GetMin(IDatastructure ds)
        {
            for (int i = 0; i < nrOfMinMax; i++)
                ds.GetMin();
        }

        private static void GetMax(IDatastructure ds)
        {
            for (int i = 0; i < nrOfMinMax; i++)
                ds.GetMax();
        }

        private static void Insert(Tuple<IDatastructure, List<KeyValuePair<int, object>>> list)
        {
            foreach (KeyValuePair<int, object> kvp in list.Item2)
                list.Item1.TryAdd(kvp);
        }

        //Used in Main
		private void SearchMain(List<KeyValuePair<int, object>> testData)
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

        private static void Delete(Tuple<IDatastructure, List<int>> DSnKeys)
        {
            foreach (int key in DSnKeys.Item2)
                DSnKeys.Item1.TryDelete(key);
        }

        //Used in Test
		private static void Search(Tuple<IDatastructure,List<int>> DSnKeys)
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
