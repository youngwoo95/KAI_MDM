namespace MDM_KAI
{
    public class CommData
    {
        /// <summary>
        /// MDM URL
        /// </summary>
        public static string targetUrl = "http://123.2.156.148:8800";

        /// <summary>
        /// 데이터베이스 아이디
        /// </summary>
        public static string? DbUser { get; set; } = "stec";

        /// <summary>
        /// 데이터베이스 비밀번호
        /// </summary>
        public static string? DbPassword { get; set; } = "stec";

        /// <summary>
        /// 데이터베이스 주소
        /// </summary>
        public static string? DbHost { get; set; } = "127.0.0.1";

        /// <summary>
        /// 데이터베이스 포트
        /// </summary>
        public static string? DbPort { get; set; } = "1521";

        /// <summary>
        /// 데이터베이스 명
        /// </summary>
        public static string? DbName { get; set; } ="stecdb";

        /// <summary>
        /// 데이터베이스 연결 문자열
        /// </summary>
        public static string DbConnStr = $"User Id={DbUser};Password={DbPassword};Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={DbHost})(PORT={DbPort}))(CONNECT_DATA=(SERVICE_NAME={DbName})));Pooling=true;Min Pool Size=2;Max Pool Size=30;";
    }
}
