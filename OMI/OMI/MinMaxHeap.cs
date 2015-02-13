using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OMI
{
    public class MinMaxHeap : IDatastructure
    {
        KeyValuePair<int, object>[] Heap;
        int curSize;
        int maxSize;
        int compCount;
        string CPUTime;

        public MinMaxHeap(int maxSize)
        {
            this.maxSize = maxSize;
            Heap = new KeyValuePair<int, object>[maxSize + 1];
        }
        public void Build(List<KeyValuePair<int, object>> values)
        {
            Heap = new KeyValuePair<int, object>[Math.Max(values.Count + 1, maxSize)];
            Heap[0] = new KeyValuePair<int, object>(int.MaxValue, null);
            curSize = values.Count;
            maxSize = int.MaxValue;
            compCount = 0;
            CPUTime = "";
            for (int i = 1; i <= values.Count; i++)
            {
                Heap[i] = values[i - 1];
            }
            buildMinMaxHeap();
        }
        public void buildMinMaxHeap()
        {
            Stopwatch myWatch = new Stopwatch();
            myWatch.Start();
            for (int i = curSize / 2; i >= 0; i--)
                percolateDown(i);
            myWatch.Stop();
            TimeSpan ts = myWatch.Elapsed;
            CPUTime = "Elapsed Time is: " + ts.Hours.ToString() + ts.Minutes.ToString() + ts.Seconds.ToString();
        }
        public void percolateDown(int pos)
        {
            if (isMaxLevel(pos))
                percolateDownMax(pos);
            else
                percolateDownMin(pos);
        }
        public void percolateDownMax(int pos)
        {
            int maxPos = GetMaxDescendant(pos);

            /* Check if we have a child */
            compCount++;
            if (maxPos > 0)
            {
                /* Check if position is a grandchild */
                compCount++;
                if (maxPos >= pos * 4)
                {
                    /* Swap if greater than grandparent */
                    compCount++;
                    if (Heap[maxPos].Key > Heap[pos].Key)
                    {
                        swap(pos, maxPos);

                        /* Swap if less than parent */
                        compCount++;
                        if (Heap[maxPos].Key < Heap[maxPos / 2].Key)
                            swap(maxPos, maxPos / 2);
                        percolateDownMax(maxPos);
                    }
                }
                /* Position is a child */
                else
                {
                    /* Swap if greater than parent */
                    compCount++;
                    if (Heap[maxPos].Key > Heap[pos].Key)
                        swap(pos, maxPos);
                }
            }
        }
        public void percolateDownMin(int pos)
        {
            int minPos = GetMinDescendant(pos);

            /* Check if we have a child */
            compCount++;
            if (minPos > 0)
            {
                /* Check if minimum is a grandchild */
                compCount++;
                if (minPos >= pos * 4)
                {
                    /* Swap if less than grandparent */
                    compCount++;
                    if (Heap[minPos].Key < Heap[pos].Key)
                    {
                        swap(pos, minPos);

                        /* Swap if greater than parent */
                        compCount++;
                        if (Heap[minPos].Key > Heap[minPos / 2].Key)
                            swap(minPos, minPos / 2);
                        percolateDownMin(minPos);
                    }
                }
                /* We don't have a grandchild */
                else
                {
                    /* Swap if less than parent */
                    compCount++;
                    if (Heap[minPos].Key < Heap[pos].Key)
                        swap(pos, minPos);
                }
            }
        }
        public void swap(int positionOne, int positionTwo)
        {
            KeyValuePair<int, object> tmp = Heap[positionOne];
            Heap[positionOne] = Heap[positionTwo];
            Heap[positionTwo] = tmp;
        }
        public bool isMaxLevel(int pos)
        {
            return (int)(Math.Log(pos + 1.0) / Math.Log(2.0)) % 2 == 1;
        }
        public int GetMaxGrandChild(int pos)
        {
            /* Initializing values */
            bool maxValInit = false;
            KeyValuePair<int, object> maxVal = new KeyValuePair<int, object>();
            int maxIndex = pos;
            int curIndex;

            /* Get a grandchild */
            curIndex = 2 * (2 * pos);

            /* Check for valid position */
            compCount++;
            if (curIndex <= curSize)
            {
                /* Check if maxVal isn't initialized or 
                /* current index is greater than maxVal
                /*/
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key > maxVal.Key))
                {
                    maxIndex = curIndex;
                    maxVal = Heap[maxIndex];
                    maxValInit = true;
                }
            }

            /* Get the other grandchild */
            curIndex = 2 * ((2 * pos) + 1);
            compCount++;
            if (curIndex <= curSize)
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key > maxVal.Key))
                {
                    maxIndex = curIndex;
                    maxVal = Heap[maxIndex];
                    maxValInit = true;
                }
            }

            curIndex = 2 * (2 * pos) + 1;
            compCount++;
            if (curIndex <= curSize)
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key > maxVal.Key))
                {
                    maxIndex = curIndex;
                    maxVal = Heap[maxIndex];
                    maxValInit = true;
                }
            }

            curIndex = (2 * ((2 * pos) + 1)) + 1;
            compCount++;
            if (curIndex <= curSize)
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key > maxVal.Key))
                {
                    maxIndex = curIndex;
                    maxVal = Heap[maxIndex];
                    maxValInit = true;
                }
            }
            return maxIndex;
        }
        public int GetMinGrandChild(int pos)
        {
            bool maxValInit = false;
            KeyValuePair<int, object> minVal = new KeyValuePair<int, object>();
            int minIdx = pos;
            int curIndex;

            curIndex = 2 * (2 * pos);
            compCount++;
            if (curIndex <= curSize)
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key < minVal.Key))
                {
                    minIdx = curIndex;
                    minVal = Heap[minIdx];
                    maxValInit = true;
                }
            }

            curIndex = 2 * ((2 * pos) + 1);
            compCount++;
            if (curIndex <= curSize)
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key < minVal.Key))
                {
                    minIdx = curIndex;
                    minVal = Heap[minIdx];
                    maxValInit = true;
                }
            }

            curIndex = 2 * (2 * pos) + 1;
            compCount++;
            if (curIndex <= curSize)
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key < minVal.Key))
                {
                    minIdx = curIndex;
                    minVal = Heap[minIdx];
                    maxValInit = true;
                }
            }

            curIndex = (2 * ((2 * pos) + 1)) + 1;
            compCount++;
            if (curIndex <= curSize)
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key < minVal.Key))
                {
                    minIdx = curIndex;
                    minVal = Heap[minIdx];
                    maxValInit = true;
                }
            }
            return minIdx;
        }
        public bool isLeafPos(int pos)
        {
            bool isLeaf = false;
            compCount++;
            if ((2 * pos > curSize) && ((2 * pos) + 1 > curSize))
                isLeaf = true;
            return isLeaf;
        }
        public int GetMaxDescendant(int pos)
        {
            bool maxValInit = false;
            KeyValuePair<int, object> maxVal = new KeyValuePair<int, object>();
            int maxIndex = pos;
            int curIndex;

            maxIndex = GetMaxGrandChild(pos);
            compCount++;
            if (maxIndex != pos)
            {
                maxVal = Heap[maxIndex];
                maxValInit = false;
            }

            curIndex = 2 * pos;
            compCount++;
            if ((curIndex <= curSize) && (isLeafPos(curIndex)))
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key > maxVal.Key))
                {
                    maxIndex = curIndex;
                    maxVal = Heap[curIndex];
                    maxValInit = true;
                }
            }

            curIndex = (2 * pos) + 1;
            compCount++;
            if ((curIndex <= curSize) && (isLeafPos(curIndex)))
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key > maxVal.Key))
                {
                    maxIndex = curIndex;
                    maxVal = Heap[curIndex];
                    maxValInit = true;
                }
            }
            return maxIndex;
        }
        public int GetMinDescendant(int pos)
        {
            bool maxValInit = false;
            KeyValuePair<int, object> minVal = new KeyValuePair<int, object>();
            int minIdx = pos;
            int curIndex;

            minIdx = GetMinGrandChild(pos);
            compCount++;
            if (minIdx != pos)
            {
                minVal = Heap[minIdx];
                maxValInit = true;
            }

            curIndex = 2 * pos;
            compCount++;
            if ((curIndex <= curSize) && (isLeafPos(curIndex)))
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key < minVal.Key))
                {
                    minIdx = curIndex;
                    minVal = Heap[curIndex];
                    maxValInit = true;
                }
            }

            curIndex = (2 * pos) + 1;
            compCount++;
            if ((curIndex <= curSize) && (isLeafPos(curIndex)))
            {
                compCount++;
                if ((!maxValInit) || (Heap[curIndex].Key < minVal.Key))
                {
                    minIdx = curIndex;
                    minVal = Heap[curIndex];
                    maxValInit = true;
                }
            }
            return minIdx;
        }
        public string getCPUCycles()
        {
            return CPUTime;
        }
        public int getNumOfComparisons()
        {
            return compCount;
        }
        public void getFirstTenElements()
        {
            int max = 10;
            string uitvoer = "";
            if (curSize < max)
                max = curSize;
            for (int i = 0; i < max; i++)
                uitvoer += ", " + Heap[i].Key;
            Console.WriteLine(uitvoer);
        }
        public void getLastTenElements()
        {
            int max = 10;
            string uitvoer = "";
            if (curSize < max)
                max = curSize;
            for (int i = curSize - max; i < curSize; i++)
                uitvoer += ", " + Heap[i].Key;
            Console.WriteLine(uitvoer);
        }
        public KeyValuePair<int, object> ExtractMin()
        {
            return Heap[0];
        }
        public KeyValuePair<int, object> ExtractMax()
        {

            //KeyValuePair<int, object>kvp = Heap[GetMaxGrandChild(0)];
            //if(TryDelete(Heap.IndexOf(kvp)))
            //{
            //    return kvp;
            //}
            //else 
            return new KeyValuePair<int, object>(-1, null);
        }
        public KeyValuePair<int, object> GetMax()
        {
            return Heap[(GetMaxGrandChild(0))];
        }
        public KeyValuePair<int, object> GetMin()
        {
            return Heap[GetMinGrandChild(0)];
        }
        public KeyValuePair<int, object> Search(int key)
        {
            for (int i = 1; i <= curSize; i++)
            {
                if (Heap[i].Key == key)
                {
                    return Heap[i];
                }
            }
            return new KeyValuePair<int, object>(-1, null);
        }
        public bool TryDelete(int position)
        {
            //Heap.RemoveAt(position);
            //buildMinMaxHeap();
            return true;
        }
        public bool TryAdd(KeyValuePair<int, object> invoer)
        {
            //Heap.Add(invoer);
            //buildMinMaxHeap();
            return true;
        }
    }
}