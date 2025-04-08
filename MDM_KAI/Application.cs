using MDM_KAI.DTO;
using MDM_KAI.Sample;
using MDM_KAI.Services;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using MDM_KAI.Database;

namespace MDM_KAI
{
    public class Application
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly IMemoryCache MemoryCache;
        private readonly ILoggerService LoggerService;
        private readonly CommandRepository CommandRepository;

        public Application(IHttpClientFactory _httpclientfactory,
            IMemoryCache _memorycache,
            ILoggerService _loggerservice,
            CommandRepository _commandrepository)
        {
            this.HttpClientFactory = _httpclientfactory;
            this.MemoryCache = _memorycache;
            this.LoggerService = _loggerservice;
            this.CommandRepository = _commandrepository;
        }

        public async Task RunAsync()
        {
            LoggerService.FileLogMessage("[START]");

            List<MdmPostDTO> randomData = SampleData.GenerateRandomData(30000);

            var tasks = new List<Task>();
            using var semaphore = new SemaphoreSlim(200);

            var temp = CommandRepository.GetAll();
            Console.WriteLine("");

            for (int i = 0; i < randomData.Count; i++)
            {
                int requestIndex = i;
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        // 실패한 전화번호(여기서는 userId)를 캐시에서 확인하고, 있으면 건너뜁니다.
                        string phoneNumber = randomData[requestIndex].userId;
                        if (MemoryCache.TryGetValue(phoneNumber, out _))
                        {
                            LoggerService.FileLogMessage($"{requestIndex} \t {phoneNumber}는 최근 실패하여 10분간 요청하지 않습니다.");
                            return;
                        }

                        var client = HttpClientFactory.CreateClient("MyClient");

                        // DTO 객체를 JSON으로 직렬화 (requestIndex를 사용)
                        var jsonData = JsonSerializer.Serialize(randomData[requestIndex]);
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        // POST 요청
                        var response = await client.PostAsync("/api/Sample/SampleCount", content);

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            // 응답 문자열 읽기
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var result = JsonSerializer.Deserialize<MDMReturnDTO>(responseContent);

                            if (result != null && result.rsltCd == "1")
                            {
                                LoggerService.ConsoleLogMessage($"{requestIndex} \t 성공: {responseContent}");
                            }
                            else
                            {
                                // 실패 시 요청 데이터의 식별자(ID)와 함께 로그 기록
                                //LoggerService.FileLogMessage($"{requestIndex} \t 실패: {result?.rsltMsg ?? "알 수 없는 오류"} - Data ID: {randomData[requestIndex].userId}");
                                Console.WriteLine($"{requestIndex} \t 실패: {result.rsltCd} - {result?.rsltMsg} - Data ID: {randomData[requestIndex].userId}");
                                
                                MemoryCache.Set(phoneNumber, new MemoryCacheEntryOptions
                                {
                                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                                });
                            }
                        }
                        else // 실패 400, 500 등등 실패건 다 여기
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"{requestIndex} \t 실패: NOT 200 OK - Data ID: {phoneNumber}, 응답내용: {errorContent}");

                            // 실패 시 해당 전화번호를 캐시에 추가
                            MemoryCache.Set(phoneNumber, true, new MemoryCacheEntryOptions
                            {
                                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerService.FileLogMessage($"{requestIndex} \t {ex.Message}");

                        // 예외 발생 시에도 해당 전화번호를 캐시에 추가
                        string phoneNumber = randomData[requestIndex].userId;
                        MemoryCache.Set(phoneNumber, true, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                        });
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
