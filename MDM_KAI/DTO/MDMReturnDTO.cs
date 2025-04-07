namespace MDM_KAI.DTO
{
    public class MDMReturnDTO
    {
        /// <summary>
        /// 요청 응답코드
        /// 1 : 성공 (rsltMsg는 empty)
        /// 1000 : Invalid Request Data
        /// 1998 : No SpeedGate Information
        /// 1999 : No User Information
        /// </summary>
        public string? rsltCd { get; set; }

        /// <summary>
        /// 요청 응답 메시지
        /// </summary>
        public string? rsltMsg { get; set; }
    }
}
