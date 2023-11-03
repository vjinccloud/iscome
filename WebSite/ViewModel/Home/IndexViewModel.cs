using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.Models.PreventionInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.ViewModel.Home
{
    public class IndexViewModel
    {
        /// <summary>
        /// 搜尋Tag列表
        /// </summary>
        public List<Tag> Tags { get; set; }
        public List<defCode> NewsType { get; set; }
        public List<New> News { get; set; }
        public string MemberInfo { get; set; }
        /// <summary>
        /// 相關網站
        /// </summary>
        public List<Link> Links { get; set; }
        public string RememberAcc { get; set; }
        /// <summary>
        /// 植物疫情
        /// </summary>
        public List<PestNotice> PestNotices { get; set; }
        public New PopupNews { get; set; }
    }
}