using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiDollar.Infrastructure.Logger;

namespace AiDollar.Infrastructure.Threading
{
    public static class BackgroundWorkerFactory
    {
        public static ILogger Logger { get; set; }
        public static IBackgroundWorkerFactory Default { get; } = new ThreadedBackgroundWorkerFactory();
        public static IBackgroundWorkerFactory Current { get; set; } = Default;
    }
}
