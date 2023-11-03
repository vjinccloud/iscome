using IscomG2C.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace InformationSystem.Jobs
{
    /// <summary>
    /// 農委會資料開放平台-農藥名稱查詢
    /// 打上DisallowConcurrentExecution標簽讓Job進行單執行緒跑，避免沒跑完時的重復執行，
    /// </summary>
    [DisallowConcurrentExecution]
    public class PesticideNameJob : IJob
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
                List<PesticideName> List = new List<PesticideName>();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var res = await client.GetAsync(new Uri(PesticideNameConfig.Url));
                JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());
                try
                {
                    foreach (var d in data)
                    {
                        JObject obj = JObject.Parse(d.ToString());
                        List.Add(new PesticideName()
                        {
                            Name = obj["Name"].ToString(),
                            EnName = obj["EnName"].ToString(),
                            CompositPercentage = obj["CompositPercentage"].ToString(),
                            PesticidesForm = obj["PesticidesForm"].ToString(),
                            CompanyName = obj["CompanyName"].ToString(),
                            ProductName = obj["ProductName"].ToString(),
                        });
                    }
                    // 寫入 指定json 檔案
                    string _directory = AppDomain.CurrentDomain.BaseDirectory;
                    _directory = String.Concat(_directory, "/assets/other_api");
                    FileHelper.CreateFolder(_directory);
                    File.WriteAllText($"{_directory}/pesticide_name.json", JsonConvert.SerializeObject(List));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    public class PesticideName
    {
        public string Name { get; set; }
        public string EnName { get; set; }
        public string CompositPercentage { get; set; }
        public string PesticidesForm { get; set; }
        public string CompanyName { get; set; }
        public string ProductName { get; set; }

    }

    public static class PesticideNameConfig
    {
        /// <summary>
        /// 農委會資料開放平台-農藥資料查詢
        /// </summary>
        public static string Url = "https://data.coa.gov.tw/Service/OpenData/FromM/TacTriPesticideData.aspx";

        /// <summary>
        /// 每月一日早上 5點
        /// </summary>
        public static string CronTime = "0 0 5 1 * ? *";
    }
}