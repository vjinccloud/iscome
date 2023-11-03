using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ICCModule.ViewModel;
using ICCModule.Helper;
using Newtonsoft.Json;
using IscomG2C.Utility;
using System.Windows.Interop;

namespace ICCModule.EntityService.Service
{
    public class Service_LineBroadcast
    {
        public static string _ChannelAccessToken = AppSettingHelper.GetAppSetting("_ChannelAccessToken");
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<LineBroadcast> GetList(Expression<Func<LineBroadcast, bool>> filter = null)
        {
            return RepositoryUtility.GetList<LineBroadcast>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(LineBroadcast data)
        {
            data.CreateDate = DateTime.Now;
            return RepositoryUtility.Insert<LineBroadcast>(data);
        }
        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void SendMessage(LinePushMessageModel req)
        {
            var _url = "https://api.line.me/v2/bot/message/multicast";
            var _http = new HttpHelper(_url);
            var responseStr = _http.Post(req, _url, _ChannelAccessToken);
        }
        /// <summary>
        /// 回覆訊息
        /// </summary>
        /// <param name="req"></param>
        public static void SendBroadcast(List<SendMessage> req)
        {
            var obj = new
            {
                messages = req
            };
            var _url = "https://api.line.me/v2/bot/message/broadcast";
            var _http = new HttpHelper(_url);
            var responseStr = _http.Post(obj, _url, _ChannelAccessToken);
            ErrorLog.SaveErrorLog(responseStr+ JsonConvert.SerializeObject(req), "LineBroadCast");
        }
    }
}
