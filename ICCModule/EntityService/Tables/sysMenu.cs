 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysMenu")]
    public class sysMenu 
    { 
         /// <summary> 功能選單ID
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true )] 
         public String MenuID { get; set; } 
         /// <summary> 功能選單父節點ID
         /// 
         /// </summary> 
         [Column] 
         public String MenuParentID { get; set; } 
         /// <summary> 功能選單名稱
         /// 
         /// </summary> 
         [Column] 
         public String Name { get; set; } 
         /// <summary> 選單路徑
         /// 
         /// </summary> 
         [Column] 
         public String Path { get; set; } 
         /// <summary> 排序值
         /// 
         /// </summary> 
         [Column] 
         public Int32 Sort { get; set; } 
         /// <summary> 節點的層級,大選單1,次選單2
         /// 
         /// </summary> 
         [Column] 
         public Int32 Level { get; set; } 
         /// <summary> 可以操作的動作代碼,用|符號區隔,C:新增,R:讀取,U:修改,D:刪除,E:匯出
         /// 
         /// </summary> 
         [Column] 
         public String AllowList { get; set; } 
         /// <summary> 
         /// 
         /// </summary> 
         [Column] 
         public String iconURL { get; set; } 
     } 
 } 
