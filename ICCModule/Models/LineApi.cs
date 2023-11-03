using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ICCModule.Models
{
    public static class LineApi
    {
        public static  string _ChannelAccessToken = "cXSsf/nMd4LxhPEhBEA63jSsbqcCO5vGgK5lJ++LXc1UhJcVoxO+fAVNTdGLQeU4eV7PROn5e50nOmGHsJmdXQGFYFDuBVu8J8nRDFO27q7EBZfyng324z9ltKtE/aTooaUbkjzAGsjG63ZSUbb5TwdB04t89/1O/w1cDnyilFU=";
        public static string recommand_friend_url = "https://line.me/R/nv/recommendOA/@382ctlwq";
        public static string disease_warning_news = "https://khpp.kcg.gov.tw/home/news";
        public static string find_job_friend = "https://liff.line.me/1645278921-kWRPP32q/?accountId=owl1337w";



        //上傳rich menu圖片用
        public static string PostFile(string url, string imgPath)
        {
            HttpClient _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(_ChannelAccessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _ChannelAccessToken);
            }
         
            //Add the file
            var fileStreamContent = new StreamContent(File.OpenRead(imgPath));
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
          
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
            var result = _httpClient.PostAsync(url, fileStreamContent);
            string res = result.Result.Content.ReadAsStringAsync().Result;
            return res;
        }

        //共用post method
        public static string SendPost(string method , string url ,string json = "")
        {
            HttpClient _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(_ChannelAccessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _ChannelAccessToken);
            }
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;

            System.Threading.Tasks.Task<HttpResponseMessage> result = null;
            if(method.ToLower() == "get")
            {
                result = _httpClient.GetAsync(url);
            }
            else if(method.ToLower() =="post")
            {
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                result = _httpClient.PostAsync(url, httpContent);
            }
            else if (method.ToLower() == "delete")
            {
                result = _httpClient.DeleteAsync(url);
            }

            string res = result.Result.Content.ReadAsStringAsync().Result;
            return res;
        }

        public static async void  SaveLineContent(string MessageId , string saveLocation)
        {
            string imageUrl = $"https://api-data.line.me/v2/bot/message/{MessageId}/content";

            byte[] imageBytes;
            HttpWebRequest imageRequest = (HttpWebRequest)WebRequest.Create(imageUrl);
            imageRequest.Headers["Authorization"] = $" Bearer {_ChannelAccessToken}";
            WebResponse imageResponse = imageRequest.GetResponse();
            Stream responseStream = imageResponse.GetResponseStream();
            using (BinaryReader br = new BinaryReader(responseStream))
            {
                imageBytes = br.ReadBytes(500000);
                br.Close();
            }
            responseStream.Close();
            imageResponse.Close();
            FileStream fs = new FileStream(saveLocation, FileMode.Create);
            //BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                //bw.Write(imageBytes);
                await fs.WriteAsync(imageBytes, 0 , imageBytes.Length);
            }
            finally
            {
                fs.Close();
                //bw.Close();
            }

        }


        //選單
        public static string AddMenu(string token, string userId , bool isLogin)
        {
            JObject model = new JObject();
            model.Add("selected", true);
            if (isLogin)
            {
                model.Add("name", "menu B");
            }
            else
            {
                model.Add("name", "menu A");
            }
            model.Add("chatBarText", "選單");

            JObject size = new JObject();
            size.Add("width", 2500);
            size.Add("height", 1686);

            model.Add("size", size);

            JArray areas = new JArray();
            if(isLogin)
            {
                areas.Add(GetArea(17, 17, 827, 836, "會員專區"));
            }
            else
            {
                areas.Add(GetArea(17, 17, 827, 836, "會員註冊/綁定"));
            }
            
            areas.Add(GetArea(8, 870, 815, 773, "我要掛號找植醫"));

            areas.Add(GetArea(849, 21, 798, 528, "植物醫師據點"));
            areas.Add(GetArea(853, 579, 802, 532, "我的電子處方箋"));
            areas.Add(GetAreaUri(845, 1140, 810, 532, "推薦好友", recommand_friend_url));

            areas.Add(GetAreaUri(1681, 17, 811, 536, "疫情預警資訊", disease_warning_news));
            areas.Add(GetAreaUri(1681, 583, 802, 540, "農業缺工請找我", find_job_friend));
            areas.Add(GetArea(1672, 1149, 820, 528, "農藥與產銷資訊"));
            
            model.Add("areas", areas);

            string json = string.Empty;
            if (json != null)
            {
                json = JsonConvert.SerializeObject(model);
            }
            string result = SendPost("post","https://api.line.me/v2/bot/richmenu", json);
            SaveTextFile(result);

            return result;
        }

        public static JObject GenUriAction(string type, string label, string uri)
        {
            JObject action = new JObject();
            action.Add("type", type);
            action.Add("label", label);
            action.Add("uri", uri);

            return action;
        }

        //簡化操作func
        public static JObject GetArea(int x, int y, int width, int height, string name)
        {
            JObject area = new JObject();
            area.Add("action", GenAction("postback", name, name));

            JObject bounds = new JObject();
            bounds.Add("x", x);
            bounds.Add("y", y);
            bounds.Add("width", width);
            bounds.Add("height", height);
            area.Add("bounds", bounds);

            return area;
        }

        //簡化操件func
        public static JObject GetAreaUri(int x, int y, int width, int height, string name, string uri)
        {
            JObject area = new JObject();
            area.Add("action", GenAction("uri", "", uri));

            JObject bounds = new JObject();
            bounds.Add("x", x);
            bounds.Add("y", y);
            bounds.Add("width", width);
            bounds.Add("height", height);
            area.Add("bounds", bounds);

            return area;
        }
        
        public static void template_carosuel_test(string token)
        {
            JArray columns = new JArray();
            columns.Add(GenColumn("第一頁","內文","", new string[]{ "postback,動作1,動作1",
                                                                "postback,動作2,動作2",
                                                                "postback,動作3,動作3" }));

            columns.Add(GenColumn("第二頁", "內文","", new string[]{ "postback,動作1,動作1",
                                                                "postback,動作2,動作2",
                                                                "postback,動作3,動作3" }));

            ReplyCarouselTmp(token , columns);
        }

        public static void template_buttons_test(string token)
        {
            JArray actions = new JArray();
            actions.Add(GenAction("uri", "線上掛號說明", "https://www.christophe.tw/lineaccount/description"));

            ReplyButtonsTmp(token , "我的頁面","說明" , actions);
        }

        public static void ReplyText(string token, string msg)
        {
            JObject item = new JObject();
            item.Add("type", "text");
            item.Add("text", msg);
            ReplyBase(token, item);
        }

        public static void ReplySticker(string token )
        {
            JObject item = new JObject();
            item.Add("type", "sticker");
            item.Add("packageId", "446");
            item.Add("stickerId", "1988");

            ReplyBase(token, item);
        }

        public static void ReplayImg(string token , string originImg, string previewImg)
        {
            JObject item = new JObject();
            item.Add("type", "image");
            item.Add("originalContentUrl", originImg);
            item.Add("previewImageUrl", previewImg);

            ReplyBase(token, item);
        }

        public static void ReplayVideo(string token , string video_url, string previewImg)
        {
            JObject item = new JObject();
            item.Add("type", "video");
            item.Add("originalContentUrl", video_url);
            item.Add("previewImageUrl", previewImg);
            item.Add("trackingId", "");

            ReplyBase(token, item);
        }

        public static void ReplayLocation(string token , string title , string addr , string latitude , string longitude)
        {
            JObject item = new JObject();
            item.Add("type", "location");
            item.Add("title", title);
            item.Add("address", addr);
            item.Add("latitude", latitude);
            item.Add("longitude", longitude);

            ReplyBase(token, item);
        }

        public static string GetMenuList()
        {
            string res = SendPost("get","https://api.line.me/v2/bot/richmenu/list");
            SaveTextFile(res);
            return res;
        }

        public static string DelMenu(string richMenuId)
        {
            string res = SendPost("delete", $"https://api.line.me/v2/bot/richmenu/{richMenuId}");
            SaveTextFile(res);
            return res;
        }

        public static void DelAllMenus()
        {
            string strJson = GetMenuList();
            JObject menu = JsonConvert.DeserializeObject<JObject>(strJson);
            foreach (JObject item in menu.GetValue("richmenus"))
            {
                string menuId = item.GetValue("richMenuId").ToString();
                DelMenu(menuId);
            }
        }

        public static void SetMenu(string richMenuId)
        {
            string json = string.Empty;
            string result = SendPost("post",$"https://api.line.me/v2/bot/user/all/richmenu/{richMenuId}", json);
            SaveTextFile(result);
        }

        //menu A :綁定前選單
        //menu B :綁定後選單
        public static void SetUserMenu(string userId, bool isBinded)
        {
            string strJson = GetMenuList();
            JObject menu = JsonConvert.DeserializeObject<JObject>(strJson);
            foreach (JObject item in menu.GetValue("richmenus"))
            {
                string menuId =  item.GetValue("richMenuId").ToString();
                string menuName = item.GetValue("name").ToString();
                if(menuName == "menu A")
                {
                    if(!isBinded)
                    {
                        string json = string.Empty;
                        string result = SendPost("post", $"https://api.line.me/v2/bot/user/{userId}/richmenu/{menuId}", json);
                        SaveTextFile(result);
                    }
                       
                }
                else if (menuName == "menu B")
                {
                    if (isBinded)
                    {
                        string json = string.Empty;
                        string result = SendPost("post", $"https://api.line.me/v2/bot/user/{userId}/richmenu/{menuId}", json);
                        SaveTextFile(result);
                    }
                }
                 
            }
        }


        public static void UploadMenu(string token, string richMenuId, string imgPath)
        {
            string fullpath = System.Web.HttpRuntime.AppDomainAppPath + imgPath;
            string json = string.Empty;

            string result = PostFile($"https://api-data.line.me/v2/bot/richmenu/{richMenuId}/content", fullpath);
            SaveTextFile(result);
        }

        //line回覆的temple中的button樣式, 每template最多4個action
        public static void ReplyButtonsTmp(string token , string title , string text, JArray actions)
        {
            JObject message = new JObject();
            message.Add("type", "template");
            message.Add("altText", "this is a carousel template");

            JObject template = new JObject();
            template.Add("type", "buttons");
            if(title != "")
            {
                template.Add("title", title);
            }
            template.Add("text", text);
            template.Add("actions", actions);
            message.Add("template", template);

            ReplyBase(token, message);
        }

        //line回覆的temple中的carusel樣式, 每column最多3個action
        public static void ReplyCarouselTmp(string token ,JArray columns)
        {
            JObject message = new JObject();
            try
            {
                message.Add("type", "template");
                message.Add("altText", "this is a carousel template");

                JObject template = new JObject();
                template.Add("type", "carousel");
                template.Add("columns", columns);
                message.Add("template", template);
            }
            catch (Exception ex)
            {
                SaveTextFile(ex.Message +":"+ ex.ToString());
            }
            ReplyBase(token, message);
        }

        public static JObject GenColumn(string title, string text , string img, string[] actionList)
        {
            JObject column1 = new JObject();

            try
            {
                column1.Add("title", title);
                column1.Add("text", text);
                if(img != "")
                {
                    column1.Add("thumbnailImageUrl", img);
                    column1.Add("imageBackgroundColor", "#FFFFFF");
                }

                JArray actionList1 = new JArray();
                foreach (string str in actionList)
                {
                    string[] arr = str.Split(',');
                    actionList1.Add(GenAction(arr[0], arr[1], arr[2]));
                }
                column1.Add("actions", actionList1);
            }
            catch(Exception ex)
            {
                SaveTextFile(ex.Message);
            }
            return column1;
        }

        //回覆messages時,固定結構,傳入單個message
        public static void ReplyBase(string token, JObject MesObj)
        {
            JArray arr = new JArray();
            arr.Add(MesObj);

            JObject model = new JObject();
            model.Add("replyToken", token);
            model.Add("messages", arr);
            string json = string.Empty;
            if (json != null)
            {
                json = JsonConvert.SerializeObject(model);
            }
            SaveTextFile(json);

            string result = SendPost("post","https://api.line.me/v2/bot/message/reply", json);
            SaveTextFile(result);
        }

        //回覆messages時,固定結構,傳入message array
        public static void ReplyBase(string token, JArray arr)
        {
            //JArray arr = new JArray();
            //arr.Add(MesObj);

            JObject model = new JObject();
            model.Add("replyToken", token);
            model.Add("messages", arr);
            string json = string.Empty;
            if (json != null)
            {
                json = JsonConvert.SerializeObject(model);
            }
            SaveTextFile(json);

            string result = SendPost("post", "https://api.line.me/v2/bot/message/reply", json);
            SaveTextFile(result);
        }

        //簡化action的產生
        public static JObject GenAction(string type, string label, string data)
        {
            JObject action = new JObject();
            action.Add("type", type);

            if (label != "")
            {
                action.Add("label", label);
            }
            if (type == "uri")
            {
                action.Add("uri", data);
            }
            else if (type == "message")
            {
                action.Add("text", data);
            }
            else
            {
                action.Add("data", data);
            }
            return action;
        }

        public static void  SaveTextFile(string log)
        {
            try
            {
                string root = "";
                //判斷呼叫的專案檔是否為Web專案?
                if (System.Web.HttpContext.Current != null)
                {
                    //網頁版本的路徑取法
                    root = System.Web.HttpRuntime.AppDomainAppPath;
                }
                else
                {
                    //視窗程式或主控台應用程式叫法
                    root = System.IO.Directory.GetCurrentDirectory();
                }
                string DirPath = root + "\\ErrorLog\\";
                //檢查資料夾路徑
                //HttpRuntime.AppDomainAppPath
                //System.Web.HttpContext.Current.Server
                bool dir_exist = System.IO.Directory.Exists(DirPath);
                //若沒有則新增資料夾
                if (dir_exist == false)
                    System.IO.Directory.CreateDirectory(DirPath);
                //路徑
                string path = DirPath + "Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
                {
                    sw.WriteLine(log);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}