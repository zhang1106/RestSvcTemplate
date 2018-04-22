using System;
using System.Threading;

namespace AiDollar.Infrastructure.Threading
{

    public interface IBackgroundWorker : IDisposable
    {
        bool IsActive { get; }
        void Start(Action<CancellationToken> action);
        void Stop();
    }

    public interface IBackgroundWorker<TState> : IDisposable
    {
        bool IsActive { get; }
        TState State { get; set; }
        void Start(Action<TState, CancellationToken> action);
        void Stop();
    }
}
