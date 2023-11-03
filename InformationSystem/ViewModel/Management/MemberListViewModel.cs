using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class MemberListViewModel
    {
        /// <summary>
        /// 角色別
        /// </summary>
        public List<defCode> defCodes { get; set; } = Service_defCode.GetList("UserIdentify", "N");

        /// <summary>
        /// 帳號列表
        /// </summary>
        public List<memberInfo> Members { get; set; }

        /// <summary>
        /// 身分別條件
        /// </summary>
        public string Identify_Filter { get; set; }

        /// <summary>
        /// 帳號狀態條件
        /// </summary>
        public string Status_Filter { get; set; }

        /// <summary>
        /// 註冊來源
        /// </summary>
        public string RegistFrom { get; set; }

        /// <summary>
        /// 當前頁數
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// 當前顯示筆數
        /// </summary>
        public int PageCounts { get; set; }
    }
}