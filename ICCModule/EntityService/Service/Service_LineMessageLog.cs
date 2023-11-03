using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository;
using ICCModule.Repository.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ICCModule.ViewModel;
using ICCModule.Helper;
using Newtonsoft.Json;
using IscomG2C.Utility;
using ICCModule.Models;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICCModule.EntityService.Service
{
    public class Service_LineMessageLog
    {
        public static string site_base_url = "https://www.christophe.tw/";
        public static string _ChannelAccessToken = "no6PN1cBowy3LpA8naDmkLvOgwKHvcwpWQBGn1vIL4wOJUm1Q2h5TC+BSQQWcSKbhT6gE5/SDgh2FVSvj70ne3LAN3eOCFpti3pJSTVfbwtsigC11zKrrDhFou3uf+ZiK2zrHiw+Th9rjFmxLT74HQdB04t89/1O/w1cDnyilFU=";
        /// <summary>
        /// 讀取清單
        /// </summary>
        /// <returns></returns>
        public static List<LineMessageLog> GetList(Expression<Func<LineMessageLog, bool>> filter = null)
        {
            return RepositoryUtility.GetList<LineMessageLog>(filter);
        }

        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static BaseResult Insert(LineMessageLog data)
        {
            return RepositoryUtility.Insert<LineMessageLog>(data);
        }
        /// <summary>
        /// 寫入單筆資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void ReceiveMsg(LineModel req)
        {
            var errorSide = "";
            try
            {
                foreach (var eventItem in req.events)
                {
                    if (eventItem.type == "message" && eventItem.message.type == "text")
                    {
                        errorSide = "1";
                        //綁定Line與姓名
                        if (!string.IsNullOrEmpty(eventItem.message.text) && eventItem.message.text.StartsWith("#帳號綁定#"))
                        {
                            errorSide = "2";
                            var _checkCode = eventItem.message.text.Substring(6);
                            if (!string.IsNullOrEmpty(_checkCode))
                            {
                                var findMember = Service_MemberInfo.GetList(x => x.LineBindCode == _checkCode && x.LineBindLimit >= DateTime.Now).FirstOrDefault();
                                if (findMember != null)
                                {

                                    errorSide = "3";
                                    var bindRes = Service_MemberInfo.BindLineMessage(findMember.ID, eventItem.source.userId);
                                    if (bindRes.result)
                                    {
                                        errorSide = "4";
                                        ReplyMessage(new LineReplyMessageModel()
                                        {
                                            replyToken = eventItem.replyToken,
                                            messages = new List<SendMessage>(){ new SendMessage()
                                            {
                                                type = "text",
                                                text = "綁定成功"
                                            }},
                                        });
                                        errorSide = "5";
                                    }
                                    else
                                    {
                                        errorSide = "6";
                                        ReplyMessage(new LineReplyMessageModel()
                                        {
                                            replyToken = eventItem.replyToken,
                                            messages = new List<SendMessage>(){ new SendMessage()
                                            {
                                                type = "text",
                                                text = "綁定失敗：" + bindRes.Msg
                                            }},
                                        });
                                        errorSide = "7";
                                    }
                                }
                                else
                                {
                                    errorSide = "8";
                                    ReplyMessage(new LineReplyMessageModel()
                                    {
                                        replyToken = eventItem.replyToken,
                                        messages = new List<SendMessage>(){ new SendMessage()
                                        {
                                            type = "text",
                                            text = "無此驗證碼或驗證碼過期"
                                        }},
                                    });
                                    errorSide = "9";
                                }
                            }
                        }
                        //紀錄文字訊息，其餘不紀錄
                        else if (!string.IsNullOrEmpty(eventItem.message.text))
                        {
                            errorSide = "10";
                            var name = "";
                            int? id = null;
                            var lineUser = Service_MemberInfo.GetList(x => x.LineMessageId == eventItem.source.userId).FirstOrDefault();
                            if (lineUser != null)
                            {
                                name = lineUser.Name;
                                id = (int)lineUser.ID;
                            }
                            else
                            {
                                errorSide = "11";
                                var _url = $"https://api.line.me/v2/bot/profile/{eventItem.source.userId}";
                                var _http = new HttpHelper(_url);
                                var responseStr = _http.Get(null, _url, _ChannelAccessToken);
                                errorSide = "12";
                                var lineProfile = JsonConvert.DeserializeObject<LineUserProfileModel>(responseStr.ToString());
                                errorSide = "13";
                                if (lineProfile != null)
                                {
                                    errorSide = "14";
                                    name = lineProfile.displayName;
                                }
                            }
                            errorSide = "15";
                            var userMsg = new LineMessageLog()
                            {
                                MemberId = id,
                                Name = name,
                                LineUserId = eventItem.source.userId,
                                UserMessage = eventItem.message.text,
                                CreateDate = DateTime.Now,
                            };
                            Insert(userMsg);
                            errorSide = "16";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.SaveErrorLog(ex,"LineError", errorSide + JsonConvert.SerializeObject(req));
            }
        }
        /// <summary>
        /// 回覆訊息
        /// </summary>
        /// <param name="req"></param>
        public static void ReplyMessage(LineReplyMessageModel req)
        {
            var _url = "https://api.line.me/v2/bot/message/reply";
            var _http = new HttpHelper(_url);
            var responseStr = _http.Post(req, _url, _ChannelAccessToken);
        }




    }
}
