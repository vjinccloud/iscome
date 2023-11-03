using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models.NotifyTemplate.SMS
{
    public class PlantDoctorSuccessReplacementModel
    {
        /// <summary>
        /// 台灣年月日
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 星期 ? (國字)
        /// </summary>
        public string Weekday { get; set; }

        /// <summary>
        /// 時間
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 取消預約網址
        /// </summary>
        public string CancelReserveUrl { get; set; }
    }
}