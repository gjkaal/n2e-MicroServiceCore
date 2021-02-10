using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using n2e.MicroService.Core.Abstractions;
using System;

namespace n2e.MicroService.Core
{
    public class HostConfiguration {

        private string[] _args;
        public HostConfiguration(string[] args)
        {
            _args = args;
        }

        public  IHost CreateHost()
        {
            return CreateHostBuilder().Build();
        }

        public IHostBuilder CreateHostBuilder() {

            var action = _args.FindStartupAction(StartupAction.Console);

            var host = Host
                // CreateDefaultBuilder adds logging providers for console, debug, eventsource
                // configure loglevels in appsettings.json or in code using the ConfigreLogging extension
                // from Microsoft.Extensions.Logging
                .CreateDefaultBuilder()
                // Logging can be configured multiple times.
                .ConfigureLogging(m => {
                    m.AddSimpleConsole(options =>
                    {
                        options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
                        options.IncludeScopes = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    });
                })
                // Use Configure services to add other service definitions
                // Best practice is to add a specific method to configure application specific dependency
                .ConfigureServices(c => ConfigureProgramServices(c))
                // When using other di framework (e.g. SimpleInjector), 
                // the setup can be extended using ConfigureContainer.

                // Configure IConfiguration
                .ConfigureAppConfiguration(config => {
                    // Add all environment variables, or only the variables with a prefix
                    // When a prefix is used, the prefix itself is removed from the key
                    config.AddEnvironmentVariables(prefix: "n2e_");
                    // Add command line variables using AddCommandLine
                    // instead of defining a specific set, the 'CreateDefaultBuilder' can be used
                    // with the arguments, with the same result.
                    config.AddCommandLine(_args);
                    // Specific INI, JSON and XML file can be added with file configuration
                    // Set the config file as optional (if it is optional...)
                    config.AddJsonFile("config.json", true);
                    // User secrets are located in the profile directory
                    // https://patrickhuber.github.io/2017/07/26/avoid-secrets-in-dot-net-core-tests.html
                    // On the command line:
                    //  dotnet user-secrets set ApiUserName myUserName
                    config.AddUserSecrets<Program>();
                });

            switch (action)
            {
                case StartupAction.Console:
                    host.UseConsoleLifetime();
                    break;
                case StartupAction.Service:
                    host.UseWindowsService();
                    break;
                default:

                    throw new NotSupportedException($"Cannot configure setup for {action}");
            }
           
            return host;
        }

        public static void ConfigureProgramServices(IServiceCollection services)
        {
            services.AddScoped<IConsoleWriter, ConsoleWriter>();
            services.AddSingleton<IActorSheduler, ActorSheduler>();

            services.AddScoped<IActor, Testclass1>();
            services.AddScoped<IActor, Testclass2>();

            services.AddHostedService<SampleBackgroundWorker>();
        }
    }

}
