using System;
using System.IO;
using System.ServiceProcess;
using log4net.Config;

namespace AiDollar.Gateway
{
    partial class AiDollarGateway : ServiceBase
    {
        public AiDollarGateway()
        {
            InitializeComponent();
        }

        static void Main(string[] args)
        {
            var currentDomain = AppDomain.CurrentDomain;
            Directory.SetCurrentDirectory(currentDomain.BaseDirectory);
            XmlConfigurator.Configure();

            var service = new AiDollarGateway();

            if (Environment.UserInteractive)
            {
                service.OnStart(args);
                Console.WriteLine("Press any key to stop program");

                Console.ReadKey();
                service.OnStop();
            }
            else
            {
                Run(service);
            }
        }

        protected override void OnStart(string[] args)
        {
            var httpRoot = AiDollar.ApiGateway.CompositionRoot.CompositeRootInstanace();
            var httpSvc = httpRoot.Initialize();
            httpSvc.Start();
        }

        protected override void OnStop()
        {

        }
    }
}
