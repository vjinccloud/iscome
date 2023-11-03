 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysAccess_log")]
    public class sysAccess_log 
    { 
         /// <summary> 流水號ID
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert )] 
         public Int64 ID { get; set; } 
         /// <summary> 使用者代號
         /// 
         /// </summary> 
         [Column] 
         public String LoginID { get; set; } 
         /// <summary> 功能別
         /// 
         /// </summary> 
         [Column] 
         public String Path { get; set; } 
         /// <summary> 動作(增刪改查,或者拒絕與失敗)
         /// 
         /// </summary> 
         [Column] 
         public String Act { get; set; } 
         /// <summary> 登入IP
         /// 
         /// </summary> 
         [Column] 
         public String LoginIP { get; set; } 
         /// <summary> 備註
         /// 
         /// </summary> 
         [Column] 
         public String Remark { get; set; } 
         /// <summary> 建立日期
         /// 
         /// </summary> 
         [Column] 
         public DateTime CDate { get; set; } 
     } 
 } 
