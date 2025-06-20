using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neuopc
{
    public class Queue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new();

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }

        public bool TryDequeue(out T item)
        {
            return _queue.TryDequeue(out item);
        }

    }
}
