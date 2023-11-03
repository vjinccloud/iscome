using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ICCModule.EntityService.Views;

namespace Website.DataModel.PestMonitor
{
    /// <summary>
    /// 資料模型 病蟲害監測資料
    /// </summary>
    [Serializable]
    public class PestMonitorData
    {
        /// <summary>
        /// 專案名稱
        /// </summary>
        public string ProjName { get; set; }

        /// <summary>
        /// 各點位資料
        /// </summary>
        public List<VW_PestMonitorDetail> Points { get; set; }

        /// <summary>
        /// 統計資料
        /// </summary>
        public List<Stat> Stats { get; set; }
    }
}