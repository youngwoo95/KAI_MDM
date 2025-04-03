using MDM_KAI.DTO;

namespace MDM_KAI.Sample
{
    public class SampleData
    {
        public static List<MdmPostDTO> GenerateRandomData(int count)
        {
            var dataList = new List<MdmPostDTO>(count);
            var random = new Random();

            // 입출력 옵션 (예: "IN", "OUT")
            string[] inOutOptions = { "IN", "OUT" };

            for (int i = 0; i < count; i++)
            {
                var dto = new MdmPostDTO
                {
                    // userId를 전화번호 형식으로 생성 (예: 010-1234-5678)
                    userId = GenerateRandomPhoneNumber(random),
                    // inOut은 "IN" 또는 "OUT" 중 랜덤 선택
                    inOut = inOutOptions[random.Next(inOutOptions.Length)],
                    // gateNum은 "Gate1" ~ "Gate10" 중 랜덤 선택
                    gateNum = "Gate" + random.Next(1, 11),
                    // time은 최근 1년 이내의 랜덤한 날짜/시간 (형식: yyyyMMddHHmmss)
                    time = GenerateRandomTime(random)
                };

                dataList.Add(dto);
            }

            return dataList;
        }

        private static string GenerateRandomPhoneNumber(Random random)
        {
            // 일반적으로 한국 휴대폰 번호 형식: 010-xxxx-xxxx
            int part1 = random.Next(1000, 10000);
            int part2 = random.Next(1000, 10000);
            return $"010-{part1}-{part2}";
        }

        private static string GenerateRandomTime(Random random)
        {
            // 최근 1년 이내의 랜덤 날짜 생성
            DateTime start = DateTime.Now.AddYears(-1);
            int rangeDays = (DateTime.Now - start).Days;
            DateTime randomDate = start.AddDays(random.Next(rangeDays))
                                       .AddHours(random.Next(0, 24))
                                       .AddMinutes(random.Next(0, 60))
                                       .AddSeconds(random.Next(0, 60));
            return randomDate.ToString("yyyyMMddHHmmss");
        }
    }
}
