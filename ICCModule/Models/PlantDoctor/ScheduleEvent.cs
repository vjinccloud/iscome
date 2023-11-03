using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ICCModule.Models.PlantDoctor
{
    public class ScheduleEvent
    {
        /// <summary>
        /// 用於 Html Element ID attribute
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 開始日期
        /// </summary>
        public string start { get; set; }

        /// <summary>
        /// 結束日其
        /// </summary>
        public string end { get; set; }

        /// <summary>
        /// 事件顯示標題
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 事件 Html Element Class attribute
        /// </summary>
        public string classNames { get; set; }

        /// <summary>
        /// 將排程內容放於此
        /// </summary>
        public doctorDutyRecord extendedProps { get; set; }
    }
}