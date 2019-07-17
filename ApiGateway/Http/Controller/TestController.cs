using System.Web.Http;
using AiDollar.Infrastructure.Logger;

namespace AiDollar.ApiGateway.Http.Controller
{
    public class TestController : ApiController
    { 
        public ILogger Logger { get; set; }

        [AllowAnonymous]
        [HttpGet]
        public string Get()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            
            return version.ToString();
        }
    }
}
