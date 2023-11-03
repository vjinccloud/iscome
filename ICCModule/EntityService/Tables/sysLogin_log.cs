 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysLogin_log")]
    public class sysLogin_log 
    { 
         /// <summary> 流水號
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert )] 
         public Int64 ID { get; set; } 
         /// <summary> 登入帳號
         /// 
         /// </summary> 
         [Column] 
         public String LoginID { get; set; } 
         /// <summary> 來源IP
         /// 
         /// </summary> 
         [Column] 
         public String LoginIP { get; set; } 
         /// <summary> 時間
         /// 
         /// </summary> 
         [Column] 
         public DateTime CDate { get; set; } 
         /// <summary> 登入驗證結果
         /// 
         /// </summary> 
         [Column] 
         public String Record { get; set; } 
     } 
 } 
