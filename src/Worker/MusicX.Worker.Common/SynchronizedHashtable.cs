namespace MusicX.Worker.Common
{
    using System;
    using System.Collections;

    // TODO: Hashtable is deprecated. Use ConcurrentDictionary<TKey, TValue> instead.
    public class SynchronizedHashtable
    {
        private static readonly object Locker = new object();

        private readonly Hashtable hashtable;

        public SynchronizedHashtable()
        {
            var unsynchronizedHashtable = new Hashtable();
            this.hashtable = Hashtable.Synchronized(unsynchronizedHashtable);
        }

        public bool Add(object value)
        {
            lock (SynchronizedHashtable.Locker)
            {
                if (this.hashtable.ContainsKey(value))
                {
                    return false;
                }

                try
                {
                    this.hashtable.Add(value, true);
                    return true;
                }
                catch (ArgumentException)
                {
                    // The item is already in the hashtable.
                    return false;
                }
            }
        }

        public void Remove(object value)
        {
            lock (SynchronizedHashtable.Locker)
            {
                this.hashtable.Remove(value);
            }
        }
    }
}
