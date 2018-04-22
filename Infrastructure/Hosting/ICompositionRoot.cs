using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiDollar.Infrastructure.Hosting
{
    public interface ICompositionRoot<out TService> : IDisposable where TService : IService
    {
        TService Initialize();
    }
}
     
