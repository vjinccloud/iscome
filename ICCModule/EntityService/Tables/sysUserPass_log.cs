 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysUserPass_log")]
    public class sysUserPass_log 
    { 
         /// <summary> 資料流水號
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert )] 
         public Int64 id { get; set; } 
         /// <summary> 登入者ID
         /// 
         /// </summary> 
         [Column] 
         public String LoginID { get; set; } 
         /// <summary> 密碼
         /// 
         /// </summary> 
         [Column] 
         public String LoginPW { get; set; } 
         /// <summary> 建立時間
         /// 
         /// </summary> 
         [Column] 
         public DateTime created_at { get; set; } 
     } 
 } 
