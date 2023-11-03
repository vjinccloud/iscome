using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Website.Models.NotifyTemplate.SMS;

namespace Website.Helper
{
    public class SmsReadFile
    {
        /// <summary>
        /// 讀取簡訊範本
        /// </summary>
        /// <param name="sTxtName">簡訊名稱</param>
        /// <param name="language">指定語系</param>
        /// <returns>範本郵件內容</returns>
        public string getSmsData(string sTxtName, string language)
        {
            //輸出
            string returnData = "";
            //實體路徑的html檔 = 應用程式所在的目錄 + 所在頁面
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\NotifyTemplate\SMS\" + language + @"_" + sTxtName + @".txt";
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
        /// 取代簡訊文字
        /// </summary>
        /// <param name="sTxtName">簡訊名稱</param>
        /// <param name="language">指定語系</param>
        /// <param name="mailReplaceData">輸入參數</param>
        /// <returns>完整郵件內容</returns>
        public string setReplacedSmsDataForPlantDoctorSuccess(string sTxtName, string language, PlantDoctorSuccessReplacementModel mailReplaceData)
        {
            //輸出
            string returnData = "";

            //讀取簡訊範本
            returnData = getSmsData(sTxtName, language);

            //取代文字
            if (mailReplaceData != null)
            {
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