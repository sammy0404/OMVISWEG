using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace OMI
{
    class HashTable: IDatastructure
    {
        private HashList[] hashList;
        public HashTable(int length)
        {
            hashList = new HashList[length];
            for (int i = 0; i < hashList.Length; i++)
            {
                hashList[i] = new HashList();
            }
        }
        public void Build(List<KeyValuePair<int, object>> values)
        {
            foreach (var v in values)
            {
                int index = v.Key%hashList.Length;
                hashList[index].Insert(v);
                //Console.WriteLine("inserted: "+ v.Key);
            }
        }

        public bool TryAdd(KeyValuePair<int, object> v)
        {
            int index = v.Key % hashList.Length;
            hashList[index].Insert(v);
            return true;
        }

        public bool TryDelete(int i)
        {
            int index = i % hashList.Length;
            hashList[index].Delete(i);
            return true;
        }

        public KeyValuePair<int, object> Search(int i)
        {
            int index = i%hashList.Length;
            return new KeyValuePair<int, object>(i, hashList[index].Find(i));
        }

        public KeyValuePair<int, object> GetMin()
        {
            if (hashList.Length == 0)
                return new KeyValuePair<int, object>(-1,null);
            var currentMin = hashList[0].GetMin();
            for (int i = 1; i < hashList.Length; i++)
            {
                var thisMin = hashList[i].GetMin();
                if (thisMin == null)
                    continue;
                if (currentMin == null || thisMin.Key < currentMin.Key)
                    currentMin = thisMin;
            }
            return new KeyValuePair<int, object>(currentMin.Key, currentMin.Value);
        }

        public KeyValuePair<int, object> GetMax()
        {
            if (hashList.Length == 0)
                return new KeyValuePair<int, object>(-1,null);
            var currentMax = hashList[0].GetMax();
            for (int i = 1; i < hashList.Length; i++)
            {
                var thisMax = hashList[i].GetMax();
                if (thisMax != null)
                    if (currentMax == null || thisMax.Key > currentMax.Key)
                        currentMax = thisMax;
            }
            return new KeyValuePair<int, object>(currentMax.Key, currentMax.Value);
        }

        public KeyValuePair<int, object> ExtractMin()
        {
            if (hashList.Length == 0)
                return new KeyValuePair<int, object>(-1, null);
            var currentMin = hashList[1].GetMin();
            for (int i = 1; i < hashList.Length; i++)
            {
                var thisMin = hashList[i].GetMin();
                if (thisMin == null)
                    continue;
                if (currentMin == null || thisMin.Key < currentMin.Key)
                    currentMin = thisMin;
            }
            currentMin.Remove();
            return new KeyValuePair<int, object>(currentMin.Key, currentMin.Value);
        }

        public KeyValuePair<int, object> ExtractMax()
        {
            if (hashList.Length == 0)
                return new KeyValuePair<int, object>(-1, null);
            var currentMax = hashList[1].GetMax();
            for (int i = 1; i < hashList.Length; i++)
            {
                var thisMax = hashList[i].GetMax();
                if (thisMax != null)
                    if (currentMax == null || thisMax.Key > currentMax.Key)
                        currentMax = thisMax;
            }

            currentMax.Remove();
            return  new KeyValuePair<int, object>(currentMax.Key, currentMax.Value);
        }
    }

    class HashList
    {
        public HashElement Start;   

        public void Insert(KeyValuePair<int, object> kvp)
        {
            var elem = new HashElement(kvp, this);
            if (Start == null)
            {
                Start = elem;
            }
            else
            {
                Start.Previous = elem;
                elem.Next = Start;
                Start = elem;
            }
        }

        public void Delete(int i)
        {
            var s = Start;
            while (s != null)
            {
                if (s.Key == i)
                {
                    s.Remove();
                    return;
                }
                s = s.Next;
            }
        }

        public object Find(int i)
        {
            var s = Start;
            while (s != null)
            {
                if (s.Key == i)
                    return s.Value;
                s = s.Next;
            }
            return null;
        }

        public HashElement GetMin()
        {
            var s = Start;
            int minValue = int.MaxValue;
            HashElement minKvP = null;
            while (s != null)
            {
                if (s.Key < minValue)
                {
                    minValue = s.Key;
                    minKvP = s;
                }
                s = s.Next;
            }
            return minKvP;
        }
        public HashElement GetMax()
        {
            var s = Start;
            int maxValue = int.MinValue;
            HashElement maxKvp = null;
            while (s != null)
            {
                if (s.Key > maxValue)
                {
                    maxValue = s.Key;
                    maxKvp = s;
                }
                s = s.Next;
            }
            return maxKvp;
        } 
    }

    class HashElement
    {
        public object Value;
        public int Key;
        public HashElement Next;
        public HashElement Previous;
        public HashList Parent;
        public HashElement(KeyValuePair<int, object> val, HashList parent )
        {
            Value = val.Value;
            Key = val.Key;
            Parent = parent;
        }

        public void Remove()
        {
            if (Parent.Start == this)
            {
                Parent.Start = this.Next;
            }

            if (Previous != null)
                Previous.Next = Next;
            if (Next != null)
                Next.Previous = Previous;
        }
    }
}
