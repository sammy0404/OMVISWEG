using System.Collections.Generic;

namespace OMI
{
    interface IDatastructure
    {
        void Build(List<KeyValuePair<int, object>> values);

        bool TryAdd(KeyValuePair<int, object> kvp);
        bool TryDelete(int i);

        KeyValuePair<int, object> Search(int i);

        KeyValuePair<int, object> GetMin();
        KeyValuePair<int, object> GetMax();

        KeyValuePair<int, object> ExtractMin();
        KeyValuePair<int, object> ExtractMax();
    }
}
