namespace MDM_KAI.DTO
{
    /// <summary>
    /// POST DTO
    /// </summary>
    public class MdmPostDTO
    {
        /// <summary>
        /// 사번 --> 전화번호
        /// </summary>
        public string? userId { get; set; }

        /// <summary>
        /// 사용자의 출입시스템 입실/퇴실 정보
        /// </summary>
        public string? inOut { get; set; }

        /// <summary>
        /// 입출입 게이트 정보
        /// </summary>
        public string? gateNum { get; set; }

        /// <summary>
        /// 입/출입 발생시간 (년월일시분초)
        /// %Y%m%d%H%i%s
        /// </summary>
        public string? time { get; set; }
    }
}
