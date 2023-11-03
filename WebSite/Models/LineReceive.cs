using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Repository.Class;
using ICCModule.ViewModel;
using ICCModule.EntityService.Service;
using ICCModule.Entity.Tables;
using ICCModule.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ICCModule.Helper;
using IscomG2C.Utility;

namespace Website.Models
{
    public class LineReceive
    {
        public static string _ChannelAccessToken = "";
        public static string base_url = "";
        public static string base_admin_url = "";
        public static string strUnbind = "您尚未綁定Line帳號，請您點選會員註冊／綁定，完成綁定流程後，才能進行植物醫師線上掛號。";
        public static string backendDirector = ConfigurationManager.AppSettings["BackendDirectory"];


        public static void ReceiveMsg(LineModel req)
        {
            _ChannelAccessToken = ConfigurationManager.AppSettings["_ChannelAccessToken"];
            base_url = ConfigurationManager.AppSettings["base_url"];
            base_admin_url = ConfigurationManager.AppSettings["base_admin_url"];

            //line api (建立選單時選單需要的參數)
            string friendUrl = ConfigurationManager.AppSettings["friendUrl"];
            string diseaseNewsUrl = ConfigurationManager.AppSettings["diseaseNewsUrl"];
            string findJobUrl = ConfigurationManager.AppSettings["findJobUrl"];
            string redirectLiff = ConfigurationManager.AppSettings["redirectLiff"];
            //設定
            LineApi.Setting(_ChannelAccessToken , friendUrl, diseaseNewsUrl , findJobUrl, redirectLiff);

            try
            {
                foreach (var eventItem in req.events)
                {
                    string userId = eventItem.source.userId;
                    string token = eventItem.replyToken;
                    bool isBinded = IsBinded(userId);
                    try
                    {
                        LineApi.SetUserMenu(userId, isBinded);
                    }
                    catch { 
                    
                    }
                    //=========================
                    //=====1.處理字串訊息======
                    //=========================
                    if (eventItem.type == "message" && eventItem.message.type == "text")
                    {
                        HandleText(isBinded, token, userId, eventItem.message.text);
                    }

                    //============================
                    //=====2.處理位置訊息=========
                    //============================
                    else if (eventItem.type == "message" && eventItem.message.type == "location")
                    {
                        HandelLocation(isBinded, token, Convert.ToDouble(eventItem.message.latitude), Convert.ToDouble(eventItem.message.longitude));
                    }

                    //============================
                    //=====3.處理圖片訊息=========
                    //============================
                    else if (eventItem.type == "message" && eventItem.message.type == "image")
                    {
                        //string img = eventItem.message.imageSet.id;
                        string LastCommand = GetLastCommand(userId);
                        if (LastCommand == "預約植醫-上傳照片")
                        {
                            if (isBinded)
                            {
                                //存檔
                                string messageId = eventItem.message.id;
                                string saveLocation = backendDirector + $"/UploadedFiles/LineUpload//CropSymptomsFiles/{userId}_{messageId}.jpg";
                                FileHelper.CreateFolder(backendDirector + $"/UploadedFiles/LineUpload/CropSymptomsFiles");
                                LineApi.SaveLineContent(messageId, saveLocation);

                                //單張
                                if (eventItem.message.imageSet == null)
                                {
                                    UpdateLastCommandDataImg(userId, "預約植醫-上傳照片", "msgId", messageId);
                                    vConfirmUpload(token , userId);
                                }
                                //多張
                                else
                                {
                                    int index = eventItem.message.imageSet.index;
                                    int total = eventItem.message.imageSet.total;

                                    UpdateLastCommandDataImg(userId, "預約植醫-上傳照片", "msgId", messageId);
                                    //每次多張上傳的最後一張,再回覆
                                    if (index == total)
                                    {
                                        vConfirmUpload(token , userId);
                                    }
                                }
                            }
                            else
                            {
                                //LineApi.ReplyText(token, strUnbind);
                                vNotLogin(token ,userId, "才能上傳照片");
                            }
                        }
                    }

                    //================================
                    //=====4.處理accountLink 訊息=====
                    //================================
                    else if (eventItem.type == "accountLink")
                    {
                        HandelAccountLine(isBinded, token, userId, eventItem.Link.nonce, eventItem.Link.result);
                    }

                    //============================
                    //=====5.處理postback訊息=====
                    //============================
                    else if (eventItem.type == "postback")
                    {
                        HandlePostback(isBinded, token, userId, eventItem.postback.data);
                    }
                }
            }
            catch (Exception ex)
            {
                LineApi.SaveTextFile(ex.ToString());
            }
        }

        //4.處理accountLink 訊息
        private static void HandelAccountLine(bool isBinded, string token, string userId, string nonce, string result)
        {
            if (result == "ok")
            {
                var res = BiindLineID(userId, nonce);
                if (res != "")
                {
                    LineApi.ReplyText(token, "綁定失敗2,userid=" + userId + ";nonce=" + nonce);
                }
                else
                {
                    LineApi.SetUserMenu(userId, true);
                    LineApi.ReplyText(token, "帳號已綁定");
                }
            }
            else
            {
                LineApi.ReplyText(token, "綁定失敗1,userid=" + userId + ";nonce=" + nonce);
            }
        }

        //1.處理字串訊息
        private static void HandleText(bool isBinded, string token, string userId, string commandText)
        {

            LineApi.SaveTextFile("receive text=" + commandText + ",from " + userId);
            if (commandText == "我的會員資料")
            {
                if (isBinded)
                {
                    memberInfo mem = GetMember(userId);
                    vMemberData(token, mem);
                }
                else
                {
                    // LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能查看我的會員資料");

                }
            }
            else if (commandText == "修改會員資料")
            {
                if (isBinded)
                {
                    memberInfo mem = GetMember(userId);
                    vMemberEdit(token, mem);
                }
                else
                {
                    //LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能修改會員資料");

                }
            }
            else if (commandText == "修改會員所在地")
            {
                if (isBinded)
                {
                    WriteLastCommand(userId, "修改會員資料-地區");
                    vEditMemberZone(token , userId);
                }
                else
                {
                    //LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能修改會員地區");
                }
            }
            
            else if (commandText == "修改會員Email")
            {
                if (isBinded)
                {
                    WriteLastCommand(userId, "請輸入email");
                    LineApi.ReplyText(token, "請輸入Email"); 

                    //vEmail(token , userId , "");



                }
                else
                {
                    //LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能修改會員Email");
                }
            }
            else if (commandText == "完成上傳")
            {
                if (isBinded)
                {
                    UpdateLastCommandData(userId, "預約植醫-地區", "upload", "finish");
                    LineApi.ReplyText(token, "請選擇輔導單位行政區\n(若無輔導單位行政區，請選擇您所在行政區)");
                    vUploadComplete(userId);
                }
                else
                {
                    //LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能完成上傳");
                }
            }
            else if (commandText == "test")
            {
                //LineApi.PushText(userId, "測試主動推送訊息");
                /* 測試主動推送
                JArray actions = new JArray();
                actions.Add(LineApi.GenAction("uri", "線上掛號說明", "https://www.christophe.tw/lineaccount/description"));
                LineApi.PushButtonTmp(userId,"測試標題","測試內文", actions);
                */
                //測試回覆,location message
                //ReplyTest(token);
                //Locations(token);
            }
            else if (commandText == "unbind")
            {
                Unbind(userId);
                LineApi.SetUserMenu(userId, false);
                LineApi.ReplyText(token, "解除綁定");
            }
            else if (commandText == "menulist")
            {
                string list = LineApi.GetMenuList();

                LineApi.ReplyText(token, list);
            }
            else if (commandText == "delmenu")
            {
                //LineApi.DelMenu(menuId_A);
                //LineApi.DelMenu(menuId_B);
                LineApi.DelAllMenus();
            }
            else if (commandText == "addmenuA")
            {
                //加入登入前選單(line api中的設定)
                string strRes = LineApi.AddMenu(token, userId, false);
                LineApi.ReplyText(token, strRes);
                //上傳選單圖片
                JObject obj = JsonConvert.DeserializeObject<JObject>(strRes);
                string richMenuId = obj.GetValue("richMenuId").ToString();
                LineApi.UploadMenu(token, richMenuId, "\\images\\before_login.jpg");

                //驗證選單
                //ValidateMenu(token);

                //設定所有人的選單為登入前
                LineApi.SetMenu(richMenuId);
            }
            else if (commandText == "addmenuB")
            {
                //加登入後選單
                string strRes = LineApi.AddMenu(token, userId, true);
                LineApi.ReplyText(token, strRes);
                JObject obj = JsonConvert.DeserializeObject<JObject>(strRes);
                string richMenuId = obj.GetValue("richMenuId").ToString();
                LineApi.UploadMenu(token, richMenuId, "\\images\\after_login.jpg");
            }
            else if (commandText == "帳號綁定成功")
            {
                //LineApi.SetUserMenu(userId, true);
                //LineApi.ReplyText(token, "帳號已綁定");
            }
            else
            {
                //假如字串非特殊字,判斷是否有上個指令; 將字串視為要修改的值
                string keyin = commandText;
                string LastCommand = GetLastCommand(userId);
                if (LastCommand == "修改會員資料-地區")
                {
                    UpdateLastCommandData(userId, "修改會員資料-地區", "zone", keyin);

                    vMemberZoneConfirm(token , keyin);


                }

                
                else if (LastCommand == "請輸入email")
                {
                    UpdateLastCommandData(userId, "請輸入email", "email", keyin);
                   
                    memberInfo mem = GetMember(userId);
                    //vMemberData(token, mem);
                    vEmailConfirm(token , userId , keyin);
                }
                
                else if (LastCommand == "預約植醫-地區")
                {
                 

                    //確認行政區
                    //vZoneConfirm(token , keyin);

                    //選擇醫師
                    vDoctor(token , userId , keyin);

                }
          
                else if (LastCommand == "預約植醫-week")
                {
                    LineApi.PushText(userId, "以下為植物醫師"+ keyin + "可供預約日期，請選擇一個您想看診的日期。");
                    vDate(token , userId , keyin);

                }
                else if (LastCommand == "預約植醫-date")
                {
                    string[] arrDate = keyin.Split(' ');
                    UpdateLastCommandData(userId, "預約時段確認", "date", arrDate[0]);
                    LineApi.PushText(userId, "請再次確認以下的掛號資訊無誤");
                    vConfirmDate(token, arrDate[0], userId);

                }
                else if (LastCommand == "預約時段確認")
                {
                    try
                    {

                   
                        UpdateLastCommandData(userId, "預約成功", "confirm", keyin);

                        AddReserve(userId);
                        LineApi.ReplyText(token, "您已完成植物醫師預約");
                        LineApi.PushText(userId, "系統已發送預約簡訊至您的手機，諮詢當天請務必準時接聽電話，以利植醫進行諮詢。");

                        //sms
                        var com = GetLastCommandObj(userId);
                        JObject obj = JsonConvert.DeserializeObject<JObject>(com.Data);
                        string zone = obj.GetValue("zone").ToString();
                        string date = obj.GetValue("date").ToString();
                        string week = obj.GetValue("week").ToString();
                        string doctorID = obj.GetValue("doctor").ToString();

                        sysUserInfo Doctor = Service_sysUserInfo.GetDetail(doctorID);
                        memberInfo mem = GetMember(userId);

                        SmsReq req = new SmsReq
                        {
                            Mobile = mem.Mobile,
                            Message = $"預約成功-醫師:{Doctor.UserName}\n地區:{zone}\n時段:{date} {week}"
                        };
                        var sendRes = SmsHelper.SendSms(req);

                        //mail

                        MailHelper mailHelper = new MailHelper();
                        string refErrMsg = "";
                        string body = "";
                        body = $"{Doctor.UserName}植物醫師，您好:<br><br>";
                        body += $"日期:{date}<br>";
                        body += $"時段:{week}<br>";
                        mailHelper.SendMail(ref refErrMsg, Doctor.EmailAddress, "植物醫師診斷系統暨作物病蟲害預警系統-掛號通知信", body);


                        LineApi.SaveTextFile("sendto=" + Doctor.EmailAddress );
                    }
                   catch(Exception e)
                    {
                        LineApi.SaveTextFile(e.ToString());
                    }


                }
                else if (LastCommand == "會員專區-輸入email")
                {
                    UpdateLastCommandData(userId, "確認email", "email", keyin);
                    vEmailConfirm(token, userId, keyin);
                }
              


            }

        }

        //5.處理postback訊息
        private static void HandlePostback(bool isBinded, string token, string userId, string data)
        {
            string postBackData = data;
            //選單1
            if (postBackData == "會員註冊/綁定")
            {
                if (isBinded)
                {
                    LineApi.ReplyText(token, "已綁定");
                }
                else
                {
                    var responseStr = LineApi.SendPost("post", $"https://api.line.me/v2/bot/user/{userId}/linkToken");
                    string linkToken = JObject.Parse(responseStr)["linkToken"].ToString();
                    //LineApi.ReplyText(token, $"{base_url}/mobile/login?linkToken={linkToken}&openExternalBrowser=1");
                    //LineApi.ReplyText(token, $"{base_url}/mobile/login?userid={userId}&openExternalBrowser=1");

                    JArray actions = new JArray();
                    actions.Add(LineApi.GenAction("uri", "會員註冊/綁定", $"{base_url}/mobile/login?userid={userId}&openExternalBrowser=1"));
                    LineApi.ReplyButtonsTmp(token, "綁定會員", $"請您點選會員註冊／綁定", actions);
                }
            }
            else if (postBackData == "會員專區")
            {
                if (isBinded)
                {
                    vMemberZone(token);
                }
                else
                {
                    //LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能進入會員專區");
                }
            }
            else if (postBackData == "植物醫師據點")
            {
                vDoctorLocation(token ,userId);
            }
            else if (postBackData == "農藥與產銷資訊")
            {
                vMedicine(token);
            }
            else if (postBackData == "我要掛號找植醫")
            {
                vMakeApp(token);
            }
            else if (postBackData == "植物醫師線上掛號")
            {
                if (isBinded)
                {
                    WriteLastCommand(userId, "預約植醫-上傳照片");
                    vStep1(token);
                }
                else
                {
                    //LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能進行植物醫師線上掛號");
                }
            }
            else if (postBackData == "繼續上傳")
            {
                if (isBinded)
                {
                    //不該寫指令,會重覆
                    //WriteLastCommand(userId, "預約植醫-上傳照片");
                    vStep1(token);
                }
                else
                {
                    //LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能繼續上傳");
                }
            }
            else if (postBackData == "取消預約電子處方箋")
            {
                if (isBinded)
                {
                    vReserveList(token, userId);
                }
                else
                {
                    //LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能查看取消預約/處方箋");
                }
            }
            else if(postBackData == "其它時段")
            {
                if (isBinded)
                {
                    WriteLastCommand(userId, "預約植醫-上傳照片");
                    vStep1(token);
                }
                else
                {
                    //LineApi.ReplyText(token, strUnbind);
                    vNotLogin(token, userId , "才能選則時段");
                }
            }
            else if(postBackData == "請輸入Email")
            {
                WriteLastCommand(userId, "會員專區-輸入email");
                //do nothing
            }
            else if(postBackData == "確認email")
            {
                
                //vEmailConfirmInfo(token, mem);


                UpdateMember(userId, "email");

                LineApi.ReplyText(token , "確認");

                memberInfo mem = GetMember(userId);
                vEmailConfirmInfo(userId, mem , "您的會員資料已更新");

            }
            else if (postBackData == "取消email")
            {
                LineApi.PushText(userId, "請重新輸入Email"); 
                //vEmail(token);
                //vEmail(token, userId, "");
            }
            else if (postBackData == "會員修改-確認行政區")
            {
                LineApi.PushText(userId, "確認");

                UpdateMember(userId, "zone");
                memberInfo mem = GetMember(userId);
                //vMemberData(token, mem);
                //vMemberEdit(token,mem);
                vEmailConfirmInfo(userId, mem, "");
            }
            else if(postBackData == "農藥哪裡買?")
            {
                vMedicineBuyConfirm(token , userId , "");
            }
           // else if (postBackData == "農藥哪裡買-分享您的定位")
           // {
                

           // }
            else if (postBackData == "農藥哪裡買取消")
            {
                //vMedicineBuyConfirm(token, userId, "");
            }
            /*
            else if(postBackData == "植醫預約-確認行政區")
            {
               

                //var com = GetLastCommandObj(userId);
                //JObject obj = JsonConvert.DeserializeObject<JObject>(com.Data);
                //JArray zone = JsonConvert.DeserializeObject<JArray>(obj.GetValue("zone").ToString());

              

                vDoctor(token);
            }*/

            else
            {
                //Postback,傳更多參數(類似get 方式), 解析更多命令 (目前只有地點用到)
                string[] arr1 = new string[] { postBackData };
                if (postBackData.Contains("&"))
                {
                    arr1 = postBackData.Split('&');
                }
                string[] cmdArr = arr1[0].Split('=');

                if (cmdArr[1] == "location")
                {
                    string name = "";
                    string addr = "";
                    string latitude = "";
                    string longitude = "";
                    foreach (string param in arr1)
                    {
                        string[] cmdArr2 = param.Split('=');
                        string key = cmdArr2[0];
                        string value = cmdArr2[1];

                        if (key == "name")
                        {
                            name = value;
                        }
                        else if (key == "addr")
                        {
                            addr = value;
                        }
                        else if (key == "latitude")
                        {
                            latitude = value;
                        }
                        else if (key == "longitude")
                        {
                            longitude = value;
                        }
                    }
                    // LineApi.ReplyText(token, "產銷資訊請點我");
                    LineApi.ReplayLocation(token,
                                            name,
                                            addr,
                                            latitude,
                                            longitude
                                            );
                }
                else if(cmdArr[1] == "choice_doctor")
                {
                    //LineApi.PushText(userId, "postback ,data=" + postBackData);

                    string[] arrDoctorLoginID = arr1[1].Split('=');
                    string strDoctorID = arrDoctorLoginID[1];

                    string LastCommand = GetLastCommand(userId);
                    if (LastCommand == "預約植醫-醫師")
                    {
                        sysUserInfo Doctor = Service_sysUserInfo.GetDetail(strDoctorID);

                        LineApi.PushText(userId, Doctor.UserName);

                        LineApi.PushText(userId , "以下為植物醫師提供預約的時段，請選擇一個您想看診的時段。");

                        

                        vWeek(token , userId , Doctor.LoginID);
                    }

                }
                else if(cmdArr[1] == "map")
                {
                    string[] arrLat = arr1[1].Split('=');
                    string strLat = arrLat[1];

                    string[] arrLong = arr1[2].Split('=');
                    string strLong = arrLong[1];

                    string[] arrAddr = arr1[3].Split('=');
                    string strAddr = arrAddr[1];

                    LineApi.ReplayLocation(token , "查看" , strAddr , strLat , strLong );
                }
                else
                {
                    //RepLineApi.ReplyTextly(token, "其它postback事件");
                }
            }
        }

        public static void HandelLocation(bool isBinded, string token, double latitude, double longitude)
        {
            DataTable dt = GetDataTable($"select top 10 (abs(latitude - {latitude})*abs(longitude-{longitude})) as size,VendorName,VendorAddress,latitude,longitude,ContactPhone from pesticide_Sellers where isnull(Latitude,0)!=0 and isnull(Longitude,0)!=0 order by(abs(latitude - {latitude}) * abs(longitude - {longitude})) asc");
            JArray columns = new JArray();
            foreach (DataRow row in dt.Rows)
            {
                string strLatitude = row["Latitude"].ToString();
                string strLongitude = row["Longitude"].ToString();
                string name = row["VendorName"].ToString();
                string addr = row["VendorAddress"].ToString();
                string phone = row["ContactPhone"].ToString();
                var placeUrl = $"https://www.google.com.tw/maps/place/{strLatitude}|{strLongitude}?openExternalBrowser=1";
                columns.Add(LineApi.GenColumn("農藥哪裡找", $"{name}\n{addr}\n{phone}", "",
                                                                    new List<string>{
                                                                    $"uri,{phone},tel:"+phone.Replace("-",""),
                                                                    $"uri,查看地圖,{placeUrl}"
                                                                    //$"postback,查看地圖,action=location&latitude={strLatitude}&longitude={strLongitude}&addr={addr}&name={name}"
                                                                   })) ;
            }
            LineApi.ReplyCarouselTmp(token, columns);
        }


        /*
        private static void BindAccount(string msg, string token, string userId)
        {
            var _checkCode = msg.Substring(6);
            string reMsg = "";
            if (!string.IsNullOrEmpty(_checkCode))
            {
                var findMember = Service_MemberInfo.GetList(x => x.LineBindCode == _checkCode && x.LineBindLimit >= DateTime.Now).FirstOrDefault();
                if (findMember != null)
                {
                    var bindRes = Service_MemberInfo.BindLineMessage(findMember.ID, userId);
                    if (bindRes.result)
                    {
                        reMsg = "綁定成功";
                    }
                    else
                    {
                        reMsg = "綁定失敗" + bindRes.Msg;
                    }
                }
                else
                {
                    reMsg = "無此驗證碼或驗證碼過期";
                }
                LineApi.ReplyText(token, reMsg);
            }
        }*/

        public static void vMedicineBuyConfirm(string token , string userId , string text)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("location", "分享您的定位", "分享您的定位"));
            actions.Add(LineApi.GenAction("postback", "取消", "農藥哪裡買-取消"));
            LineApi.PushButtonsTmp(userId, "農藥哪裡買?", "是否分享您的定位?", actions);
        }

        public static void vEmail(string token , string userId , string text)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("postback", "請輸入Email", "請輸入Email"));
            LineApi.ReplyButtonsTmp(token, "", "訊息", actions);

            //LineApi.ReplyBubbleTmp(token);
            //LineApi.ReplyFlexCarousel(token);
            //LineApi.ReplyFlexBubble(token , "請輸入Email");
        }


        public static void vEmailConfirm(string token, string userId, string text)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("postback", "確認", "確認email"));
            actions.Add(LineApi.GenAction("postback", "重新輸入", "取消email"));
            LineApi.ReplyButtonsTmp(token, "確認會員Email", "請確認您所輸入的Email:"+text, actions);
        }

        public static void vEmailConfirmInfo(string userId, memberInfo mem , string title)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("message", "修改會員資料", "修改會員資料"));
            string text = $"姓名:{mem.Name}\n所在區域:{mem.City}{mem.District}\n手機號碼:{mem.Mobile}\nEmail:{mem.Email}";
            //LineApi.ReplyButtonsTmp(token, title, text, actions);

            LineApi.PushButtonsTmp(userId, "", text, actions);
        }


        #region 產出LINE訊息畫面部份
        public static void vNotLogin(string token, string userId , string text)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("postback", "會員註冊/綁定", "會員註冊/綁定"));
            //actions.Add(LineApi.GenAction("message", "修改會員資料", "修改會員資料"));

            LineApi.ReplyButtonsTmp(token, "尚未綁定Line帳號", $"請您點選會員註冊／綁定，完成綁定流程後，{text}。", actions);
        }

        /// <summary>
        /// 產出電子處方箋訊息
        /// </summary>
        public static void vReserveList(string token, string userId)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("uri", "前往", $"{base_url}/mobile/reserve?userId={userId}"));
            LineApi.ReplyButtonsTmp(token, "取消預約/處方箋", "前往我的取消預約/處方箋", actions);
        }
        /// <summary>
        /// 產出 修改會員資料 訊息
        /// </summary>
        public static void vMemberZone(string token)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("message", "我的會員資料", "我的會員資料"));
            actions.Add(LineApi.GenAction("message", "修改會員資料", "修改會員資料"));

            LineApi.ReplyButtonsTmp(token, "會員專區", "為您提供的服務", actions);
        }

        public static void vMemberZoneConfirm(string token,string text)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("postback", "確認", "會員修改-確認行政區"));
            actions.Add(LineApi.GenAction("message", "重新輸入", "修改會員所在地"));
            LineApi.ReplyButtonsTmp(token, "確認所在行政區域", "請確認您所在的行政區域:" + text, actions);
        }

        /*
        public static void vZoneConfirm(string token, string text)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("postback", "確認", "植醫預約-確認行政區"));
            actions.Add(LineApi.GenAction("postback", "重新輸入", "植醫預約-取消所在區"));
            LineApi.ReplyButtonsTmp(token, "確認所在行政區域", "請確認您所在的行政區域:" + text, actions);
        }*/

        /// <summary>
        /// 產出 我的會員資料 訊息
        /// </summary>
        public static void vMemberData(string token, memberInfo mem)
        {
            JArray actions = new JArray();
            //actions.Add(LineApi.GenAction("message", "我的會員資料", "我的會員資料"));
            actions.Add(LineApi.GenAction("message", "修改會員資料", "修改會員資料"));
            string text = $"姓名:{mem.Name}\n所在區域:{mem.City}{mem.District}\n手機號碼:{mem.Mobile}\nEmail:{mem.Email}";
            LineApi.ReplyButtonsTmp(token, "", text, actions);
        }
        /// <summary>
        /// 產出 會員修改
        /// </summary>
        public static void vMemberEdit(string token, memberInfo mem)
        {
            
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("message", "修改會員所在地", "修改會員所在地"));
            actions.Add(LineApi.GenAction("message", "修改會員Email", "修改會員Email"));
            string text = $"姓名:{mem.Name}\n所在區域:{mem.City}{mem.District}\n手機號碼:{mem.Mobile}\nEmail:{mem.Email}";
            LineApi.ReplyButtonsTmp(token, "", text, actions);
            

            /*
            string text = $"姓名:{mem.Name}\n所在區域:{mem.City}{mem.District}\n手機號碼:{mem.Mobile}\nEmail:{mem.Email}";
            JArray columns = new JArray();
          
            columns.Add(LineApi.GenColumn("修改會員資料", text, "",
                                                                new List<string>{
                                                                    $"message,修改會員所在地,修改會員所在地",
                                                                    $"message,修改會員Email,修改會員Email"
                                                                }));
           
            LineApi.ReplyCarouselTmp(token, columns);
            */

        }
        /// <summary>
        /// 產出 會員修改地區
        /// </summary>
        public static void vEditMemberZone(string token , string userId)
        {
            //先取得資料
            var list = GetDistricts("高雄市");
            // List<string> actionList = new List<string>();

            /*
             先分成每5個一個LIST
           */
            int rowIndex = 1;
            List<List<string>> allList = new List<List<string>>();
            List<string> list1 = new List<string>();
            foreach (var item in list)
            {

                if (rowIndex % 5 == 1 && rowIndex > 1)
                {
                    allList.Add(list1);
                    list1 = new List<string>();
                }

                list1.Add($"message,{item.Name},{item.Name}");
                rowIndex++;
            }
            if (list1.Count() > 0)
            {
                allList.Add(list1);
            }


            //產生JSON
            JArray bubbleList = new JArray();
            foreach (var tmplist in allList)
            {
                JArray comList = new JArray();

                int titleIndex = 0;
                foreach (string act in tmplist)
                {
                    string[] arr1 = act.Split(',');
                    if (titleIndex == 0)
                    {
                        //string[] arrStr = arr1[1].Split(' ');
                        //string strWeek = arrStr[0];
                        comList.Add(LineApi.GenFlexComponent("text", "行政區"));
                    }
                    comList.Add(LineApi.GenFlexButton(LineApi.GenAction(arr1[0], arr1[1], arr1[2])));
                    titleIndex++;
                }
                JObject bubble1 = LineApi.GetFlexBubble(comList);
                bubbleList.Add(bubble1);
            }

            LineApi.PushFlexCarouselTmp(userId, bubbleList);
        }

        /// <summary>
        /// 產出 植物醫師線上掛號 訊息
        /// </summary>
        public static void vMakeApp(string token)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("uri", "線上掛號說明", $"{base_url}/mobile/description"));
            actions.Add(LineApi.GenAction("postback", "植物醫師線上掛號", "植物醫師線上掛號"));
            LineApi.ReplyButtonsTmp(token, "植物醫師線上掛號", "為您提供的服務", actions);
        }
        /// <summary>
        /// 產出 作物病徵圖片
        /// </summary>
        public static void vStep1(string token)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("camera", "開啟相機", ""));
            actions.Add(LineApi.GenAction("cameraRoll", "開啟相簿", ""));
            LineApi.ReplyButtonsTmp(token, "植物醫師線上掛號", "請提供作物病徵圖片", actions);
        }
        /// <summary>
        /// 產出 確認上傳完成
        /// </summary>
        public static void vConfirmUpload(string token , string userId)
        {
            var com = GetLastCommandObj(userId);
            JObject obj = JsonConvert.DeserializeObject<JObject>(com.Data);
            JArray arrMagId = JsonConvert.DeserializeObject<JArray>(obj.GetValue("msgId").ToString());
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("postback", "繼續上傳", "繼續上傳"));
            actions.Add(LineApi.GenAction("message", "完成上傳", "完成上傳"));
            LineApi.ReplyButtonsTmp(token, "是否繼續上傳?", "你已上傳了"+ arrMagId.Count + "張照片", actions);
        }

        /// <summary>
        /// 產出 上傳完成,選地區
        /// </summary>
        public static void vUploadComplete(string userId)
        {
            //先取得資料
            var list = GetDistricts("高雄市");
           // List<string> actionList = new List<string>();

            /*
             先分成每5個一個LIST
           */
            int rowIndex = 1;
            List<List<string>> allList = new List<List<string>>();
            List<string> list1 = new List<string>();
            foreach(var item in list)
            {

                if(rowIndex %5 ==1  && rowIndex >1)
                {
                    allList.Add(list1);
                    list1 = new List<string>();
                }

                list1.Add($"message,{item.Name},{item.Name}");
                rowIndex++;
            }
            if(list1.Count() >0)
            {
                allList.Add(list1);
            }


            //產生JSON
            JArray bubbleList = new JArray();
            foreach (var tmplist in allList)
            {
                JArray comList = new JArray();

                int titleIndex = 0;
                foreach(string act in tmplist)
                {
                    string[] arr1 = act.Split(',');
                    if (titleIndex == 0)
                    {
                        //string[] arrStr = arr1[1].Split(' ');
                        //string strWeek = arrStr[0];
                        comList.Add(LineApi.GenFlexComponent("text", "行政區"));
                    }
                    comList.Add(LineApi.GenFlexButton(LineApi.GenAction(arr1[0], arr1[1], arr1[2])));
                    titleIndex++;
                }
                JObject bubble1 = LineApi.GetFlexBubble(comList);
                bubbleList.Add(bubble1);
            }

            LineApi.PushFlexCarouselTmp(userId , bubbleList);
        }

        //轉成每三個為一個List<string> (調整成Carousel template 的結構)
        public static List<List<string>> ToActionListByWeek(List<string> list)
        {
            JArray actions = new JArray();
            List<List<string>> columnsList = new List<List<string>>();

            for(int i =0; i<8;i++)
            {
                List<string> tmpList1 = new List<string>();
                foreach(string item in list)
                {
                    string[] arr = item.Split(',');

                    int weekNum = int.Parse( arr[3]);
                  
                    if(i == (weekNum -1))
                    {
                        tmpList1.Add(item);
                    }
                }

                if(tmpList1.Count() >0)
                {
                    columnsList.Add(tmpList1);
                }
            }
            return columnsList;
        }

        //轉成每三個為一個List<string> (調整成Carousel template 的結構)
        public static List<List<string>> To3ActionList(List<string> list)
        {
            JArray actions = new JArray();
            List<List<string>> columnsList = new List<List<string>>();
            if (list.Count() > 0)
            {
                //LineApi.SaveTextFile("list.Count():" + list.Count());
                int count = 0;
                List<string> actionList = new List<string>();
                foreach (string item in list)
                {
                    actionList.Add(item);
                    if (count == 2)
                    {
                        columnsList.Add(actionList);
                        //清空
                        actionList = new List<string>();
                        count = 0;
                        continue;
                    }
                    count++;
                }
                LineApi.SaveTextFile("actionList.Count:" + actionList.Count);
                if (actionList.Count > 0)
                {
                    //有2頁 (單頁不用補 action)
                    if (columnsList.Count >0)
                    {
                        if (actionList.Count == 1)
                        {
                            actionList.Add($"message,test,test");
                        }
                        if (actionList.Count == 2)
                        {
                            actionList.Add($"message,test,test");
                        }
                    }
                    columnsList.Add(actionList);
                }
            }
            return columnsList;
        }


        /// <summary>
        /// 產出 植醫所在區域
        /// </summary>
        public static void vDoctorLocation(string token ,string userId)
        {
            JArray columns = new JArray();


            List<FileManagement> list = Service_FileManagement
                                                  .GetList("line_doctor_img", null)
                                                  .OrderBy(a => a.TableID)
                                                  .ToList<FileManagement>();
            if(list.Count > 0 )
            {
                foreach (FileManagement f in list)
                {

                    //columns.Add(LineApi.GenColumn("植醫所在區域", "地區1", $"{base_admin_url}/ErrorLog/" + f.FileName,
                    //                                              new List<string>{
                    //                                             "message,更多,更多"
                    //                                             }));
                    LineApi.PushImg(userId , $"{base_admin_url}/ErrorLog/" + f.FileName , $"{base_admin_url}/ErrorLog/" + f.FileName);
                }

                //LineApi.ReplyCarouselTmp(token, columns);
            }
            else
            {
                LineApi.ReplyText(token, "尚未設定");
            }
           

        }

        /// <summary>
        /// 產出 產銷資訊請點我
        /// </summary>
        public static void vMedicine(string token)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("postback", "農藥哪裡買?", "農藥哪裡買?"));
            actions.Add(LineApi.GenAction("uri", "產銷資訊請點我", "https://liff.line.me/1645278921-kWRPP32q/?accountId=kh_agrinfo_bot"));
            LineApi.ReplyButtonsTmp(token, "農藥與產銷資訊", "請選擇以下服務", actions);
        }
        /// <summary>
        /// 產出 醫師選擇
        /// </summary>
        public static void vDoctor(string token , string userId, string zone)
        {
            try
            {
                zone = zone.Replace("'", "");
                zone = zone.Replace("/*", "");
                zone = zone.Replace("*/", "");
                zone = zone.Replace("\"", "");


                var district = GetDistrictByName(zone);

                List<sysUserInfo> list = Service_sysUserInfo.GetDoctorList().Where(x => x.State == "Y").ToList();
                list = list.Where(x => x.RoleID != "R08" || (x.District ?? "").Contains(district.Zip)).ToList();
                //DataTable dt = GetDataTable($"select * from sysUserInfo where district in ('{district.Zip}')");
                //JArray columns = new JArray();
                //foreach (DataRow row in dt.Rows)
                //{
                //    sysUserInfo newUser = new sysUserInfo();

                //    if(row["UserName"] != null)
                //    {
                //        newUser.UserName = row["UserName"].ToString();
                //    }

                //    if (row["LoginID"] != null)
                //    {
                //        newUser.LoginID = row["LoginID"].ToString();
                //    }
                //    list.Add(newUser);
                //}


                List<string> actionList = new List<string>();
                //每筆產生action list
                int MaxCount = 30;
                int index = 0;
                foreach (var item in list)
                {
                    if (index >= MaxCount)
                    {
                        break;
                    }
                    // 20221130 for審查 先暫時固定 高佩琳
                    /*
                    if (item.LoginID != "peilin26")
                    {
                        continue;
                    }*/

                    actionList.Add($"postback,{item.UserName},act=choice_doctor&id={item.LoginID}");
                    index++;
                }
                //將每5筆action,分成一個bubble
                List<List<string>> columnsList = To5ActionList(actionList);

                //產生bubble
                JArray bubbleList = new JArray();
                foreach (var tmplist in columnsList)
                {
                    JArray comList = new JArray();

                    int titleIndex = 0;
                    foreach (string act in tmplist)
                    {
                        string[] arr1 = act.Split(',');
                        if (titleIndex == 0)
                        {
                            //string[] arrStr = arr1[1].Split(' ');
                            //string strWeek = arrStr[0];
                            comList.Add(LineApi.GenFlexComponent("text", "看診植物醫師"));
                        }
                        comList.Add(LineApi.GenFlexButton(LineApi.GenAction(arr1[0], arr1[1], arr1[2])));
                        titleIndex++;
                    }
                    JObject bubble1 = LineApi.GetFlexBubble(comList);
                    bubbleList.Add(bubble1);
                }
               

                //確認有醫師才更新至下一階段
                if(list.Count >0)
                {
                    UpdateLastCommandData(userId, "預約植醫-醫師", "zone", zone);
                    LineApi.ReplyFlexCarouselTmp(token, bubbleList);

                }
                else
                {
                    LineApi.ReplyText(token, "無可選擇的植物醫師，請選擇其它地區。");
                }
            }
            catch (Exception ex)
            {
                LineApi.SaveTextFile(ex.ToString());
            }
        }


        private static District GetDistrictByName(string districtName)
        {
            
            District district = new District();
            string _directory = AppDomain.CurrentDomain.BaseDirectory;
            _directory = String.Concat(_directory, "/assets/");
            using (StreamReader r = new StreamReader(String.Concat(_directory, "taiwan_districts.json")))
            {
                string json = r.ReadToEnd();
                List<CityDistricts> items = JsonConvert.DeserializeObject<List<CityDistricts>>(json);
                
                var list = items.Find(x => x.Name == "高雄市").Districts;

                 district = list.Where(a => a.Name == districtName).Take(1).SingleOrDefault();
            }
            return district;
        }

        private static List<List<string>> To5ActionList(List<string> actionList)
        {
            int rowIndex = 1;
            List<List<string>> allList = new List<List<string>>();
            List<string> list1 = new List<string>();
            foreach (var item in actionList)
            {
                if (rowIndex % 5 == 1 && rowIndex > 1)
                {
                    allList.Add(list1);
                    list1 = new List<string>();
                }
                list1.Add(item);
                rowIndex++;
            }
            if (list1.Count() > 0)
            {
                allList.Add(list1);
            }
            return allList;
        }
        /// <summary>
        /// 產出 星期時段選擇
        /// </summary>
        public static void vWeek(string token , string userId , string doctoID)
        {
            try
            {
                //先更新輸入的的doctor
                UpdateLastCommandData(userId, "預約植醫-醫師", "doctor", doctoID);
                var com = GetLastCommandObj(userId);
                JObject obj = JsonConvert.DeserializeObject<JObject>(com.Data);
                //string strDoctor = obj.GetValue("doctor").ToString();

                sysUserInfo Doctor =  Service_sysUserInfo.GetDetail(doctoID);
                // List<doctorDutyRecord> list = Service_doctor_DutyRecord.GetDoctorDutyList(Doctor.LoginID)
                //                                    .Where(a=>a.Date > dt.AddDays(1))                                
                //                                    .ToList();
                DataTable dtList = GetDoctorDutyByID(Doctor.LoginID, userId);

                List<string> actionList = new List<string>();
                //每筆產生action list
                int MaxCount = 30;
                int index = 0;
                foreach (DataRow item in dtList.Rows)
                {
                    //if (index >= MaxCount)
                    //{
                    //    break;
                    //}

                    string week = Num2Week( int.Parse( item["numWeek"].ToString()));
                    string time = item["Time"].ToString();
                    actionList.Add($"message,{week} {time},{week} {time},{item["numWeek"].ToString()}");
                    index++;
                }
                //將每3筆action,分成一個column
                List<List<string>> columnsList = ToActionListByWeek(actionList);

                JArray bubbleList = new JArray();
                foreach (var weekList in columnsList)
                {
                    JArray comList = new JArray();
                    
                    int titleIndex = 0;
                    foreach (string act in weekList)
                    {
                        string[] arr1 = act.Split(',');
                        if (titleIndex==0)
                        {
                            string[] arrStr = arr1[1].Split(' ');

                            string strWeek = arrStr[0];
                            comList.Add(LineApi.GenFlexComponent("text", strWeek));
                        }
                      
                        comList.Add(LineApi.GenFlexButton(LineApi.GenAction(arr1[0], arr1[1], arr1[2])));

                        titleIndex++;
                    }
                    JObject bubble1 = LineApi.GetFlexBubble(comList);
                    bubbleList.Add(bubble1);
                }


                /*

                JArray columns = new JArray();
                foreach (List<string> actList in columnsList)
                {
                    columns.Add(LineApi.GenColumn("選擇時間", "請選擇以下服務", "", actList));
                }*/
                if (dtList.Rows.Count > 0)
                {
                    //LineApi.ReplyCarouselTmp(token, columns);
                    LineApi.ReplyFlexCarouselTmp(token, bubbleList);
                    //有時段選擇才更新下個指令
                    UpdateLastCommandData(userId, "預約植醫-week", "doctor", doctoID);
                }
                else
                {
                    LineApi.ReplyText(token, " 無可預約時段，請選擇其它植物醫師或時段。");
                }
            }
            catch(Exception e)
            {
                LineApi.SaveTextFile(e.ToString());
            }
        }

        /// <summary>
        /// 產出 日期選擇
        /// </summary>
        public static void vDate(string token , string userId , string keyin)
        {
            UpdateLastCommandData(userId, "預約植醫-week", "week", keyin);

            string strDate = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime dt = Convert.ToDateTime(strDate);
            var com = GetLastCommandObj(userId);
            JObject obj = JsonConvert.DeserializeObject<JObject>(com.Data);
            string strDoctorID = obj.GetValue("doctor").ToString();
            string strWeek = obj.GetValue("week").ToString();

            string[] arrTime = strWeek.Split(' ');
            int weekNum = Week2Num(arrTime[0]);
            string time = arrTime[1];
            sysUserInfo Doctor = Service_sysUserInfo.GetDetail(strDoctorID);
            // List<doctorDutyRecord> list = Service_doctor_DutyRecord.GetDoctorDutyList(Doctor.LoginID)
            //                                     .Where(a => a.Date > dt.AddDays(1) && a.Time == arrTime[1])
            //                                     .ToList();
            DataTable dtList = GetDoctorDutyByWeektime(Doctor.LoginID, userId , weekNum, time);
            List<string> actionList = new List<string>();
            //每筆產生action list
            int MaxCount = 30;
            int index = 0;
            foreach (DataRow item in dtList.Rows)
            {
                if (index >= MaxCount)
                {
                    break;
                }
                string strDate2 = Convert.ToDateTime( item["Date"].ToString()).ToString("yyyy-MM-dd");
               

                string week = Num2Week(int.Parse(item["numWeek"].ToString()));

                actionList.Add($"message,{strDate2} ({week}),{strDate2}  ({week})");
                index++;
            }
            //將每3筆action,分成一個column
            List<List<string>> columnsList = To5ActionList(actionList);
            /*
            JArray columns = new JArray();
            foreach (List<string> actList in columnsList)
            {
                columns.Add(LineApi.GenColumn("選擇時間", "請選擇以下服務", "", actList));
            }*/



            //產生bubble
            JArray bubbleList = new JArray();
            foreach (var tmplist in columnsList)
            {
                JArray comList = new JArray();
                int titleIndex = 0;
                foreach (string act in tmplist)
                {
                    string[] arr1 = act.Split(',');
                    if (titleIndex == 0)
                    {
                        comList.Add(LineApi.GenFlexComponent("text", "選擇日期"));
                    }
                    comList.Add(LineApi.GenFlexButton(LineApi.GenAction(arr1[0], arr1[1], arr1[2])));
                    titleIndex++;
                }
                JObject bubble1 = LineApi.GetFlexBubble(comList);
                bubbleList.Add(bubble1);
            }
         



            if (actionList.Count > 0)
            {
                //LineApi.ReplyCarouselTmp(token, columns);
                LineApi.ReplyFlexCarouselTmp(token, bubbleList);

                //確定有日期可選才進下個指令
                UpdateLastCommandData(userId, "預約植醫-date", "week", keyin);
            }
            else
            {
                LineApi.ReplyText(token, "無可預約時段，請選擇其它植物醫師或時段。");
            }
        }
        /// <summary>
        /// 產出 掛號最後確認 
        /// </summary>
        public static void vConfirmDate(string token, string date, string userId)
        {
            var com = GetLastCommandObj(userId);
            JObject obj = JsonConvert.DeserializeObject<JObject>(com.Data);
            string zone = obj.GetValue("zone").ToString();
            string doctorID = obj.GetValue("doctor").ToString();
            sysUserInfo Doctor = Service_sysUserInfo.GetDetail(doctorID);

            string week = obj.GetValue("week").ToString();

            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("message", "確認", "預約時段確認"));
            actions.Add(LineApi.GenAction("postback", "重新選擇", "其它時段"));

            LineApi.ReplyButtonsTmp(token, "植物醫師掛號單", $"醫師:{Doctor.UserName}\n地區:{zone}\n時段:{date} {week}", actions);
        }
        /// <summary>
        /// 產出 農藥哪裡買 
        /// </summary>
        public static void vMedicineBuy(string token)
        {
            JArray actions = new JArray();
            actions.Add(LineApi.GenAction("location", "分享您所在位置", ""));
            LineApi.ReplyButtonsTmp(token, "農藥哪裡買", "請分享位置後確認,會找出離您最近的農藥商店", actions);
        }
        #endregion


        #region 更新資料至資料庫部份
        /// <summary>
        /// 判斷是否綁定
        /// </summary>
        private static bool IsBinded(string userId)
        {
            var lineUser = Service_MemberInfo.GetList(x => x.LineUserId == userId).FirstOrDefault();
            if (lineUser == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 星期幾轉 number
        /// </summary>
        public static string Num2Week(int num)
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[num - 1];
            return week;
        }

        public static int Week2Num(string strWeek)
        {
            int index = 0;
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            foreach (string str in weekdays)
            {
                if (strWeek == str)
                {
                    return index + 1;
                }
                index++;
            }
            return index;
        }
        /// <summary>
        /// 解除綁定
        /// </summary>
        private static void Unbind(string userId)
        {
            string sErrMsg = "";
            BaseRepository Base = new BaseRepository();
            var model = Base.QueryData<memberInfo>(ref sErrMsg, x => x.LineUserId == userId).SingleOrDefault();
            model.LineUserId = "";
            model.LineBindCode = "";
            model.UpdatedAt = DateTime.Now;
            bool rslt = Base.Update(ref sErrMsg);
        }

        /// <summary>
        /// 取得地區
        /// </summary>
        private static IEnumerable<District> GetDistricts(string city)
        {
            IEnumerable<District> list2 = new List<District>();
            string _directory = AppDomain.CurrentDomain.BaseDirectory;
            _directory = String.Concat(_directory, "/assets/");
            using (StreamReader r = new StreamReader(String.Concat(_directory, "taiwan_districts.json")))
            {
                string json = r.ReadToEnd();
                var list = JsonConvert.DeserializeObject<List<CityDistricts>>(json);

                list2 = list.Where(a => a.Name == city).SingleOrDefault().Districts;
            }
            return list2;
        }

        /// <summary>
        /// 新增預約掛號
        /// </summary>
        public static void AddReserve(string userId)
        {
            IEnumerable<District> list2 = new List<District>();
            string _directory = AppDomain.CurrentDomain.BaseDirectory;
            _directory = String.Concat(_directory, "/assets/");
            using (StreamReader r = new StreamReader(String.Concat(_directory, "taiwan_districts.json")))
            {
                string _json = r.ReadToEnd();
                var list = JsonConvert.DeserializeObject<List<CityDistricts>>(_json);

                list2 = list.Where(a => a.Name == "高雄市").SingleOrDefault().Districts;
            }

            LineCommand CommandObj = GetLastCommandObj(userId);
            JObject json = JsonConvert.DeserializeObject<JObject>(CommandObj.Data);

            string strDate = json.GetValue("date").ToString();
            string strWeek = json.GetValue("week").ToString();
            string[] arr = strWeek.Split(' ');
            string strDoctorID = json.GetValue("doctor").ToString();
            string strZone = json.GetValue("zone").ToString();

            sysUserInfo Doctor = Service_sysUserInfo.GetDetail(strDoctorID);
            memberInfo mem = GetMember(userId);

            string sErrMsg = "";
            BaseRepository Base = new BaseRepository();
            var model = new doctorSchedule();
            //必填
            model.ReserveDatetime = Convert.ToDateTime( strDate + " " + arr[1] + ":00");
            model.PlantingArea = 10;
            model.ContactMethod = "";
            model.Context = "";
            model.MonthIndex = 1;
            model.IsDel = false;
            model.CreatedAt = DateTime.Now;

            model.Mobile = mem.Mobile;
            model.Sexy = mem.Sexy;
            model.Origin = "Line";
            model.Inquiry = "Network";
            model.MemberInfoID = mem.ID;
            model.Name = mem.Name;
            model.Status = "Appointment";
            model.Creator = "system";

            model.ContactMethod = "[\"Line\"]";
            model.Zip = list2.FirstOrDefault(x => x.Name == strZone)?.Zip;
            model.District = strZone;
            model.LoginID = Doctor.LoginID;

            //更新回資料庫
            bool rslt = Base.Insert(model, ref sErrMsg);
            long new_inser_sch_id = model.ID;
            //更新時段
           doctorDutyRecord duty =  Service_doctor_DutyRecord.GetDetailByDatetime(Doctor.LoginID , strDate , arr[1]) ;
            duty.DoctorScheduleID = new_inser_sch_id;
            duty.UpdatedAt = DateTime.Now;
            duty.UpdateUser = "line reserve by " + mem.Name + "(" + mem.LoginID + ")";
            Service_doctor_DutyRecord.Update(duty);

            List<string> CropSymptomsFiles = new List<string>();
            //更新檔案
            LineCommand cmd = GetLastCommandObj(userId);
            //JObject json = JsonConvert.DeserializeObject<JObject>(cmd.Data);
            foreach (string item in json.GetValue("msgId"))
            {
                var filePath = $"/UploadedFiles/LineUpload/CropSymptomsFiles/{userId}_{item}.jpg";
                var finalFolder = $"/UploadedFiles/PlantDoctor/{model.ID}/CropSymptomsFiles";
                FileHelper.CreateFolder(backendDirector+finalFolder);

                var finalFilePath = $"/UploadedFiles/PlantDoctor/{model.ID}/CropSymptomsFiles/{userId}_{item}.jpg";

                File.Move((backendDirector + filePath), (backendDirector + finalFilePath));

                FileManagement file = new FileManagement();
                file.TableName = "doctor_Schedule";
                file.TableID = new_inser_sch_id.ToString();
                file.FileName = userId + "_" + item + ".jpg";
                file.FilePath = backendDirector + finalFilePath;

                //FileStream fs = new FileStream(檔名, FileMode.Open, FileAccess.Read);
                file.FileType = "image/jpeg";
                file.UploadTime = DateTime.Now;
                file.FileLength = 999999999;
                Service_FileManagement.Insert(file);

                CropSymptomsFiles.Add(finalFilePath);
            }
            if (CropSymptomsFiles.Any())
            {
                model.CropSymptoms = string.Join(",", CropSymptomsFiles);
                Service_doctorSchedule.Update(model);
            }
        }

        /// <summary>
        /// 更新nonce欄位,line綁定步驟
        /// </summary>
        public static string UpdateNonce(string account, string nonce)
        {
            string sErrMsg = "";
            BaseRepository Base = new BaseRepository();
            var model = Base.QueryData<memberInfo>(ref sErrMsg, x => x.LoginID == account).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
            }
            model.LineUserId = "";
            model.LineNonce = nonce;
            model.UpdatedAt = DateTime.Now;
            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return sErrMsg;
        }

        /// <summary>
        /// 綁定,將LINE ID 更新到 LineUserId 欄位
        /// </summary>
        public static string BiindLineID(string userId, string nonce)
        {
            string sErrMsg = "";
            BaseRepository Base = new BaseRepository();
            var model = Base.QueryData<memberInfo>(ref sErrMsg, x => x.LineNonce == nonce).SingleOrDefault();
            if (model == null)
            {
                sErrMsg = "查無欲更新的資料";
            }
            model.LineUserId = userId;
            //model.LineBindCode = nonce;
            model.UpdatedAt = DateTime.Now;
            //更新回資料庫
            bool rslt = Base.Update(ref sErrMsg);
            return sErrMsg;
        }

        /// <summary>
        /// 取得member資料
        /// </summary>
        public static memberInfo GetMember(string userId)
        {
            try
            {
                string sErrMsg = "";
                BaseRepository Base = new BaseRepository();
                var model = Base.QueryData<memberInfo>(ref sErrMsg, x => x.LineUserId == userId).SingleOrDefault();
                return model;
            }
            catch (Exception e)
            {
                LineApi.SaveTextFile(e.Message);
            }
            return new memberInfo();
        }
        /// <summary>
        ///更新member資料
        /// </summary>
        public static void UpdateMember(string userId, string type)
        {
            try
            {
                DataTable dt = GetDataTable($"select top 1 * from LineCommand where LineUserId='{userId}' order by CreateDate desc");
                string strJson = dt.Rows[0]["Data"].ToString();
                JObject objData = JsonConvert.DeserializeObject<JObject>(strJson);
                string sErrMsg = "";

                BaseRepository Base = new BaseRepository();
                var model = Base.QueryData<memberInfo>(ref sErrMsg, x => x.LineUserId == userId).SingleOrDefault();
                if (type == "zone")
                {
                    model.District = objData.GetValue("zone").ToString();
                }

                if (type == "email")
                {
                    model.Email = objData.GetValue("email").ToString();
                }
                model.UpdatedAt = DateTime.Now;

                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
                //return new BaseResult(rslt, sErrMsg);
            }
            catch (Exception e)
            {
                LineApi.SaveTextFile(e.Message);
            }
        }

        public static void UpdateMemberUserID(string account , string userId)
        {
            try
            {
                string sErrMsg = "";
                BaseRepository Base = new BaseRepository();
                var model = Base.QueryData<memberInfo>( x => x.LoginID == account).SingleOrDefault();
                model.UpdatedAt = DateTime.Now;
                model.LineUserId = userId;
                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
            }
            catch (Exception e)
            {
                LineApi.SaveTextFile(e.Message);
            }
        }

        /// <summary>
        /// 取得正在操作指令物件
        /// </summary>
        public static LineCommand GetLastCommandObj(string userId)
        {
            string sErrMsg = "";
            BaseRepository Base = new BaseRepository();
            var model = Base.QueryData<LineCommand>(ref sErrMsg, x => x.LineUserId == userId)
                            .OrderByDescending(a => a.CreateDate)
                            .Take(1)
                            .SingleOrDefault();
            return model;
        }

        /// <summary>
        /// 取得正在操作指令
        /// </summary>
        public static string GetLastCommand(string userId)
        {
            string cmd = "";
            try
            {
                DataTable dt = GetDataTable($"select top 1 * from LineCommand where LineUserId='{userId}' order by CreateDate desc");
                if (dt.Rows.Count > 0)
                {
                    cmd = dt.Rows[0]["Command"].ToString();
                }
            }
            catch (Exception e)
            {
                LineApi.SaveTextFile(e.Message);
            }
            return cmd;
        }

        /// <summary>
        /// 取得可預約時段, 判斷欄位 DoctorScheduleID=0 為無預約
        /// </summary>
        public static DataTable GetDoctorDutyByID(string doctorId , string userId)
        {
            string strDate = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime date1 = Convert.ToDateTime(strDate);
            date1.AddDays(1);
            string sql = $"select distinct DATEPART(DW,Date) as numWeek,Time  from doctor_DutyRecords where DoctorId='{doctorId}' and Date>= '" + date1.ToString("yyyy-MM-dd") + "' and DoctorScheduleID=0";
            DataTable dt = GetDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 取得可預約日期, 判斷欄位 DoctorScheduleID=0 為無預約
        /// </summary>
        public static DataTable GetDoctorDutyByWeektime(string doctorId, string userId, int weekNum, string time)
        {
            string strDate = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime date1 = Convert.ToDateTime(strDate);
            date1.AddDays(1);
            string sql = $"select distinct Date,DATEPART(DW,Date) as numWeek  from doctor_DutyRecords where DoctorId='{doctorId}' and time='{time}' and DATEPART(DW,Date)={weekNum} and Date>= '" + date1.ToString("yyyy-MM-dd") + "' and DoctorScheduleID=0";
            DataTable dt = GetDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 開始一段新的指令
        /// </summary>
        public static void WriteLastCommand(string userId, string cmd)
        {
            try
            {
                string sErrMsg = "";
                BaseRepository Base = new BaseRepository();
                var model = new LineCommand();
                model.LineUserId = userId;
                model.Command = cmd;
                model.CreateDate = DateTime.Now;
                //更新回資料庫
                bool rslt = Base.Insert(model, ref sErrMsg);
            }
            catch (Exception e)
            {
                LineApi.SaveTextFile(e.Message);
            }
        }
        /// <summary>
        /// 更新正在操作的指令(圖片)
        /// </summary>
        public static void UpdateLastCommandDataImg(string userId, string cmd, string fieldName, string fieldValue)
        {
            try
            {
                string sErrMsg = "";
                BaseRepository Base = new BaseRepository();
                var model = Base.QueryData<LineCommand>(ref sErrMsg, x => x.LineUserId == userId)
                        .OrderByDescending(x => x.CreateDate)
                        .Take(1)
                        .SingleOrDefault();

                string strJson = model.Data;
                JObject newObj = new JObject();
                if (model.Data != null)
                {
                    newObj = JsonConvert.DeserializeObject<JObject>(strJson);
                    string tmp = newObj.GetValue(fieldName).ToString();
                    JArray arr = JsonConvert.DeserializeObject<JArray>(tmp);
                    arr.Add(fieldValue);
                    newObj.Remove(fieldName);
                    newObj.Add(fieldName, arr);
                }
                else
                {
                    JArray arrImg = new JArray();
                    arrImg.Add(fieldValue);
                    newObj.Add(fieldName, arrImg);
                }
                model.Data = JsonConvert.SerializeObject(newObj);
                model.Command = cmd;
                model.CreateDate = DateTime.Now;
                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
            }
            catch (Exception e)
            {
                LineApi.SaveTextFile(e.Message + ":" + e.ToString());
            }
        }

        /// <summary>
        /// 更新正在操作的指令
        /// </summary>
        public static void UpdateLastCommandData(string userId, string cmd, string fieldName, string fieldValue)
        {
            try
            {
                string sErrMsg = "";
                BaseRepository Base = new BaseRepository();
                var model = Base.QueryData<LineCommand>(ref sErrMsg, x => x.LineUserId == userId)
                        .OrderByDescending(x => x.CreateDate)
                        .Take(1)
                        .SingleOrDefault();

                string strJson = model.Data;
                JObject newObj = new JObject();
                if (model.Data != null)
                {
                    newObj = JsonConvert.DeserializeObject<JObject>(strJson);
                    newObj.Remove(fieldName);
                }
                newObj.Add(fieldName, fieldValue);
                model.Data = JsonConvert.SerializeObject(newObj);
                model.Command = cmd;
                model.CreateDate = DateTime.Now;
                //更新回資料庫
                bool rslt = Base.Update(ref sErrMsg);
            }
            catch (Exception e)
            {
                LineApi.SaveTextFile(e.Message + ":" + e.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 取資料,原則上是要加在service裡,但sql太複雜或找不到對應指令暫用此來做
        /// </summary>
        public static DataTable GetDataTable(string sql)
        {
            string connString = ConfigurationManager.ConnectionStrings["ICCSystemContext"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            DataTable dt = new DataTable();
            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dt);
            conn.Close();
            da.Dispose();
            return dt;
        }

    }
}