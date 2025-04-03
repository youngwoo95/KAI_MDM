namespace MDM_KAI.Services
{
    public interface ILoggerService
    {
        /// <summary>
        /// file Log Message
        /// </summary>
        /// <param name="message"></param>
        public void FileLogMessage(string message);

        /// <summary>
        /// console Log Message
        /// </summary>
        /// <param name="message"></param>
        public void ConsoleLogMessage(string message);
    }
}
