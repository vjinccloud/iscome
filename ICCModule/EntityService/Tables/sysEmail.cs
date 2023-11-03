 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysEmail")]
    public class sysEmail 
    { 
         /// <summary> 信件ID
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert )] 
         public Int32 Email_ID { get; set; } 
         /// <summary> 收件者信箱
         /// 
         /// </summary> 
         [Column] 
         public String mailTo { get; set; } 
         /// <summary> 送信者名稱
         /// 
         /// </summary> 
         [Column] 
         public String name { get; set; } 
         /// <summary> 主旨
         /// 
         /// </summary> 
         [Column] 
         public String subject { get; set; } 
         /// <summary> 信件內容
         /// 
         /// </summary> 
         [Column] 
         public String Email_content { get; set; } 
         /// <summary> 是否已經寄送?
         /// 
         /// </summary> 
         [Column] 
         public bool? IS_Send { get; set; } 
         /// <summary> 送信日期
         /// 
         /// </summary> 
         [Column] 
         public DateTime? Send_Date { get; set; } 
         /// <summary> 更新日期
         /// 
         /// </summary> 
         [Column] 
         public DateTime? Update_Date { get; set; } 
         /// <summary> 寄送是否成功?
         /// 
         /// </summary> 
         [Column] 
         public bool isSuccess { get; set; } 
         /// <summary> 信件錯誤訊息
         /// 
         /// </summary> 
         [Column] 
         public String ErrorMsg { get; set; } 
     } 
 } 
