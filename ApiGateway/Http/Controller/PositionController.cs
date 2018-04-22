using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Bam.Compliance.ApiGateway.Models;
using Bam.Compliance.ApiGateway.Services;

namespace Bam.Compliance.ApiGateway.Http.Controller
{
    public class PositionController : ApiController
    {
        private readonly IIntradayAggrRetriever _intradayRetriever;
        public PositionController (IIntradayAggrRetriever intradayRetriever)
        {
            _intradayRetriever = intradayRetriever;
        }

        [AllowAnonymous]
        public List<Position> GetIntradayPositions(string symbol)
        {
            var positions = _intradayRetriever.GetPositions(symbol).ToList();
            return positions;
        }
    }
}
