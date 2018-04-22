using System.Web.Http.ExceptionHandling;
using AiDollar.Infrastructure.Logger;

namespace AiDollar.ApiGateway.Http
{
    public class LoggingExceptionHandler : ExceptionHandler
    {
        private readonly ILogger _logger;

        public LoggingExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }

        public override void Handle(ExceptionHandlerContext context)
        {
            _logger.LogError(context.Request?.RequestUri?.ToString(), context.Exception);
            base.Handle(context);
        }
    }
}
