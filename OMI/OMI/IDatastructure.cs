using System.Collections.Generic;

namespace OMI
{
    interface IDatastructure
    {
        void Build(List<KeyValuePair<int, object>> values);

        bool TryAdd(KeyValuePair<int, object> kvp);
        bool TryDelete(int i);

        KeyValuePair<int, object> Search(int i);

        object GetMin();
        object GetMax();

        object ExtractMin();
        object ExtractMax();
    }
}
