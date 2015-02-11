using System;
using System.Collections.Generic;

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

        public object GetMin()
        {
            if (hashList.Length == 0)
                return null;
            var currentMin = hashList[1].GetMin();
            for (int i = 1; i < hashList.Length; i++)
            {
                var thisMin = hashList[i].GetMin();
                if (thisMin.Key < currentMin.Key)
                    currentMin = thisMin;
            }
            return currentMin;
        }

        public object GetMax()
        {
            if (hashList.Length == 0)
                return null;
            var currentMax = hashList[1].GetMax();
            for (int i = 1; i < hashList.Length; i++)
            {
                var thisMax = hashList[i].GetMax();
                if (thisMax.Key < currentMax.Key)
                    currentMax = thisMax;
            }
            return currentMax;
        }

        public object ExtractMin()
        {
            throw new NotImplementedException();
        }

        public object ExtractMax()
        {
            throw new NotImplementedException();
        }
    }

    class HashList
    {
        private HashElement start;   
        public HashList()
        {
        }

        public void Insert(KeyValuePair<int, object> kvp)
        {
            var elem = new HashElement(kvp);
            if (start == null)
            {
                start = elem;
            }
            else
            {
                start.Previous = elem;
                elem.Next = start;
                start = elem;
            }
        }

        public void Delete(int i)
        {
            var s = start;
            while (s != null)
            {
                if (s.Key == i)
                {
                    s.Previous.Next = s.Next;
                    if (s.Next != null)
                        s.Next.Previous = s.Previous;
                    return;
                }
                s = s.Next;
            }
        }

        public object Find(int i)
        {
            var s = start;
            while (s != null)
            {
                if (s.Key == i)
                    return s.Value;
                s = s.Next;
            }
            return null;
        }

        public KeyValuePair<int, object> GetMin()
        {
            var s = start;
            int minValue = int.MaxValue;
            var minObject = start.Value;
            while (s != null)
            {
                if (s.Key < minValue)
                {
                    minValue = s.Key;
                    minObject = s.Value;
                }
                s = s.Next;
            }
            return new KeyValuePair<int, object>(minValue, minObject);
        }
        public KeyValuePair<int, object> GetMax()
        {
            var s = start;
            int maxValue = int.MinValue;
            var minObject = start.Value;
            while (s != null)
            {
                if (s.Key < maxValue)
                {
                    maxValue = s.Key;
                    minObject = s.Value;
                }
                s = s.Next;
            }
            return new KeyValuePair<int, object>(maxValue, minObject);
        } 
    }

    class HashElement
    {
        public object Value;
        public int Key;
        public HashElement Next;
        public HashElement Previous;

        public HashElement(KeyValuePair<int, object> val )
        {
            Value = val.Value;
            Key = val.Key;
        }
    }
}
