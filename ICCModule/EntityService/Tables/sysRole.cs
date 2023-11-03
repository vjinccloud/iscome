 using System; 
 using System.Collections.Generic; 
 using System.Data.Linq.Mapping; 
 using System.Text; 

 namespace ICCModule.Entity.Tables 
 { 

    [Serializable]
    [Table(Name = "sysRole")]
    public class sysRole 
    { 
         /// <summary> 角色代碼
         /// 
         /// </summary> 
         [Column( IsPrimaryKey = true )] 
         public String RoleCode { get; set; } 
         /// <summary> 角色名稱
         /// 
         /// </summary> 
         [Column] 
         public String RoleName { get; set; } 
         /// <summary> 是否為系統管理者?直接無條件
         /// 
         /// </summary> 
         [Column] 
         public bool isAdmin { get; set; }
        /// <summary> 是否啟用
        /// 
        /// </summary> 
        [Column]
        public string isActive { get; set; }
        /// <summary> 建立者
        /// 
        /// </summary> 
        [Column]
        public String CreateUser { get; set; }
        /// <summary> 建立日期
        /// 
        /// </summary> 
        [Column]
        public DateTime CreateDate { get; set; }
        /// <summary> 編輯者
        /// 
        /// </summary> 
        [Column]
        public String UpdateUser { get; set; }
        /// <summary> 編輯日期
        /// 
        /// </summary> 
        [Column]
        public DateTime UpdateDate { get; set; }
    } 
 } 
