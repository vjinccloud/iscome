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
    public class EventSignJob : IJob
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
            var openDateSet = 60;
            var signDateSet = 20;
            try
            {
                var _res = from openTimes in Service_activityPlanOpenTIme.GetList(x => x.Date < DateTime.Today.AddDays((-1)*Math.Abs(openDateSet)))
                           join sign in Service_activitySign.GetList(x => x.SignInDate < DateTime.Today.AddDays((-1) * Math.Abs(signDateSet)))
                           on openTimes.ID equals sign.ActivityOpenTimesId
                           select sign;

                Service_activitySign.DeleteAll(_res.Select(x => x.Id).ToList());
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e, "JobError");
            }
        }
    }
}