namespace MDM_KAI.Services
{
    public class LoggerService : ILoggerService
    {
        // 동시에 접근할 가능성이 있을까봐서 - 락
        private static readonly object LogLock = new object();

        /// <summary>
        /// 파일로그
        /// </summary>
        public void FileLogMessage(string message)
        {
            try
            {
                DateTime Today = DateTime.Now;
                string dir_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemLog");

                DirectoryInfo di = new DirectoryInfo(dir_path);

                if(!di.Exists)
                {
                    di.Create();
                }

                // 년도 파일 없으면 생성
                dir_path = Path.Combine(dir_path, Today.Year.ToString());
                di = new DirectoryInfo(dir_path);
                if(!di.Exists)
                {
                    di.Create();
                }

                dir_path = Path.Combine(dir_path, Today.Month.ToString());
                di = new DirectoryInfo(dir_path);

                // 월 파일 없으면 생성
                if(!di.Exists)
                {
                    di.Create();
                }

                dir_path = Path.Combine(dir_path, $"{Today.Year}_{Today.Month}_{Today.Day}.txt");

                lock(LogLock)
                {
                    using (var fs = new FileStream(dir_path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]\t{message}");
#if DEBUG
                        Console.WriteLine($"[INFO] {message}");
#endif
                    }
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 콘솔 로그
        /// </summary>
        /// <param name="message"></param>
        public void ConsoleLogMessage(string message)
        {
            try
            {
                lock (LogLock)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Green;

                    // 로그 출력
                    Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]\t{message}");

                    Console.ResetColor();
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

    }
}
