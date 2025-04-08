namespace MDM_KAI.Services
{
    public interface ISettingFiles
    {
        /// <summary>
        /// 설정파일 읽기
        /// </summary>
        /// <returns></returns>
        public bool SettingFileRead();

        /// <summary>
        /// 설정파일 저장
        /// </summary>
        /// <returns></returns>
        public Task SettingFileSave();
    }
}
