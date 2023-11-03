using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel
{
    public class NotifyStatModel
    {
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string KeyWord { get; set; }
        /// <summary>
        /// 次數
        /// </summary>
        public int Counts { get; set; } = 0;
    }
    public class NotifyStatExcelModel
    {
        public int 排行順序 { get; set; }
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string 關鍵字 { get; set; }
        /// <summary>
        /// 次數
        /// </summary>
        public int 計次 { get; set; } = 0;
    }
}