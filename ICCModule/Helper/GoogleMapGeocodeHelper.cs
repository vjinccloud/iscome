using ICCModule.Models.Map.Google;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.Helper
{
    public class GoogleMapGeocodeHelper
    {
        private HttpClient client;

        /// <summary>
        /// 基本請求網址
        /// </summary>
        private string RequestGeocodeUrl = @"https://maps.googleapis.com/maps/api/geocode";

        /// <summary>
        /// API Key
        /// </summary>
        private string _ApiKey { get; set; }

        public GoogleMapGeocodeHelper()
        {
            client = new HttpClient();
            _ApiKey = AppSettingHelper.GetAppSetting("MapApiKey");
        }

        /// <summary>
        /// 取回 Request Uri
        /// </summary>
        /// <param name="Address">地址</param>
        /// <param name="type">json, xml</param>
        /// <returns></returns>
        public string GetGeocodeAddressRequestUri(string Address, string type = "json")
        {
            return $"{RequestGeocodeUrl}/{type}?address={Address}&key={_ApiKey}";
        }

        /// <summary>
        /// 設定 Api Key
        /// </summary>
        /// <param name="ApiKey"></param>
        public void SetApiKey(string ApiKey)
        {
            _ApiKey = ApiKey;
        }

        /// <summary>
        /// 發起 任務請求 Geocode 單次
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public async Task<GeocodeApi> GeocodeAddress(string Address)
        {
            var response = await client.GetAsync(GetGeocodeAddressRequestUri(Address))
                .ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<GeocodeApi>(await response.Content.ReadAsStringAsync());

            return result;
        }
        
        /// <summary>
        /// 發起 任務請求 Geocode 單次
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public GeocodeApi GeocodeAddressNa(string Address)
        {
            var response = client.GetStringAsync(GetGeocodeAddressRequestUri(Address));
            var result = JsonConvert.DeserializeObject<GeocodeApi>(response.Result);

            return result;
        }

        /// <summary>
        /// 批次取回 Geocode 結果
        /// </summary>
        /// <param name="AddressList"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, GeocodeApi>> GeocodeAddressInParallel(IDictionary<string, string> AddressList)
        {
            var result = new Dictionary<string, GeocodeApi>();
            int batchSize = 100;
            int numberOfBatches = (int)Math.Ceiling((double)AddressList.Count() / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var currentItems = AddressList.Skip(i * batchSize).Take(batchSize);
                var tasks = currentItems.Select(s => GeocodeAddress(s.Value));
                var geocodeApis = await Task.WhenAll(tasks);
                int j = 0;
                foreach (var s in currentItems)
                {
                    result.Add(s.Key, geocodeApis.ElementAt(j));
                    j++;
                }
            }

            return result;
        }

    }
}
