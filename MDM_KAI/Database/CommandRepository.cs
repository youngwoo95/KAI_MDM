using Dapper;
using MDM_KAI.Database.DTO;
using System.Data;

namespace MDM_KAI.Database
{
    /*
     Insert/Update/Delete / Execute / return int / 영향받은 행 수 반환
     단일 값 조회 / ExecuteScalar<T> / T / 첫 번째 컬럼 값 반환
     한줄 조회 / QuerySingle<T> / T / 하나의 행 반환 (없거나 여러 개면 예외)
     여러 줄 조회 / Query<T> / IEnumerable<T> / 여러 행 반환
     */

    public class CommandRepository
    {
        private readonly IDbConnectionFactory ConnectionFactory;

        public CommandRepository(IDbConnectionFactory _connectionfactory)
        {
            this.ConnectionFactory = _connectionfactory;
        }

        /// <summary>
        /// MDM TARGET - 작성해야할듯 (SELECT)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GunTaeEventModel> GetAll()
        {
            using (IDbConnection db = ConnectionFactory.CreateConnection())
            {
                string sql = "SELECT * from GUNTAE_EVENT";
                

                //string sql = "SELECT EVT_TIME as GETTIME, " +
                //    "HUMAN_ID, " +
                //    "FN_GET_SABUN(HUMAN_ID) as SABUN, " +
                //    "BUTTON_STATUS as BUTTONSTATE, " +
                //    "DECODE(BUTTON_STATUS,'1','TRUE','2','FALSE','3','FALSE','4','TRUE',BUTTON_STATUS) as BUTTONRESULT, " +
                //    "SEND_YN " +
                //    "FROM GUNTAE_EVENT " +
                //    "WHERE EVT_REASON = 'C' AND COMMAND_CODE = 'E' AND EVT_CODE = '1' AND HUMAN_ID is not null AND FN_GET_SABUN(HUMAN_ID) is not null AND EVT_TIME >= (SELECT )";
                return db.Query<GunTaeEventModel>(sql);
            }
        }

        /// <summary>
        /// 새로운 GunTaeEventModel 레코드를 삽입합니다.
        /// </summary>
        public int Create(GunTaeEventModel newEvent)
        {
            using (IDbConnection db = ConnectionFactory.CreateConnection())
            {
                string sql = @"
                    INSERT INTO GUNTAE_EVENT
                        (MC_ID, SC_ID, DOOR_ID, EVT_TIME, CR_NO, CR_LOCATION, DOOR_MODE, CARD_AUTH, DOOR_STATUS, 
                         BUTTON_STATUS, CARD_LENGTH, CARD_NO, CARD_ID, IS_GIMUN, HUMAN_ID, EVT_REASON, COMMAND_CODE, EVT_CODE, WORK_DATE, MSG_ID)
                    VALUES
                        (@MC_ID, @SC_ID, @DOOR_ID, @EVT_TIME, @CR_NO, @CR_LOCATION, @DOOR_MODE, @CARD_AUTH, @DOOR_STATUS, 
                         @BUTTON_STATUS, @CARD_LENGTH, @CARD_NO, @CARD_ID, @IS_GIMUN, @HUMAN_ID, @EVT_REASON, @COMMAND_CODE, @EVT_CODE, @WORK_DATE, @MSG_ID)";

                return db.Execute(sql, newEvent);
            }
        }

        /// <summary>
        /// 특정 GunTaeEventModel 레코드를 업데이트합니다.
        /// 여기서는 예제로 HUMAN_ID를 기준으로 업데이트합니다.
        /// </summary>
        public int Update(GunTaeEventModel updatedEvent)
        {
            using (IDbConnection db = ConnectionFactory.CreateConnection())
            {
                string sql = @"
                    UPDATE GUNTAE_EVENT
                    SET MC_ID = @MC_ID,
                        SC_ID = @SC_ID,
                        DOOR_ID = @DOOR_ID,
                        EVT_TIME = @EVT_TIME,
                        CR_NO = @CR_NO,
                        CR_LOCATION = @CR_LOCATION,
                        DOOR_MODE = @DOOR_MODE,
                        CARD_AUTH = @CARD_AUTH,
                        DOOR_STATUS = @DOOR_STATUS,
                        BUTTON_STATUS = @BUTTON_STATUS,
                        CARD_LENGTH = @CARD_LENGTH,
                        CARD_NO = @CARD_NO,
                        CARD_ID = @CARD_ID,
                        IS_GIMUN = @IS_GIMUN,
                        EVT_REASON = @EVT_REASON,
                        COMMAND_CODE = @COMMAND_CODE,
                        EVT_CODE = @EVT_CODE,
                        WORK_DATE = @WORK_DATE,
                        MSG_ID = @MSG_ID
                    WHERE HUMAN_ID = @HUMAN_ID";

                return db.Execute(sql, updatedEvent);
            }
        }

        /// <summary>
        /// 특정 GunTaeEventModel 레코드를 삭제합니다.
        /// 여기서는 예제로 HUMAN_ID를 기준으로 삭제합니다.
        /// </summary>
        public int Delete(string humanId)
        {
            using (IDbConnection db = ConnectionFactory.CreateConnection())
            {
                string sql = "DELETE FROM GUNTAE_EVENT WHERE HUMAN_ID = @HumanId";
                return db.Execute(sql, new { HumanId = humanId });
            }
        }

    }
}
