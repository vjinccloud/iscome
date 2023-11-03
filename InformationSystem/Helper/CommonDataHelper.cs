using ICCModule.Models;
using ICCModule.Models.PreventionInfo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace InformationSystem.Helper
{
    public static class CommonDataHelper
    {
        public static List<CityDistricts> CityDistricts
        {
            get
            {
                string _directory = AppDomain.CurrentDomain.BaseDirectory;
                _directory = String.Concat(_directory, "/assets/");
                using (StreamReader r = new StreamReader(String.Concat(_directory, "taiwan_districts.json")))
                {
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<CityDistricts>>(json);
                }
            }
        }

        /// <summary>
        /// 取回台灣地區城市行政區列表，之後可考慮寫入 Redis 減少 IO
        /// </summary>
        /// <returns></returns>
        public static List<CityDistricts> GetCityDistricts()
        {
            return CityDistricts;
        }

        /// <summary>
        /// 取回指定城市行政區
        /// </summary>
        /// <param name="City"></param>
        /// <returns></returns>
        public static List<District> GetDistricts(string City)
        {
            return GetCityDistricts().Find(x => x.Name == City).Districts;
        }

        /// <summary>
        /// 取回指定城市指定行政區名稱
        /// </summary>
        /// <param name="City">城市</param>
        /// <param name="Zip">行政區編碼</param>
        /// <returns></returns>
        public static string GetDistrictName(string City, string Zip)
        {
            return GetDistricts(City).Find(d => d.Zip == Zip).Name;
        }

        /// <summary>
        /// 取回指定城市指定行政區名稱
        /// </summary>
        /// <param name="City">城市</param>
        /// <param name="Zip">行政區編碼</param>
        /// <returns></returns>
        public static string GetDistrictZip(string City, string Name)
        {
            if (String.IsNullOrEmpty(City) || String.IsNullOrEmpty(Name))
            {
                return String.Empty;
            }

            return GetDistricts(City).Find(d => d.Name == Name).Zip;
        }

        public static List<PestNotice> PreventionInfoList()
        {
            string _directory = AppDomain.CurrentDomain.BaseDirectory;
            _directory = String.Concat(_directory, "/assets/other_api/");
            using (StreamReader r = new StreamReader(String.Concat(_directory, "pest_notice.json")))
            {
                List<PestNotice> List = new List<PestNotice>();
                JArray data = JArray.Parse(r.ReadToEnd());
                try
                {
                    foreach (var d in data)
                    {
                        JObject obj = JObject.Parse(d.ToString());
                        List.Add(new PestNotice()
                        {
                            Title = obj["Title"].ToString(),
                            Content = obj["Content"].ToString(),
                            SuggestedMedication = obj["SuggestedMedication"].ToString(),
                            ProvideCiry = obj["ProvideCiry"].ToString(),
                            PlantType = obj["PlantType"].ToString(),
                            CreatedAt = obj["CreatedAt"].ToString(),
                            ProvideUnit = obj["ProvideUnit"].ToString(),
                        }); ;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return List;
            }
        }
    }
}