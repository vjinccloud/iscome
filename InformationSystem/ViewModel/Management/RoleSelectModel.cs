using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class RoleSelectModel
    {
        /// <summary>
        /// 序號
        /// </summary>
        public string RoleID { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 帳號數量
        /// </summary>
        public int RoleCount { get; set; }

    }
}