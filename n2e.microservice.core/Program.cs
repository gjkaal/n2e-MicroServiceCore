using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace n2e.MicroService.Core
{
    /*
     * For using Ioc:
     * Install-Package Microsoft.Extensions.DependencyInjection.Abstractions
     * 
     * For using generic logging:
     * Install-Package Microsoft.Extensions.Logging
     * Install-Package Microsoft.Extensions.Logging.Abstractions
     * 
     * Packages facilitating a hosted process: 
     * Install-Package Microsoft.Extensions.Hosting
     * Install-Package Microsoft.Extensions.Hosting.Abstractions
     * 
     * Packages for using configuration options:
     * Install-Package Microsoft.Extensions.Configuration
     * Install-Package Microsoft.Extensions.Configuration.Abstractions
     *
     * Package for using service worker:
     * Install-Package  Microsoft.Extensions.Hosting.WindowsServices
     */

    /// <summary>
    /// Sample program using the setup
    /// </summary>
    class Program
    {
        protected Program() { }

        static async Task Main(string[] args)
        {
            var host = new HostConfiguration(args).CreateHost();
            var cancellationToken = new CancellationToken();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            await host.RunAsync(cancellationToken);
            logger.LogInformation("Program end");
        }
    }

    public interface IConsoleWriter
    {
        void WriteMessage(string message);
    }

    public static class Constants
    {
        public const string ApiUserName = "ApiUserName";
    }

    public class ConsoleWriter : IConsoleWriter
    {
        private readonly ILogger<ConsoleWriter> _logger;
        private readonly IConfiguration _config;
        private readonly string _userName;
        public ConsoleWriter(ILogger<ConsoleWriter> logger, IConfiguration config)
        {
            _logger = logger;
            _logger.LogInformation("Console writer initialized");
            _config = config;
            _userName = config[Constants.ApiUserName];
        }

        public void WriteMessage(string message)
        {
            _logger.LogInformation($"Running as {_userName}");
            _logger.LogInformation("WriteMessage starting");
            Console.WriteLine(message);
            _logger.LogInformation("WriteMessage complete");
        }
    }

}
