using MDM_KAI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace MDM_KAI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = HostBuilder(args);

            var app = host.Services.GetRequiredService<Application>();
            await app.RunAsync();

            await host.StopAsync();
        }

        /// <summary>
        /// HTTP Host Builder 세팅
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHost HostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
             .ConfigureLogging(logging =>
             {
                 logging.AddConsole();
                 logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
             })
             .ConfigureServices((context, services) =>
             {
                 // LoggerService 등록
                 services.AddSingleton<ILoggerService, LoggerService>();

                 services.AddHttpClient("MyClient", client =>
                 {
                     client.BaseAddress = new Uri(CommData.targetUrl);
                     client.Timeout = TimeSpan.FromSeconds(30);
                 })
                 .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                     retryCount: 5,
                     sleepDurationProvider: retryAttempt =>
                     {
                         var jitter = TimeSpan.FromMilliseconds(new Random().Next(0, 500));
                         return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + jitter;
                     },
                     onRetry: (outcome, timespan, retryAttempt, context) =>
                     {
                         Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds:F1} seconds due to {outcome.Exception?.Message}");
                     }));

                 // Application 클래스 등록
                 services.AddTransient<Application>();
             })
             .Build();
        }

    }
}
