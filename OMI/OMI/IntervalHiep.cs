using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C5;

namespace OMI
{
    class IntervalHiep:IDatastructure
    {
        private IntervalHeap<int> heap;
        public IntervalHiep()
        {
            heap = new IntervalHeap<int>();
        } 
        public void Build(List<System.Collections.Generic.KeyValuePair<int, object>> values)
        {
            heap.AddAll(values.Select(x=> x.Key));
        }

        public bool TryAdd(System.Collections.Generic.KeyValuePair<int, object> kvp)
        {
            heap.Add(kvp.Key);
            return true;
        }

        public bool TryDelete(int i)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.KeyValuePair<int, object> Search(int i)
        {
            int value = -1;
            heap.Find(x => x == i, out value);
            return new System.Collections.Generic.KeyValuePair<int, object>(value, value);
        }

        public System.Collections.Generic.KeyValuePair<int, object> GetMin()
        {
            int i = heap.FindMin();
            return new System.Collections.Generic.KeyValuePair<int, object>(i, i);
        }

        public System.Collections.Generic.KeyValuePair<int, object> GetMax()
        {
            int i = heap.FindMax();
            return new System.Collections.Generic.KeyValuePair<int, object>(i, i);
        }

        public System.Collections.Generic.KeyValuePair<int, object> ExtractMin()
        {
            int i = heap.DeleteMin();
            return new System.Collections.Generic.KeyValuePair<int, object>(i, i);

        }
        public System.Collections.Generic.KeyValuePair<int, object> ExtractMax()
        {
            int i = heap.DeleteMax();
            return new System.Collections.Generic.KeyValuePair<int, object>(i, i);
        }
    }
}
