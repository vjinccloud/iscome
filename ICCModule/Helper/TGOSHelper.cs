using ICCModule.Models.Map.TGOS;
using ICCModule.ViewModel;
using IscomG2C.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ICCModule.Helper
{
    public class TGOSHelper
    {
        private HttpClient client;

        /// <summary>
        /// 基本請求網址
        /// </summary>
        private string RequestBasicUrl = @"https://addr.tgos.tw/addrws/v30";

        /// <summary>
        /// APP ID
        /// </summary>
        private string _AppId { get; set; }

        // <summary>
        /// API Key
        /// </summary>
        private string _ApiKey { get; set; }

        public TGOSHelper()
        {
            client = new HttpClient();
        }

        /// <summary>
        /// 取回 Request Uri
        /// </summary>
        /// <param name="Address">地址</param>
        /// <param name="type">json, xml</param>
        /// <returns></returns>
        public string GetGeocodeAddressRequestUri()
        {
            _AppId = AppSettingHelper.GetAppSetting("TGOS_Geocoding_APPID");
            _ApiKey = AppSettingHelper.GetAppSetting("TGOS_Geocoding_APIKEY");
            return $"{RequestBasicUrl}//GeoQueryAddr.asmx";
        }

        /// <summary>
        /// 取回 Request Uri
        /// </summary>
        /// <param name="Address">地址</param>
        /// <param name="type">json, xml</param>
        /// <returns></returns>
        public string GetQueryAddressRequestUri()
        {
            _AppId = AppSettingHelper.GetAppSetting("TGOS_QueryAddr_APPID");
            _ApiKey = AppSettingHelper.GetAppSetting("TGOS_QueryAddr_APIKEY");
            return $"{RequestBasicUrl}/QueryAddr.asmx/QueryAddr";
        }

        /// <summary>
        /// 設定 App Id
        /// </summary>
        /// <param name="AppId"></param>
        public void SetAppId(string AppId)
        {
            _AppId = AppId;
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
        public async Task<QueryAddr> QueryAddress(string Address)
        {
            var _url = GetQueryAddressRequestUri();
            QueryAddrRequest queryAddrRequest = new QueryAddrRequest();
            queryAddrRequest.oAPPId = _AppId;
            queryAddrRequest.oAPIKey = _ApiKey;
            queryAddrRequest.oAddress = Address;
            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
            foreach (var property in queryAddrRequest.GetType().GetProperties())
            {
                keyValuePairs.Add(new KeyValuePair<string, string>(property.Name, property.GetValue(queryAddrRequest).ToString()));
            }

            var content = new FormUrlEncodedContent(keyValuePairs);

            var response = await client.PostAsync(_url, content)
                .ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<QueryAddr>(await response.Content.ReadAsStringAsync());

            return result;
        }

        /// <summary>
        /// 發起 任務請求 Geocode 單次
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        public string QueryAddressNa(string aAddress,string oSRS = "EPSG:3826")
        {
            var res = "";
            #region 參數
            _AppId = AppSettingHelper.GetAppSetting("TGOS_QueryAddr_APPID");
            _ApiKey = AppSettingHelper.GetAppSetting("TGOS_QueryAddr_APIKEY");
            //坐標系統(SRS)EPSG:4326(WGS84)國際通用, EPSG:3825 (TWD97TM119) 澎湖及金馬適用,EPSG:3826 (TWD97TM121) 台灣地區適用,EPSG:3827 (TWD67TM119) 澎湖及金馬適用,EPSG:3828 (TWD67TM121) 台灣地區適用
            string aSRS = oSRS;
            //0:最近門牌號機制,1:單雙號機制,2:[最近門牌號機制]+[單雙號機制]
            int aFuzzyType = 0;
            //回傳的資料格式，允許傳入的代碼為：JSON、XML
            string aResultDataType = "JSON";
            //模糊比對回傳門牌號的許可誤差範圍，輸入格式為正整數，如輸入 0 則代表不限制誤差範圍
            int aFuzzyBuffer = 0;
            //是否只進行完全比對，允許傳入的值為：true、false，如輸入 true ，模糊比對機制將不被使用
            string aIsOnlyFullMatch = "false";
            //是否支援舊門牌的查詢，允許傳入的值為：true、false，如輸入 true ，查詢時範圍包含舊門牌
            string aIsSupportPast = "true";
            //是否顯示地址的統計區相關資訊，允許傳入的值為：true、false
            string aIsShowCodeBase = "true";
            //是否鎖定縣市，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [縣市] 要與所輸入的門牌地址中的 [縣市] 完全相同
            string aIsLockCounty = "false";
            //是否鎖定鄉鎮市區，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [鄉鎮市區] 要與所輸入的門牌地址中的 [鄉鎮市區] 完全相同
            string aIsLockTown = "false";
            //是否鎖定村里，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [村里] 要與所輸入的門牌地址中的 [村里] 完全相同
            string aIsLockVillage = "false";
            //是否鎖定路段，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [路段] 要與所輸入的門牌地址中的 [路段] 完全相同
            string aIsLockRoadSection = "false";
            //是否鎖定巷，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [巷] 要與所輸入的門牌地址中的 [巷] 完全相同
            string aIsLockLane = "false";
            //是否鎖定弄，允許傳入的值為：true、false，如輸入 true ，則代表查詢結果中的 [弄] 要與所輸入的門牌地址中的 [弄] 完全相同
            string aIsLockAlley = "false";
            //是否鎖定地區，允許傳入的值為：true、fals，如輸入 true ，則代表查詢結果中的 [地區] 要與所輸入的門牌地址中的 [地區] 完全相同
            string aIsLockArea = "false";
            //號之、之號是否視為相同，允許傳入的值為：true、false
            string aIsSameNumber_SubNumber = "true";
            //TGOS_Query_Addr_Url WebService網址
            string aCanIgnoreVillage = "true";
            //找不時是否可忽略村里，允許傳入的值為：true、false
            string aCanIgnoreNeighborhood = "true";
            //找不時是否可忽略鄰，允許傳入的值為：true、false
            int aReturnMaxCount = 0;
            //如為多筆時，限制回傳最大筆數，輸入格式為正整數，如輸入 0 則代表不限制回傳筆數

            //使用string.Format給予參數設定
            string param = @"oAPPId={0}&oAPIKey={1}&oAddress={2}&oSRS={3}&oFuzzyType={4}
                         &oResultDataType={5}&oFuzzyBuffer={6}&oIsOnlyFullMatch={7}&oIsSupportPast={8}&oIsShowCodeBase={9}
                         &oIsLockCounty={10}&oIsLockTown={11}&oIsLockVillage={12}&oIsLockRoadSection={13}
                         &oIsLockLane={14}&oIsLockAlley={15}&oIsLockArea={16}&oIsSameNumber_SubNumber={17}
                         &oCanIgnoreVillage={18}&oCanIgnoreNeighborhood={19}&oReturnMaxCount={20}";

            //給予需要的參數
            param = string.Format(param,

                  //應用程式識別碼(APPId) 實由使用者向TGOS申請
                  _AppId,
                  //應用程式介接驗證碼(APIKey)由使用者向TGOS申請
                  _ApiKey,
                  aAddress,
                  aSRS,
                  aFuzzyType,
                  aResultDataType,
                  aFuzzyBuffer,
                  aIsOnlyFullMatch,
                  aIsSupportPast,
                  aIsShowCodeBase,
                  aIsLockCounty,
                  aIsLockTown,
                  aIsLockVillage,
                  aIsLockRoadSection,
                  aIsLockLane,
                  aIsLockAlley,
                  aIsLockArea,
                  aIsSameNumber_SubNumber,
                  aCanIgnoreVillage,
                  aCanIgnoreNeighborhood,
                  aReturnMaxCount
              );
            #endregion
            var oUrl = "https://addr.tgos.tw/addrws/v40/QueryAddr.asmx/QueryAddr?";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(oUrl);
            param = param.Replace("+", "%2B");
            byte[] PostData = Encoding.UTF8.GetBytes(param); //參數以UTF8格式轉成Byte[]之後才能夠寫入資料流       
            req.Method = "POST"; //取得或設定要求的方法
            //取得或設定 Content-type HTTP 標頭的值。 輸入資料被編碼為名稱/值對 
            //當Method為get時候，瀏覽器用x-www-form-urlencoded的編碼方式把form數據轉換成一個字串（name1=value1&name2=value2...）
            //然後把這個字串append到url後面，用?分割，加載這個新的url。
            //當Method為post時候，瀏覽器把form數據封裝到http body中，
            //然後發送到server。如果沒有type=file的控件，用默認的application/x-www-form-urlencoded就可以了。
            req.ContentType = "application/x-www-form-urlencoded";

            //取得或設定 Content-length HTTP 標頭(資訊)
            req.ContentLength = PostData.Length;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
            //client對sever請求提出取得資料流 
            //using清理非托管不受GC控制的資源
            using (Stream reqStream = req.GetRequestStream())
            {
                //把參數資料寫入資料流
                reqStream.Write(PostData, 0, PostData.Length);
            }
            //取server端資料使用 request.GetResponse() 方法時，
            //HttpWebRequest即會將資料傳送到指定的網址做處理，
            //處理完成後會依照Server的動作看是回傳資料或是重新指向他頁
            using (WebResponse wr = req.GetResponse())
            {

                //取得sever端資料流放入myStream
                //一般回傳'所需要的資料要用 HttpWebResponse類別 來接收Response的內容，
                //取得Response後在使用 StreamReader類別 來讀取 response.GetResponseStream() 的資料流內容
                using (Stream myStream = wr.GetResponseStream())
                {
                    //new一個新的資料讀取器,從myStream取得資料
                    //清理非托管不受GC控制的資源
                    using (StreamReader myStreamReader = new StreamReader(myStream))
                    {

                        //對讀取出來的JSON格式資料進行加密
                        string Json = HttpUtility.HtmlEncode(myStreamReader.ReadToEnd());
                        //對讀取出來的JSON格式資料進行解密
                        Json = HttpContext.Current.Server.HtmlDecode(Json);
                        //XmlDocument讀取XML
                        XmlDocument aXmlDocument = new XmlDocument();
                        aXmlDocument.XmlResolver = null;
                        aXmlDocument.LoadXml(Json);

                        //使用 StreamReader 來從標準文字檔讀取資料。
                        //HttpUtility.HtmlEncode()將字串轉換為 HTML 編碼的字串。
                        var geocoding = JsonConvert.DeserializeObject<TgosModel>(aXmlDocument.DocumentElement.InnerText);

                        if (geocoding.AddressList.Any())
                        {
                            res = geocoding.AddressList.FirstOrDefault().X + "," + geocoding.AddressList.FirstOrDefault().Y;
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 批次取回 Geocode 結果
        /// </summary>
        /// <param name="AddressList"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, QueryAddr>> GeocodeAddressInParallel(IDictionary<string, string> AddressList)
        {
            var result = new Dictionary<string, QueryAddr>();
            int batchSize = 100;
            int numberOfBatches = (int)Math.Ceiling((double)AddressList.Count() / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var currentItems = AddressList.Skip(i * batchSize).Take(batchSize);
                var tasks = currentItems.Select(s => QueryAddress(s.Value));
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
