 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysUserRoleRef")]
    public class sysUserRoleRef 
    { 
         /// <summary> 使用者帳號
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true )] 
         public String LoginID { get; set; } 
         /// <summary> 角色代碼,對應到sysRole.RoleCode
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true )] 
         public String RoleCode { get; set; } 
         /// <summary> 備註
         /// 
         /// </summary> 
         [Column] 
         public String Remark { get; set; } 
     } 
 } 
