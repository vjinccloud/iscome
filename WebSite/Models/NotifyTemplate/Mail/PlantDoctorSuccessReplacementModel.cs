using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models.NotifyTemplate.Mail
{
    public class PlantDoctorSuccessReplacementModel
    {
        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string NameRecipient { get; set; }

        /// <summary>
        /// 預約日期
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
        /// 手機號
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 聯繫管道
        /// </summary>
        public string ContactStr { get; set; }

        /// <summary>
        /// 取消預約網址
        /// </summary>
        public string CancelReserveUrl { get; set; }
    }
}