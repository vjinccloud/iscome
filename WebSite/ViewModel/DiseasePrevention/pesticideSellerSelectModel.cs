using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel
{
    public class pesticideSellerSelectModel : pesticideSeller
    {
        /// <summary>
        /// 狀態
        /// </summary> 
        public string StatusString { get; set; }
        /// <summary>
        /// 狀態
        /// </summary> 
        public string UpdateString { get; set; }
    }
}