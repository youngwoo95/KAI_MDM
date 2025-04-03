using MDM_KAI.DTO;
using MDM_KAI.Sample;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System.Text;
using System.Text.Json;

namespace MDM_KAI
{
    internal class Program
    {
        private static IHttpClientFactory? httpClientFactory;

        // Thread-safe 카운터를 위한 변수
        private static int successCount = 0;
        private static int failureCount = 0;

        static async Task Main(string[] args)
        {
            using IHost host = HostBuilder(args);
            httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();

            List<MdmPostDTO> randomData = SampleData.GenerateRandomData(30000); // DB받아오는거
            
            var tasks = new List<Task>();
            // 동시에 실행될 요청 개수를 제한 (예: 100건)
            using var semaphore = new SemaphoreSlim(200);

            if (randomData.Count > 0)
            {
                for (int i = 0; i < randomData.Count; i++)
                {
                    await semaphore.WaitAsync();
                    int requestIndex = i; // 클로저 문제 방지용

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var client = httpClientFactory.CreateClient("MyClient");

                            // DTO 객체를 JSON으로 직렬화 (requestIndex를 사용)
                            var jsonData = JsonSerializer.Serialize(randomData[requestIndex]);
                            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                            var response = await client.PostAsync("/api/Sample/SampleCount", content);
                            response.EnsureSuccessStatusCode();

                            var responseContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Request {requestIndex}: Success, Response: {responseContent}");

                            // 성공 카운터 증가 (스레드 안전)
                            Interlocked.Increment(ref successCount);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"에러 {requestIndex}: Failed, Error: {ex.Message}");
                            // 실패 카운터 증가 (스레드 안전)
                            Interlocked.Increment(ref failureCount);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }
            }

            await Task.WhenAll(tasks);
            await host.StopAsync(); // 호스트 종료 시 로그 플러시
            Console.WriteLine($"========================================Total Success: {successCount}, Total Failures: {failureCount}===============================");
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
             })
             .Build();
        }

    }
}
