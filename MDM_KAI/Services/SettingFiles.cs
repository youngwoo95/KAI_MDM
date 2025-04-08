using Newtonsoft.Json.Linq;

namespace MDM_KAI.Services
{
    public class SettingFiles : ISettingFiles
    {
        private static readonly object SettingLock = new object();

        private readonly ILoggerService LoggerService;

        public SettingFiles(ILoggerService _loggerService)
        {
            this.LoggerService = _loggerService;   
        }

        /// <summary>
        /// 설정파일 읽기
        /// </summary>
        /// <returns></returns>
        public bool SettingFileRead()
        {
            try
            {
                lock (SettingLock)
                {
                    string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", "MDMSettingPath.txt");
                    if (File.Exists(FilePath))
                    {
                        // JSON 파일 읽기 및 역직렬화
                        string json = File.ReadAllText(FilePath);
                        JObject JsonParse = JObject.Parse(json);

                        // 데이터베이스 IP
                        CommData.DbHost = JsonParse["dbHost"]?.ToString() ?? String.Empty;
                        // 데이터베이스 PORT
                        CommData.DbPort = JsonParse["dbPort"]?.ToString() ?? String.Empty;
                        // 데이터베이스 ID
                        CommData.DbUser = JsonParse["dbUser"]?.ToString() ?? String.Empty;
                        // 데이터베이스 PW
                        CommData.DbPassword = JsonParse["dbPassword"]?.ToString() ?? String.Empty;
                        // 데이터베이스 명
                        CommData.DbName = JsonParse["dbName"]?.ToString() ?? String.Empty;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                LoggerService.FileLogMessage(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 설정파일 저장
        /// </summary>
        /// <returns></returns>
        public Task SettingFileSave()
        {
            throw new NotImplementedException();
        }
    }
}
