#if NET35

using System.Collections.Generic;
using System.Threading;

namespace NetFX.Polyfill2
{
    public class BlockingCollection<T>
    {
        private object _queueLock = new();
        private Queue<T> _queue = new();
        private AutoResetEvent _objectAvailableEvent = new(false);

        public T Take()
        {
            //locks on queueLock, if queue is empty, wait for objectAvailableEvent to be set, then take the first item in the queue
            lock (_queueLock)
            {
                if (_queue.Count > 0)
                    return _queue.Dequeue();
            }

            _objectAvailableEvent.WaitOne();

            return Take();
        }

        public void Add(T obj)
        {
            //lock on queueLock, add the object to the queue, then set the objectAvailableEvent
            lock (_queueLock)
            {
                _queue.Enqueue(obj);
            }

            _objectAvailableEvent.Set();
        }
    }
}

#endif
