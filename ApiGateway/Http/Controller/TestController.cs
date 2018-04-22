using System.Web.Http;
using AiDollar.Infrastructure.Logger;

namespace AiDollar.ApiGateway.Http.Controller
{
    public class TestController : ApiController
    {
        public TestController()
        {
          
        }
        public ILogger Logger { get; set; }

        [AllowAnonymous]
        [HttpGet]
        public string Get(string hello)
        {
            return hello;
        }
    }
}
