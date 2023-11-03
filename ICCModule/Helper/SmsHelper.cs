using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using ICCModule.Helper;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IscomG2C.Utility;

namespace ICCModule.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class SmsHelper
    {
        static string smsUrl = "http://api.twsms.com/json/sms_send.php";
        /// <summary>寄信(直接從Web.Config取得相關參數 僅需要指定 收件者 主題 內容 即可)
        /// 
        /// </summary>
        /// <param name="sErrMsg"></param>
        /// <param name="TargetMail">目標信箱</param>
        /// <param name="Subject">主題</param>
        /// <param name="MailBody">內文(含HTML語法)</param>
        /// <returns></returns>
        public static SendResponse SendSms(SmsReq req)
        {
            var _response = new SendResponse();
            try
            {                
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var _url = $"{smsUrl}?username={req.UserName}&password={req.LoginPass}&mobile={req.Mobile}&message={HttpUtility.UrlEncode(req.Message)}";

                    var res = client.GetStringAsync(new Uri(_url));
                    var response = JsonConvert.DeserializeObject<SmsResponse>(res.Result);

                    if (response.code == "00000")
                    {
                        _response.IsSuccess = true;
                        _response.Msg = "發送成功";
                    }
                    else
                    {
                        ErrorLog.SaveErrorLog("發送失敗:"+ response.text, "SmsHelper");
                        _response.Msg = response.text;
                    }
                }
            }
            catch (Exception ex)
            {
                _response.Msg = "寄件錯誤!" + ex.ToString();
                ErrorLog.SaveErrorLog(ex, "SmsHelper");
            }
            return _response;
        }
    }
    public class SmsReq
    {
        public string UserName { get; set; } = "1100809kcg";
        public string LoginPass { get; set; } = "1101130kcg";
        public string Mobile { get; set; }
        public string Message { get; set; }
    }
    public class SmsResponse
    {
        public string code { get; set; }
        public string text { get; set; }
        public int msgid { get; set; }
    }
    public class SendResponse
    {
        public bool IsSuccess { get; set; } = false;
        public string Msg { get; set; }
    }


}
