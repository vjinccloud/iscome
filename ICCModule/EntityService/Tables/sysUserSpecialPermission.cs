 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysUserSpecialPermission")]
    public class sysUserSpecialPermission
    { 
         /// <summary> 
         /// 登入者ID
         /// </summary> 
         [Column(IsPrimaryKey = true)] 
         public string sysUserId { get; set; }
        /// <summary> 
        /// 權限流水號
        /// </summary> 
        [Column(IsPrimaryKey = true)]
        public int PermissionId { get; set; }
     } 
 } 
