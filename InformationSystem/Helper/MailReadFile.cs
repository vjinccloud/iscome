using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using InformationSystem.Models;

namespace InformationSystem.Helper
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
        public string setReplacedEmailData(string sHtmlName, string language, SignUpEmailReplaceModel mailReplaceData)
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
                if (!string.IsNullOrEmpty(mailReplaceData.AccountVerifyUrl))
                {
                    returnData = Regex.Replace(returnData, "#AccountVerifyUrl#", mailReplaceData.AccountVerifyUrl);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.WebsiteUrl))
                {
                    returnData = Regex.Replace(returnData, "#WebsiteUrl#", mailReplaceData.WebsiteUrl);
                }
                //...無限向下增加
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
        public string setReplacedEmailDataForAdmin(string sHtmlName, string language, AdminEmailReplaceModel mailReplaceData)
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
                if (!string.IsNullOrEmpty(mailReplaceData.Account))
                {
                    returnData = Regex.Replace(returnData, "#Account#", mailReplaceData.Account);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.LoginPass))
                {
                    returnData = Regex.Replace(returnData, "#Password#", mailReplaceData.LoginPass);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.WebsiteUrl))
                {
                    returnData = Regex.Replace(returnData, "#WebsiteUrl#", mailReplaceData.WebsiteUrl);
                }
                //...無限向下增加
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
        public string setReplacedEmailDataForForgot(string sHtmlName, string language, ForgetEmailReplaceModel mailReplaceData)
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
                if (!string.IsNullOrEmpty(mailReplaceData.ApplicationTime))
                {
                    returnData = Regex.Replace(returnData, "#ApplicationTime#", mailReplaceData.ApplicationTime);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.ResetPasswordUrl))
                {
                    returnData = Regex.Replace(returnData, "#ResetPasswordUrl#", mailReplaceData.ResetPasswordUrl);
                }
                //...無限向下增加
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
        public string setReplacedEmailDataForMember(string sHtmlName, string language, MemberEmailReplaceModel mailReplaceData)
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
                if (!string.IsNullOrEmpty(mailReplaceData.Account))
                {
                    returnData = Regex.Replace(returnData, "#Account#", mailReplaceData.Account);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.LoginPass))
                {
                    returnData = Regex.Replace(returnData, "#Password#", mailReplaceData.LoginPass);
                }
                if (!string.IsNullOrEmpty(mailReplaceData.WebsiteUrl))
                {
                    returnData = Regex.Replace(returnData, "#WebsiteUrl#", mailReplaceData.WebsiteUrl);
                }
                //...無限向下增加
            }
            return returnData;
        }

    }
}