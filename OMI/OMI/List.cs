using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI
{
    public class List : IDatastructure
    {
        public List<KeyValuePair<int, object>> items = new List<KeyValuePair<int, object>>();

        public void Build(List<KeyValuePair<int, object>> values)
        {
            if (values.Count > 0)
            {
                var temp = new KeyValuePair<int, object>[values.Count()];
                values.CopyTo(temp, 0);
                items = values.ToList();
                QuickSort();                
            }
        }

        public bool TryAdd(KeyValuePair<int, object> kvp)
        {
            int i = 0;
            while (true)
            {
                if (i == items.Count)
                {
                    items.Add(kvp);
                    return true;
                }
                if (i == 0 && items[i].Key >= kvp.Key)
                {
                    items.Insert(0, kvp);
                    return true;
                }
                if (i != 0 && items[i - 1].Key < kvp.Key && items[i].Key >= kvp.Key)
                {
                    items.Insert(i, kvp);
                    return true;
                }
                i++;
            }
        }

        public bool TryDelete(int i)
        {
            int searchResult = BinarySearch(i, 0, items.Count - 1);
            if (searchResult > -1)
            {
                items.RemoveAt(searchResult);
            }
            return true;
        }

        public KeyValuePair<int, object> Search(int i)
        {
            int searchResult = BinarySearch(i, 0, items.Count - 1);
            if (searchResult > -1)
            {
                return items[BinarySearch(i, 0, items.Count - 1)];
            }
            return new KeyValuePair<int, object>(-1, null);
        }

        public KeyValuePair<int, object> GetMin()
        {
            if (items.Count > 0)
            {
                return items[0];
            }
            return new KeyValuePair<int, object>(-1, null);
        }

        public KeyValuePair<int, object> GetMax()
        {
            if (items.Count > 0)
            {
                return items[items.Count - 1];
            }
            return new KeyValuePair<int, object>(-1, null);
        }

        public KeyValuePair<int, object> ExtractMin()
        {
            if (items.Count > 0)
            {
                KeyValuePair<int, object> item = items[0];
                TryDelete(item.Key);
                return item;
            }
            return new KeyValuePair<int, object>(-1, null); 
        }

        public KeyValuePair<int, object> ExtractMax()
        {
            if (items.Count > 0)
            {
                KeyValuePair<int, object> item = items[items.Count - 1];
                TryDelete(item.Key);
                return item;
            }
            return new KeyValuePair<int, object>(-1, null);
        }

        private int BinarySearch(int key, int min, int max)
        {
            if (max < min)
                return -1;
            else
            {
                int middle = (min + max) / 2;

                if (items[middle].Key > key)
                    return BinarySearch(key, min, middle - 1);
                else if (items[middle].Key < key)
                    return BinarySearch(key, middle + 1, max);
                else
                    return middle;
            }
        }

        private void QuickSort()
        {
            DoSort(0, items.Count - 1);
        }

        private void DoSort(int left, int right)
        {
            int leftValue = left;
            int rightValue = right;
            KeyValuePair<int, object> middle = items[(left + right) / 2];

            while (leftValue <= rightValue)
            {
                while (items[leftValue].Key < middle.Key)
                {
                    leftValue++;
                }
                while (middle.Key < items[rightValue].Key)
                {
                    rightValue--;
                }

                if (leftValue <= rightValue)
                {
                    KeyValuePair<int, object> leftPart = items[leftValue];
                    items[leftValue] = items[rightValue];
                    items[rightValue] = leftPart;
                    leftValue++;
                    rightValue--;
                }
            }

            if (left < rightValue)
            {
                DoSort(left, rightValue);
            }
            if (leftValue < right)
            {
                DoSort(leftValue, right);
            }
        }
    }
}
