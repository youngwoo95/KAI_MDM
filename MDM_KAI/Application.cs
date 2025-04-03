using MDM_KAI.DTO;
using MDM_KAI.Sample;
using MDM_KAI.Services;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace MDM_KAI
{
    public class Application
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly ILoggerService LoggerService;

        public Application(IHttpClientFactory _httpclientfactory, ILoggerService _loggerservice)
        {
            this.HttpClientFactory = _httpclientfactory;
            this.LoggerService = _loggerservice;
        }

        public async Task RunAsync()
        {
            LoggerService.FileLogMessage("[START]");

            List<MdmPostDTO> randomData = SampleData.GenerateRandomData(30000);

            var tasks = new List<Task>();
            using var semaphore = new SemaphoreSlim(200);

            for (int i = 0; i < randomData.Count; i++)
            {
                await semaphore.WaitAsync();
                int requestIndex = i;

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var client = HttpClientFactory.CreateClient("MyClient");

                        // DTO 객체를 JSON으로 직렬화 (requestIndex를 사용)
                        var jsonData = JsonSerializer.Serialize(randomData[requestIndex]);
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        // POST 요청
                        var response = await client.PostAsync("/api/Sample/SampleCount", content);
                        response.EnsureSuccessStatusCode();

                        // Response반환
                        var responseContent = await response.Content.ReadAsStringAsync();
                        LoggerService.ConsoleLogMessage($"{requestIndex} \t {responseContent}");
                    }
                    catch (Exception ex)
                    {
                        LoggerService.FileLogMessage($"{requestIndex} \t {ex.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

    }
}
