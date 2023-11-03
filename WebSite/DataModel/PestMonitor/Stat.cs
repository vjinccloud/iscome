using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.DataModel.PestMonitor
{
    /// <summary>
    /// 統計資料
    /// </summary>
    public class Stat
    {
        /// <summary>
        /// 村里別
        /// </summary>
        public string Village { get; set; }

        /// <summary>
        /// 害蟲數量
        /// </summary>
        public decimal PestNumbers { get; set; }

    }
}