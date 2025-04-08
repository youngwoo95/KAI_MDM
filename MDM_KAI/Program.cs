using MDM_KAI.Database;
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
            
            await host.RunAsync();
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
                 logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning); // Waring 경고 아니면 HTTP 로그안띄움.
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
                 // Memory Cache 사용
                 services.AddMemoryCache();

                 // BackgroundService를 등록하여 주기적으로 Application.RunAsync() 호출
                 services.AddHostedService<BackgrondWorker>();

                 services.AddSingleton<ISettingFiles, SettingFiles>();

                 // IDbConnectionFactory를 OracleConnectionFactory로 등록
                 services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(CommData.DbConnStr));

                 // Repository 등록
                 services.AddTransient<CommandRepository>();

             })
             .Build();
        }

       

  

    }
}
