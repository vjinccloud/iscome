using ICCModule.EntityService.Service;
using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "Role_Has_Permissions")]
    public class RoleHasPermission
    {
        /// <summary>
        /// 對應權限ID
        /// </summary> 
        [Column(IsPrimaryKey =true)]
        public int PermissionId { get; set; }

        /// <summary>
        /// 對應 sysRole 的 RoleCode
        /// </summary> 
        [Column(IsPrimaryKey =true)]
        public string RoleCode { get; set; }

        /// <summary>
        /// 取得 Permission 相關資料
        /// </summary>
        public Permission Permission
        {
            get 
            {
                return Service_Permission.GetDetail(PermissionId);
            }
        }

        /// <summary>
        /// 取得 Role 相關資料
        /// </summary>
        public sysRole Role
        {
            get
            {
                return Service_sysRole.GetDetail(RoleCode);
            }
        }
    }
}
