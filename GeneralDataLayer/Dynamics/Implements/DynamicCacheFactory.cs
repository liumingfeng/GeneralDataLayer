using System.Collections.Concurrent;

namespace GeneralDataLayer.Dynamics.Implements
{
    internal class DynamicCacheFactory<T>
    {
        public static ConcurrentDictionary<int, ConcurrentDictionary<int, T>> DictInstance { get; } = new ConcurrentDictionary<int, ConcurrentDictionary<int, T>>();
    }
}