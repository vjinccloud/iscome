using IscomG2C.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ICCModule.Helper;
using InformationSystem.Helper;
using IscomG2C.Utility.Library;
using InformationSystem.Jobs;

namespace InformationSystem
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalFilters.Filters.Add(new MaintenanceActionFilter());
            
            // Scheduler
            QuartzFactory.AddJob<PestNoticeJob>("PestNoticeJob", PestNoticeConfig.CronTime);
            QuartzFactory.AddJob<PesticideInfoJob>("PesticideInfoJob", PesticideInfoConfig.CronTime);
            QuartzFactory.AddJob<PesticideNameJob>("PesticideNameJob", PesticideNameConfig.CronTime);
            QuartzFactory.AddJob<DailyWeatherJob>("DailyWeatherJob", "0 0 0 * * ? *");
            QuartzFactory.AddJob<EventSignJob>("EventSignJob", "0 0 1 * * ? *");
            QuartzFactory.AddJob<DailyRegistJob>("DailyRegistJob", "0 0 0 * * ? *");
            QuartzFactory.AddJob<SolarTermPushJob>("SolarTermPushJob", SolarTermPushConfig.CronTime);
        }

        /// <summary>定義應用程式錯誤處理
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            bool is404 = false;
            //取得錯誤訊息
            Exception ex = Server.GetLastError();
            if (ex is HttpException)
            {
                //如果是404錯誤(找不到網頁或者相關圖片)
                if (((HttpException)(ex)).GetHttpCode() == 404)
                {
                    is404 = true;
                }
            }

            //如果是404錯誤
            if (is404)
            {
                //取得路徑與參數
                string path = Request.Path + "?" + Request.QueryString;
                //儲存錯誤紀錄
                ErrorLog.SaveErrorLog(ex, "404Error", "path:" + path);
            }
            else
            {   //非404錯誤 則為一般的應用程式錯誤

                //取得路徑與參數
                string path = Request.Path + "?" + Request.QueryString;
                //儲存錯誤紀錄
                ErrorLog.SaveErrorLog(ex, "ApplicationError", "path:" + path);
                // Code that runs when an unhandled error occurs
                //Server.Transfer("~/GenericError.html");
            }
        }
        
    }
}
