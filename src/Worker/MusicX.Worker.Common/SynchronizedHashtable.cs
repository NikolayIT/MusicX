namespace MusicX.Worker.Common
{
    using System.Collections.Concurrent;

    public class SynchronizedHashtable<T>
    {
        private readonly ConcurrentDictionary<T, bool> concurrentDictionary;

        public SynchronizedHashtable()
        {
            this.concurrentDictionary = new ConcurrentDictionary<T, bool>();
        }

        public bool Add(T value)
        {
            return this.concurrentDictionary.TryAdd(value, true);
        }

        public void Remove(T value)
        {
            this.concurrentDictionary.TryRemove(value, out _);
        }
    }
}
