using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using IscomG2C.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;

namespace InformationSystem.Jobs
{
    /// <summary>
    /// 氣候紀錄
    /// 打上DisallowConcurrentExecution標簽讓Job進行單執行緒跑，避免沒跑完時的重復執行，
    /// </summary>
    [DisallowConcurrentExecution]
    public class DailyWeatherJob : IJob
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
                var data = new WeatherResponse();
                try
                {
                    var sDate = DateTime.Now.Date.AddDays(-1).AddHours(8);
                    var eDate = DateTime.Now.Date.AddDays(-1).AddHours(17);
                    if (!Service_DailyWeather.GetList(x => x.DataDate == sDate.Date).Any())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var _url = $"https://data.coa.gov.tw/api/v1/AutoWeatherStationType/?Start_time={sDate.ToString("yyyy-MM-dd HH:mm")}&End_time={eDate.ToString("yyyy-MM-dd HH:mm")}&CITY_SN=5";

                        var res = await client.GetAsync(new Uri(_url));
                        var resContent = await res.Content.ReadAsStringAsync();
                        data = JsonConvert.DeserializeObject<WeatherResponse>(resContent);
                        decimal _d = 0;
                        data.Data = data.Data.Where(x => decimal.TryParse(x.TEMP, out _d) && decimal.TryParse(x.H_24R, out _d)).ToList();
                        if (data.Data.Any())
                        {
                            var dataGroup = data.Data.GroupBy(x => new { x.CITY, x.CITY_SN, x.TOWN_SN, x.TOWN, TIME = x.TIME.Date }).Select(x => new DailyWeather
                            {
                                DataDate = x.Key.TIME,
                                CitySN = x.Key.CITY_SN,
                                CityName = x.Key.CITY,
                                DistrictSN = x.Key.TOWN_SN,
                                DistrictName = x.Key.TOWN,
                                Temp = x.Sum(o => o.TEMP.ToDecimal32()) / x.Count(),
                                Rainfall = x.Sum(o => o.H_24R.ToDecimal32()) / x.Count(),
                            });
                            foreach (var item in dataGroup)
                            {
                                Service_DailyWeather.Insert(item);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ErrorLog.SaveErrorLog(e, "JobError");
                }
            }
        }
    }
    public class WeatherResponse
    {
        public WeatherResponse()
        {
            Data = new List<DailyWeatherModel>();
        }
        public string RS { get; set; }
        public List<DailyWeatherModel> Data { get; set; }
        public bool Next { get; set; }
    }
    public class DailyWeatherModel
    {
        /// <summary>
        /// 測站名稱
        /// </summary>
        public string Station_name { get; set; }
        /// <summary>
        /// 測站序號
        /// </summary>
        public string Station_ID { get; set; }
        /// <summary>
        /// 資料觀測時間
        /// </summary>
        public DateTime TIME { get; set; }
        /// <summary>
        /// 測站位置緯度(TWD67)
        /// </summary>
        public decimal Station_Latitude { get; set; }
        /// <summary>
        /// 測站位置經度(TWD67)
        /// </summary>
        public decimal Station_Longitude { get; set; }
        /// <summary>
        /// 溫度
        /// </summary>
        public string TEMP { get; set; }
        /// <summary>
        /// 日累積雨量
        /// </summary>
        public string H_24R { get; set; }
        /// <summary>
        /// 縣市
        /// </summary>
        public string CITY { get; set; }
        /// <summary>
        /// 縣市編號
        /// </summary>
        public string CITY_SN { get; set; }
        /// <summary>
        /// 鄉鎮
        /// </summary>
        public string TOWN { get; set; }
        /// <summary>
        /// 鄉鎮編號
        /// </summary>
        public string TOWN_SN { get; set; }
    }
}