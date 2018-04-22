using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiDollar.Infrastructure.Threading
{
    public interface IBackgroundWorkerFactory
    {
        IBackgroundWorker Create(string name);
        IBackgroundWorker<TState> Create<TState>(string name);
    }
}
