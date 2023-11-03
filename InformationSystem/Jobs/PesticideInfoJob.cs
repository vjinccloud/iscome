using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using IscomG2C.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;

namespace InformationSystem.Jobs
{
    /// <summary>
    /// 農委會資料開放平台-農藥資料查詢
    /// 打上DisallowConcurrentExecution標簽讓Job進行單執行緒跑，避免沒跑完時的重復執行，
    /// </summary>
    [DisallowConcurrentExecution]
    public class PesticideInfoJob : IJob
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
                List<PesticideInfo> List = new List<PesticideInfo>();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var res = await client.GetAsync(new Uri(PesticideInfoConfig.Url));
                JArray data = JArray.Parse(await res.Content.ReadAsStringAsync());
                try
                {
                    foreach (var d in data)
                    {
                        JObject obj = JObject.Parse(d.ToString());
                        List.Add(new PesticideInfo()
                        {
                            LicenseWord = obj["許可證字"].ToString(),
                            LicenseNumber = obj["許可證號"].ToString(),
                            ChineseName = obj["中文名稱"].ToString(),
                            PesticideCode = obj["農藥代號"].ToString(),
                            EnglishName = obj["英文名稱"].ToString(),
                            BrandName = obj["廠牌名稱"].ToString(),
                            ChemicalComposition = obj["化學成分"].ToString(),
                            Manufacturer = obj["國外原製造廠商"].ToString(),
                            DosageForm = obj["劑型"].ToString(),
                            Content = obj["含量"].ToString(),
                            ValidityPeriod = obj["有效期限"].ToString(),
                            TradeName = obj["廠商名稱"].ToString(),
                            FRACFungicideResistance = obj["FRAC殺菌劑抗藥性"].ToString(),
                            HRACHerbicideResistance = obj["HRAC除草劑抗藥性"].ToString(),
                            IRACInsecticideResistance = obj["IRAC殺蟲劑抗藥性"].ToString(),
                            PesticideApplicationRange = obj["農藥使用範圍"].ToString(),
                            LicenseMarking = obj["許可證標示"].ToString(),
                        });
                    }
                    // 寫入 指定json 檔案
                    string _directory = AppDomain.CurrentDomain.BaseDirectory;
                    _directory = String.Concat(_directory, "/assets/other_api");
                    FileHelper.CreateFolder(_directory);
                    File.WriteAllText($"{_directory}/pesticide_info.json", JsonConvert.SerializeObject(List));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }

    public class PesticideInfo
    {
        /// <summary>
        /// 許可證字
        /// </summary>
        /// <value></value>
        public string LicenseWord { get; set; }
        /// <summary>
        /// 許可證號
        /// </summary>
        /// <value></value>
        public string LicenseNumber { get; set; }
        /// <summary>
        /// 中文名稱
        /// </summary>
        /// <value></value>
        public string ChineseName { get; set; }
        /// <summary>
        /// 農藥代號
        /// </summary>
        /// <value></value>
        public string PesticideCode { get; set; }
        /// <summary>
        /// 英文名稱
        /// </summary>
        /// <value></value>
        public string EnglishName { get; set; }
        /// <summary>
        /// 廠牌名稱
        /// </summary>
        /// <value></value>
        public string BrandName { get; set; }
        /// <summary>
        /// 化學成分
        /// </summary>
        /// <value></value>
        public string ChemicalComposition { get; set; }
        /// <summary>
        /// 國外原製造廠商
        /// </summary>
        /// <value></value>
        public string Manufacturer { get; set; }
        /// <summary>
        /// 劑型
        /// </summary>
        /// <value></value>
        public string DosageForm { get; set; }
        /// <summary>
        /// 含量
        /// </summary>
        /// <value></value>
        public string Content { get; set; }
        /// <summary>
        /// 有效期限
        /// </summary>
        /// <value></value>
        public string ValidityPeriod { get; set; }
        /// <summary>
        /// 廠商名稱
        /// </summary>
        /// <value></value>
        public string TradeName { get; set; }
        /// <summary>
        /// FRAC殺菌劑抗藥性
        /// </summary>
        /// <value></value>
        public string FRACFungicideResistance { get; set; }
        /// <summary>
        /// HRAC除草劑抗藥性
        /// </summary>
        /// <value></value>
        public string HRACHerbicideResistance { get; set; }
        /// <summary>
        /// IRAC殺蟲劑抗藥性
        /// </summary>
        /// <value></value>
        public string IRACInsecticideResistance { get; set; }
        /// <summary>
        /// 農藥使用範圍
        /// </summary>
        /// <value></value>
        public string PesticideApplicationRange { get; set; }
        /// <summary>
        /// 許可證標示
        /// </summary>
        /// <value></value>
        public string LicenseMarking { get; set; }
    }

    public static class PesticideInfoConfig
    {
        /// <summary>
        /// 農委會資料開放平台-農藥資料查詢
        /// </summary>
        public static string Url = "https://data.coa.gov.tw/Service/OpenData/FromM/PesticideData.aspx";

        /// <summary>
        /// 每週一早上 7點
        /// </summary>
        public static string CronTime = "0 0 7 ? * MON *";
    }
}