using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Website.Models.NotifyTemplate.Mail;

namespace Website.Helper
{
    public class MailReadFile
    {
        /// <summary>
        /// 讀取信件範本
        /// </summary>
        /// <param name="sHtmlName">郵件名稱</param>
        /// <param name="language">指定語系</param>
        /// <returns>範本郵件內容</returns>
        public string getEmailData(string sHtmlName, string language)
        {
            //輸出
            string returnData = "";
            //實體路徑的html檔 = 應用程式所在的目錄 + 所在頁面
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\NotifyTemplate\Mail\" + language + @"_" + sHtmlName + @".html";
            //有資料
            if (File.Exists(path))
            {
                //讀取檔案
                StreamReader streamReader = new StreamReader(path, Encoding.GetEncoding("Big5"));
                //轉字串給輸出
                returnData = streamReader.ReadToEnd();
            }
            return returnData;
        }

        /// <summary>
        /// 取代郵件文字
        /// </summary>
        /// <param name="sHtmlName">郵件名稱</param>
        /// <param name="language">指定語系</param>
        /// <param name="mailReplaceData">輸入參數</param>
        /// <returns>完整郵件內容</returns>
        public string setReplacedEmailDataForPlantDoctorSuccess(string sHtmlName, string language, PlantDoctorSuccessReplacementModel mailReplaceData)
        {
            //輸出
            string returnData = "";

            //讀取信件範本
            returnData = getEmailData(sHtmlName, language);

            //取代文字
            if (mailReplaceData != null)
            {
                if (!string.IsNullOrEmpty(mailReplaceData.NameRecipient))
                {
                    returnData = Regex.Replace(returnData, "#NameRecipient#", mailReplaceData.NameRecipient);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.Date))
                {
                    returnData = Regex.Replace(returnData, "#Date#", mailReplaceData.Date);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.Weekday))
                {
                    returnData = Regex.Replace(returnData, "#Weekday#", mailReplaceData.Weekday);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.Time))
                {
                    returnData = Regex.Replace(returnData, "#Time#", mailReplaceData.Time);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.Mobile))
                {
                    returnData = Regex.Replace(returnData, "#Mobile#", mailReplaceData.Mobile);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.ContactStr))
                {
                    returnData = Regex.Replace(returnData, "#ContactStr#", mailReplaceData.ContactStr);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.CancelReserveUrl))
                {
                    returnData = Regex.Replace(returnData, "#CancelReserveUrl#", mailReplaceData.CancelReserveUrl);
                }
                //...無限向下增加
            }
            return returnData;
        }
    }
}