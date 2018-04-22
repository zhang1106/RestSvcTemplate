using System;
using AiDollar.Infrastructure.Configuration;

namespace AiDollar.ApiGateway
{
    public class RabbitMqSettings : AppSettingsBase
    {
        public RabbitMqSettings() : base ("ApiGateway.RabbitMq.")
        {

        }

        public string Endpoint
        {
            get { return GetValue(() => Endpoint); }
        }

        public string IntradayTradesExchange
        {
            get { return GetValue(() => IntradayTradesExchange); }
        }

        public string IntradayOrdersExchange
        {
            get { return GetValue(() => IntradayOrdersExchange); }
        }

        public string IntradayPositionsExchange
        {
            get { return GetValue(() => IntradayPositionsExchange); }
        }

        public string EndOfDayTradesExchange
        {
            get { return GetValue(() => EndOfDayTradesExchange); }
        }

        public string EndOfDayOrdersExchange
        {
            get { return GetValue(() => EndOfDayOrdersExchange); }
        }

        public string EndOfDayPositionsExchange
        {
            get { return GetValue(() => EndOfDayPositionsExchange); }
        }

        public TimeSpan RetryInterval
        {
            get { return GetValue(() => RetryInterval, TimeSpan.FromSeconds(5)); }
        }

        public bool PublishTrades
        {
            get { return GetValue(() => PublishTrades, true); }
        }

        public bool PublishOrders
        {
            get { return GetValue(() => PublishOrders); }
        }

        public bool PublishPositions
        {
            get { return GetValue(() => PublishPositions); }
        }
    }
}
