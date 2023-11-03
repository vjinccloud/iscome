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
using ICCModule.EntityService.Service;
using ICCModule.ViewModel;
using InformationSystem.ViewModel;
using ICCModule.Helper;

namespace InformationSystem.Jobs
{
    /// <summary>
    /// 農委會資料開放平台-植物疫情預警
    /// 打上DisallowConcurrentExecution標簽讓Job進行單執行緒跑，避免沒跑完時的重復執行，
    /// </summary>
    [DisallowConcurrentExecution]
    public class SolarTermPushJob : IJob
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
            try
            {
                var getPush = Service_SolarTermsPush.GetList(x => !x.IsPushed && x.PushDate == DateTime.Today.Date && x.IsNeedPush);
                foreach(var req in getPush)
                {
                    var msg = new List<SendMessage>();
                    foreach (var item in JsonConvert.DeserializeObject<List<PushContentTab>>(req.PushContents))
                    {
                        if (item.tabId == 1) msg.Add(new SendMessage() { type = "text", text = item.content });
                        else if (item.tabId == 2) msg.Add(new SendMessage() { type = "image", originalContentUrl = $"{AppSettingHelper.GetAppSetting("Backend_HostName")}{item.path}", previewImageUrl = $"{AppSettingHelper.GetAppSetting("Backend_HostName")}{item.path}" });
                    }
                    Service_LineBroadcast.SendBroadcast(msg);

                    Service_SolarTermsPush.UpdatePushed(req.Id);
                }
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e, "LineBroadCast");
            }
        }
    }

    public static class SolarTermPushConfig
    {
        /// <summary>
        /// 每日早上 6點
        /// </summary>
        public static string CronTime = "0 0 6 * * ? *";
    }
}