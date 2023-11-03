using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class GroupInfoViewModel
    {
        /// <summary>
        /// 權限編碼
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 原權限列表
        /// </summary>
        public List<Permission> Permissions { get; set; }
    }
    public class userPermissionViewModel
    {
        /// <summary>
        /// 權限編碼
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 原權限列表
        /// </summary>
        public List<Permission> Permissions { get; set; }
    }
}