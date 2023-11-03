 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysAnnouncement")]
    public class sysAnnouncement 
    { 
         /// <summary> 流水號
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert )] 
         public Int64 PID { get; set; } 
         /// <summary> 建立日期
         /// 
         /// </summary> 
         [Column] 
         public DateTime CreateTime { get; set; } 
         /// <summary> 更新日期
         /// 
         /// </summary> 
         [Column] 
         public DateTime UpdateTime { get; set; } 
         /// <summary> 發佈日期-起
         /// 
         /// </summary> 
         [Column] 
         public DateTime? PublishTime_Beg { get; set; } 
         /// <summary> 發佈日期-迄
         /// 
         /// </summary> 
         [Column] 
         public DateTime? PublishTime_End { get; set; } 
         /// <summary> 標題
         /// 
         /// </summary> 
         [Column] 
         public String Title { get; set; } 
         /// <summary> 內文
         /// 
         /// </summary> 
         [Column] 
         public String Content { get; set; } 
         /// <summary> 是否停用
         /// 
         /// </summary> 
         [Column] 
         public bool isDisabled { get; set; } 
         /// <summary> 是否為全系統公告?
         /// 
         /// </summary> 
         [Column] 
         public bool isGlobal { get; set; } 
         /// <summary> 所屬群組清單
         /// 
         /// </summary> 
         [Column] 
         public String GroupList { get; set; } 
         /// <summary> 所屬使用者清單
         /// 
         /// </summary> 
         [Column] 
         public String UserList { get; set; } 
     } 
 } 
