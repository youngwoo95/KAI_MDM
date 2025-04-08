namespace MDM_KAI.Database.DTO
{
    public class GunTaeEventModel
    {
        public string MC_ID { get; set; } = null!;
        public string SC_ID { get; set; } = null!;
        public string DOOR_ID { get; set; } = null!;
        public DateTime EVT_TIME { get; set; }
        public string? CR_NO { get; set; }
        public string? CR_LOCATION { get; set; }
        public string? DOOR_MODE { get; set; }
        public string? CARD_AUTH { get; set; }
        public string? DOOR_STATUS { get; set; }
        public string? BUTTON_STATUS { get; set; }
        public string? CARD_LENGTH { get; set; }
        public string? CARD_NO { get; set; }
        public string? CARD_ID { get; set; }
        public string? IS_GIMUN { get; set; }
        public string HUMAN_ID { get; set; } = null!;
        public string? EVT_REASON { get; set; }
        public string? COMMAND_CODE { get; set; }
        public string? EVT_CODE { get; set; }
        public DateTime? WORK_DATE { get; set; }
        public string MSG_ID { get; set; } = null!;


    }
}
 