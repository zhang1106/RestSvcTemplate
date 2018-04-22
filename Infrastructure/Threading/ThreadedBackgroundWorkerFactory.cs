using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiDollar.Infrastructure.Threading
{
    public class ThreadedBackgroundWorkerFactory : IBackgroundWorkerFactory
    {
        public IBackgroundWorker Create(string name)
        {
            return new ThreadedBackgroundWorker(name, BackgroundWorkerFactory.Logger);
        }

        public IBackgroundWorker<TState> Create<TState>(string name)
        {
            return new ThreadedBackgroundWorker<TState>(name, BackgroundWorkerFactory.Logger);
        }
    }
}
