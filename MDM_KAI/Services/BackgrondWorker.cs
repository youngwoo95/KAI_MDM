using Microsoft.Extensions.Hosting;

namespace MDM_KAI.Services
{
    public class BackgrondWorker : BackgroundService
    {
        private readonly Application Application;
        private readonly ILoggerService LoggerService;

        public BackgrondWorker(Application _application, ILoggerService _loggerService)
        {
            this.Application = _application;
            LoggerService = _loggerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Application.RunAsync();
                }
                catch(Exception ex)
                {
                    LoggerService.FileLogMessage(ex.ToString());
                }

                // RunAsync 실행 완료 후 1분 대기
                try
                {
                    //await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                }
                catch(TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
