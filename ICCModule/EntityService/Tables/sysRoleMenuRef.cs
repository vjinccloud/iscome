 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysRoleMenuRef")]
    public class sysRoleMenuRef 
    { 
         /// <summary> 角色代碼,對應到sysRole.RoleCode
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true )] 
         public String RoleCode { get; set; } 
         /// <summary> 功能選單ID,對應到sysMenu.MenuID
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true )] 
         public String Menu_ID { get; set; } 
         /// <summary> 可授權的操作,用|符號區隔(EX: |C|R|U|D|)
         /// 
         /// </summary> 
         [Column] 
         public String AllowList { get; set; } 
         /// <summary> 備註
         /// 
         /// </summary> 
         [Column] 
         public String Remark { get; set; } 
     } 
 } 
