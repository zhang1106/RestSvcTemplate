using System;
using System.Threading;

namespace AiDollar.Infrastructure.Threading
{
    // based on http://www.adammil.net/blog/v111_Creating_High-Performance_Locks_and_Lock-free_Code_for_NET_.html
    public sealed class FastSemaphore
    {
        private uint _count;

        public void Release()
        {
            lock (this)
            {
                if (_count == uint.MaxValue)
                {
                    throw new InvalidOperationException();
                }

                _count++;
                Monitor.Pulse(this);
            }
        }

        public void Release(uint count)
        {
            if (count != 0)
            {
                lock (this)
                {
                    _count += count;
                    if (_count < count) // if it overflowed, undo the addition and throw an exception
                    {
                        _count -= count;
                        throw new InvalidOperationException();
                    }

                    if (count == 1)
                        Monitor.Pulse(this);
                    else
                        Monitor.PulseAll(this);
                }
            }
        }

        public void Wait()
        {
            lock (this)
            {
                while (_count == 0) Monitor.Wait(this);
                _count--;
            }
        }
    }
}
