using IscomG2C.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Quartz;

namespace InformationSystem.Jobs
{
    /// <summary>
    /// 農委會資料開放平台-植物疫情預警
    /// 打上DisallowConcurrentExecution標簽讓Job進行單執行緒跑，避免沒跑完時的重復執行，
    /// </summary>
    [DisallowConcurrentExecution]
    public class PestNoticeJob: IJob
    {

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(async () =>
            {
                await QueryData();
            });
        }

        public static async Task QueryData()
        {
            using (var client = new HttpClient())
            {
                List<PestNotice> List = new List<PestNotice>();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var res = await client.GetAsync(new Uri(PestNoticeConfig.PestNoticeDataUrl));
                JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());
                try
                {
                    foreach (var d in data)
                    {
                        JObject obj = JObject.Parse(d.ToString());
                        List.Add(new PestNotice()
                        {
                            Title = obj["主旨"].ToString(),
                            Content = obj["疫情內容"].ToString(),
                            SuggestedMedication = obj["建議用藥"].ToString(),
                            ProvideCiry = obj["發布縣市"].ToString(),
                            PlantType = obj["植物品項"].ToString(),
                            CreatedAt = obj["發布日期"].ToString(),
                            ProvideUnit = obj["發布單位"].ToString(),
                        }); ;
                    }
                    // 寫入 指定json 檔案
                    string _directory = AppDomain.CurrentDomain.BaseDirectory;
                    _directory = String.Concat(_directory, "/assets/other_api");
                    FileHelper.CreateFolder(_directory);
                    File.WriteAllText($"{_directory}/pest_notice.json", JsonConvert.SerializeObject(List));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    /// <summary>
    /// 農委會資料開放平台-植物疫情預警
    /// </summary>
    public class PestNotice
    {
        /// <summary>
        /// 主旨
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// 疫情內容
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// 建議用藥
        /// </summary>
        /// <value>
        /// The suggested medication.
        /// </value>
        public string SuggestedMedication { get; set; }

        /// <summary>
        /// 發布縣市
        /// </summary>
        /// <value>
        /// The provider ciry.
        /// </value>
        public string ProvideCiry { get; set; }

        /// <summary>
        /// 植物品項
        /// </summary>
        /// <value>
        /// The type of the plant.
        /// </value>
        public string PlantType { get; set; }

        /// <summary>
        /// 發布日期
        /// </summary>
        /// <value>
        /// The provide date.
        /// </value>
        public string CreatedAt { get; set; }

        /// <summary>
        /// 發布單位
        /// </summary>
        /// <value>
        /// The provide unit.
        /// </value>
        public string ProvideUnit { get; set; }
    }

    public static class PestNoticeConfig
    {
        /// <summary>
        /// 農委會資料開放平台-植物疫情預警
        /// </summary>
        public static string PestNoticeDataUrl = "https://data.coa.gov.tw/Service/OpenData/FromM/PestNoticeData.aspx";

        /// <summary>
        /// 每日早上 6點
        /// </summary>
        public static string CronTime = "0 38 13 * * ? *";
    }
}