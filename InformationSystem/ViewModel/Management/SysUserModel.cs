using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class SysUserModel
    {
        /// <summary>
        /// 定義列表
        /// </summary>
        public List<sysRole> defCodes { get; set; }

        /// <summary>
        /// 帳號列表
        /// </summary>
        public List<sysUserInfo> SysUsers { get; set; }

        /// <summary>
        /// 身分別條件
        /// </summary>
        public String Identify_Filter { get; set; }

        /// <summary>
        /// 帳號狀態條件
        /// </summary>
        public String Status_Filter { get; set; }

        /// <summary>
        /// 特權帳號條件
        /// </summary>
        public String Special_Filter { get; set; }

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