using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiDollar.Infrastructure.Hosting
{
    public interface IService
    {
        string Name { get; }
        void Start();
        void Stop();
    }
}
