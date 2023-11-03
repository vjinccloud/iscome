using Website.Models.ExternalAPI;
using Website.ViewModel.PreventionInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ICCModule.Helper;
using IscomG2C.Utility;

namespace Website.Controllers
{
    public class PreventionInfoController : Controller
    {
        /// <summary>
        /// 疫情預警畫面
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail()
        {
            return View();
        }

        /// <summary>
        /// 疫情預警資料
        /// </summary>
        /// <param name="KeyWord">關鍵字</param>
        /// <returns></returns>
        public async Task<ActionResult> List()
        {
            List<PestNotice> List = new List<PestNotice>();
            JArray data = new JArray();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                if (String.IsNullOrEmpty(AppSettingHelper.GetAppSetting("BackendHostUrl")))
                {
                    throw new Exception("BackendHostUrl Not Set.");
                }
                string url = String.Concat(AppSettingHelper.GetAppSetting("BackendHostUrl"), "/Other/PreventionInfoDetail?last=0");
                var res = await client.GetAsync(new Uri(url));
                data = JArray.Parse(await res.Content.ReadAsStringAsync());
                List = data.ToObject<List<PestNotice>>();
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e.Message);
                var res = await client.GetAsync(new Uri(PestNoticeConfig.PestNoticeDataUrl));
                data = JArray.Parse(await res.Content.ReadAsStringAsync());
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
                    });
                }
            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        //作物防災告警分布地圖
        public ActionResult CropMap()
        {
            return View();
        }
    }
}