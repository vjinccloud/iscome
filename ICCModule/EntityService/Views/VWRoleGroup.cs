using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICCModule.Entity.Tables
{
    [Serializable]
    [Table(Name = "VW_Role_Group")]
    public class VWRoleGroup
    {
        /// <summary>
        /// 對應 sysRole 的 RoleCode
        /// </summary>
        [Column]
        public string RoleID { set; get; }

        /// <summary>
        /// 對應 sysRole 的 RoleName
        /// </summary>
        [Column]
        public string RoleName { set; get; }

        /// <summary>
        /// 計算 sysUserInfo 的角色數量
        /// </summary>
        [Column]
        public int RoleCount { get; set; }
    }
}
