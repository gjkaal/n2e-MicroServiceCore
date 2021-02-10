using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using n2e.MicroService.Core.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace n2e.MicroService.Core
{
    public class SampleBackgroundWorker : BackgroundService
    {
        private const int fixedUpdateTimer = 200;
        private readonly IServiceProvider _services;
        private readonly ILogger<SampleBackgroundWorker> _logger;

        public SampleBackgroundWorker(IServiceProvider services, ILogger<SampleBackgroundWorker> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var myConsole = _services.GetRequiredService<IConsoleWriter>();
                myConsole.WriteMessage("Hello World");

                var theService = _services.GetService<IActorSheduler>();
                await theService.Start(fixedUpdateTimer, stoppingToken);

                foreach (var actor in _services.GetServices<IActor>())
                {
                    theService.Register(actor);
                }
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "An error occurred.");
            }
        }
    }

}
