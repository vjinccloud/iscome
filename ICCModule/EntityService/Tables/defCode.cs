 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables
{ 

    [Serializable]
    [Table(Name = "defCode")]
    public class defCode : IComparable<defCode>
    { 
         /// <summary> 類型碼
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true )] 
         public String Kind { get; set; } 
         /// <summary> 類型名稱
         /// 
         /// </summary> 
         [Column] 
         public String KindName { get; set; } 
         /// <summary> 代碼
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true )] 
         public String Code { get; set; } 
         /// <summary> 代碼名稱
         /// 
         /// </summary> 
         [Column] 
         public String Name { get; set; } 
         /// <summary> 是否停用 Y:停用 N:未停用
         /// 
         /// </summary> 
         [Column] 
         public String NoUse { get; set; } 
         /// <summary> 排序值 棄用
         /// 
         /// </summary> 
         [Column] 
         public Int32 Sort { get; set; } 
         /// <summary> 備註
         /// 
         /// </summary> 
         [Column] 
         public String Remark { get; set; } 
         /// <summary> 允許編輯 Y=允許 N=不允許
         /// 
         /// </summary> 
         [Column] 
         public String AllowUpdate { get; set; } 
         /// <summary> 建立人員
         /// 
         /// </summary> 
         [Column] 
         public String CreateUser { get; set; } 
         /// <summary> 建立日期
         /// 
         /// </summary> 
         [Column] 
         public DateTime CreateDate { get; set; } 
         /// <summary> 更新人員
         /// 
         /// </summary> 
         [Column] 
         public String UpdateUser { get; set; } 
         /// <summary> 更新日期
         /// 
         /// </summary> 
         [Column] 
         public DateTime UpdateDate { get; set; } 

         public int CompareTo(defCode compareDefCode)
         {
            if (compareDefCode == null)
            {
                return 1;
            }
            else
            {
                return Sort.CompareTo(compareDefCode.Sort);
            }
         }
     } 
 } 
