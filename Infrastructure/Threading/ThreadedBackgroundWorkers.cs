using System;
using System.Threading;
using AiDollar.Infrastructure.Logger;

namespace AiDollar.Infrastructure.Threading
{
    public abstract class ThreadedBackgroundWorkerBase
    {
        private readonly Thread _thread;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _isDisposed;

        protected ThreadedBackgroundWorkerBase(string name, ILogger logger)
        {
            Logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
            _thread = new Thread(DoWork)
            {
                Name = name,
                IsBackground = true
            };
        }

        public bool IsActive => _thread.IsAlive;

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            Logger?.LogDebug($"Stopping background worker '{_thread.Name}'");
            _cancellationTokenSource.Cancel();
            if (_thread.IsAlive)
            {
                Logger?.LogDebug($"Waiting for thread to exit '{_thread.Name}'");
                _thread.Join();
            }
            _cancellationTokenSource.Dispose();
            _isDisposed = true;
        }

        protected void Start(object state)
        {
            Logger?.LogDebug($"Starting background worker '{_thread.Name}'");
            _thread.Start(state);
        }

        protected ILogger Logger { get; }
        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;
        protected abstract void DoWork(object state);
    }

    public class ThreadedBackgroundWorker : ThreadedBackgroundWorkerBase, IBackgroundWorker
    {
        public ThreadedBackgroundWorker(string name, ILogger logger) : base(name, logger)
        {
        }

        public void Start(Action<CancellationToken> action)
        {
            base.Start(action);
        }

        protected override void DoWork(object state)
        {
            var action = (Action<CancellationToken>)state;

            try
            {
                action(CancellationToken);
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Thread '{Thread.CurrentThread.Name}' died", ex);
            }
        }
    }

    public class ThreadedBackgroundWorker<TState> : ThreadedBackgroundWorkerBase, IBackgroundWorker<TState>
    {
        public ThreadedBackgroundWorker(string name, ILogger logger) : base(name, logger)
        {
        }

        public void Start(Action<TState, CancellationToken> action)
        {
            base.Start(action);
        }

        public TState State { get; set; }

        protected override void DoWork(object state)
        {
            var action = (Action<TState, CancellationToken>)state;

            try
            {
                action(State, CancellationToken);
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Thread '{Thread.CurrentThread.Name}' died", ex);
            }
        }
    }
}
