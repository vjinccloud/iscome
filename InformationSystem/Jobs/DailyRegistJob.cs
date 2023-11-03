using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
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
    public class DailyRegistJob : IJob
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
            var setDate = DateTime.Now.Date.AddHours(16);
            try
            {
                var allMember = Service_MemberInfo.GetList(x => setDate.AddDays(-1) <= x.CreatedAt && x.CreatedAt < setDate);
                var memberIdentity = Service_defCode.GetList("UserIdentify");

                var i = 0;
                var registData = allMember.Select(x => new
                {
                    序號 = (i += 1),
                    姓名 = x.Name ?? "",
                    行政區 = x.District ?? "",
                    手機號碼 = x.Mobile ?? "",
                    Email = x.Email ?? "",
                    角色別 = memberIdentity.FirstOrDefault(o => o.Code == x.Identify)?.Name ?? "",
                    註冊來源 = x.RegistFrom,
                }).ToList();

                if (registData.Any())
                {
                    var _contentType = new ContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    _contentType.Name = $"每日會員註冊數報表_{DateTime.Now.ToString("yyyyMMdd")}.xlsx";
                    var _ms = ExcelHelper.RenderDataTableToExcel(ExcelHelper.ConvertToDataTable(registData));

                    var _htmlComtent = new System.Text.StringBuilder();
                    _htmlComtent.Append(@"<!DOCTYPE html>
                                          <html>
                                              <head>
                                                  <meta http-equiv='content-type' content='text/html; charset=utf-8' />
                                                  <meta charset='utf-8' />
                                              </head>
                                              <body>
                                                  系統管理員您好，
                                                  <br />
                                                  <div>附件為今日會員註冊數報表。</div>
                                                  <br />
                                                  <div>***本郵件為系統自動發送，請勿直接回信***</div>
                                              </body>
                                          </html>");
                    var allSystemManager = Service_sysUserInfo.GetList(x => x.RoleID == "R02").Where(x => !string.IsNullOrEmpty(x.EmailAddress)).ToList();
                    string refErrMsg = "";
                    var sendMail = MailHelper.SendMail(ref refErrMsg, allSystemManager.Select(x => x.EmailAddress).ToList(), $"高雄市政府農業局線上植物防疫平台－{DateTime.Now.Year - 1911} 年 {DateTime.Now.Month} 月 {DateTime.Now.Day} 日會員註冊數通知", _htmlComtent.ToString(), _ms, _contentType);
                }
            }
            catch (Exception e)
            {
                ErrorLog.SaveErrorLog(e, "JobError");
            }
        }

        public void SendReistMail()
        {

        }
    }
}