 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "VW_FuntionMenu_LV2")]
    public class VW_FuntionMenu_LV2 
    { 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public String MenuID { get; set; } 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public String MenuParentID { get; set; } 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public String Name { get; set; } 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public String Path { get; set; } 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public Int32 Sort { get; set; } 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public Int32 Level { get; set; } 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public String RoleCode { get; set; } 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public String AllowList { get; set; } 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public String LoginID { get; set; } 
     } 
 } 
