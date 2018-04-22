using System;
using System.Threading;

namespace AiDollar.Infrastructure.Threading
{
    // based on http://www.adammil.net/blog/v111_Creating_High-Performance_Locks_and_Lock-free_Code_for_NET_.html
    public sealed class FastReaderWriterLock
    {
        private const int OwnedByWriter = unchecked((int)0x80000000), ReservedForWriter = 0x40000000;
        private const int WriterWaitingMask = 0x3FC00000, ReaderMask = 0x3FF800, ReaderWaitingMask = 0x7FF;
        private const int OneWriterWaiting = 1 << 22, OneReader = 1 << 11, OneReaderWaiting = 1;

        // the high bit is set if the lock is owned by a writer. the next bit is set if the lock is reserved for a writer. the next 8 bits are
        // the number of threads waiting to write. the next 11 bits are the number of threads reading. the low 11 bits are the number of
        // threads waiting to read. this lets us easily check if the lock is free for reading by comparing it to the read mask
        private int _lockState;
        private readonly FastSemaphore _readWait = new FastSemaphore();
        private readonly FastSemaphore _writeWait = new FastSemaphore();

        public struct ReadReleaser : IDisposable
        {
            private FastReaderWriterLock _lock;

            internal ReadReleaser(FastReaderWriterLock @lock)
            {
                _lock = @lock;
            }

            public void Dispose()
            {
                if (_lock != null)
                {
                    _lock.ExitRead();
                    _lock = null;
                }
            }
        }

        public struct WriteReleaser : IDisposable
        {
            private FastReaderWriterLock _lock;

            internal WriteReleaser(FastReaderWriterLock @lock)
            {
                _lock = @lock;
            }

            public void Dispose()
            {
                if (_lock != null)
                {
                    _lock.ExitWrite();
                    _lock = null;
                }
            }
        }

        public ReadReleaser EnterRead()
        {
            while (true)
            {
                int state = _lockState;
                if ((uint)state < ReaderMask) // if it's free or owned by a reader, and we're not currently at the reader limit...
                {
                    if (Interlocked.CompareExchange(ref _lockState, state + OneReader, state) == state) break; // try to join in
                }
                else if ((state & ReaderWaitingMask) == ReaderWaitingMask) // otherwise, we need to wait. if there aren't any wait slots left...
                {
                    Thread.Sleep(10); // massive contention. sleep while waiting for a slot
                }
                else if (Interlocked.CompareExchange(ref _lockState, state + OneReaderWaiting, state) == state) // if we could get a wait slot...
                {
                    _readWait.Wait(); // wait until we're awakened
                }
            }

            return new ReadReleaser(this);
        }

        public void ExitRead()
        {
            if ((_lockState & ReaderMask) == 0) throw new SynchronizationLockException();

            int state, newState;
            do
            {
                // if there are other readers, just subtract one reader. otherwise, if we're the only reader and there are no writers waiting, free
                // the lock. otherwise, we'll wake up one of the waiting writers, so subtract one and reserve it for the writer. we must be careful
                // to preserve the ReservedForWriter flag since it can be set by a writer before it decides to wait
                state = _lockState;
                newState = (state & ReaderMask) != OneReader ? state - OneReader :
                           (state & WriterWaitingMask) == 0 ? state & ReservedForWriter :
                           (state | ReservedForWriter) - (OneReader + OneWriterWaiting);
            } while (Interlocked.CompareExchange(ref _lockState, newState, state) != state);

            if ((state & ReaderMask) == OneReader) ReleaseWaitingThreads(state); // if we were the last reader, release waiting threads
        }

        private void ReleaseWaitingThreads(int state)
        {
            // if any writers were waiting, release one of them. otherwise, if any readers were waiting, release all of them
            if ((state & WriterWaitingMask) != 0) _writeWait.Release();
            else if ((state & ReaderWaitingMask) != 0) _readWait.Release((uint)(state & ReaderWaitingMask));
        }

        public WriteReleaser EnterWrite()
        {
            while (true)
            {
                int state = _lockState, ownership = state & (OwnedByWriter | ReservedForWriter | ReaderMask);
                if (ownership == 0 || ownership == ReservedForWriter) // if the lock is free or reserved for a writer...
                {
                    // try to take it for ourselves
                    if (Interlocked.CompareExchange(ref _lockState, state & ~ReservedForWriter | OwnedByWriter, state) == state)
                        return new WriteReleaser(this);
                }
                else if ((state & WriterWaitingMask) == WriterWaitingMask) // if we want to wait but there aren't any slots left...
                {
                    Thread.Sleep(10); // massive contention. sleep while we wait for a slot
                }
                else if (Interlocked.CompareExchange(ref _lockState, state + OneWriterWaiting, state) == state) // if we got a wait slot...
                {
                    _writeWait.Wait();
                }
            }
        }

        public void ExitWrite()
        {
            if ((_lockState & OwnedByWriter) == 0)
                throw new SynchronizationLockException();

            int state, newState;
            do
            {
                // if no writers are waiting, mark the lock as free. otherwise, subtract one waiter and reserve the lock for it
                state = _lockState;
                newState = (state & WriterWaitingMask) == 0 ? 0 : state + unchecked(ReservedForWriter - OwnedByWriter - OneWriterWaiting);
            } while (Interlocked.CompareExchange(ref _lockState, newState, state) != state);

            ReleaseWaitingThreads(state);
        }
    }
}
