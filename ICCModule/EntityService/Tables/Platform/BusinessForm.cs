using ICCModule.EntityService.Service;
using IscomG2C.Utility;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "BusinessForms")]
    public class BusinessForm
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary>
        /// 多選權限，透過 "," 號進行分割
        /// </summary>
        /// <value>
        /// The codes.
        /// </value>
        [Column]
        public string Codes { get; set; }

        /// <summary> 檔案顯示名稱
        /// 
        /// </summary> 
        [Column]
        public string FileName { get; set; }

        /// <summary> 檔案保存路徑
        /// 
        /// </summary> 
        [Column]
        public string FilePath { get; set; }

        /// <summary> 顯示狀態
        /// 
        /// </summary> 
        [Column]
        public bool Show { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column(IsDbGenerated = true)]
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        [DataType(DataType.Date)]
        public DateTime? UpdatedAt { get; set; }


        /// <summary>
        /// Gets the name of the type from defCode Table
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public List<string> PermissionNames
        {
            get
            {
                if (Codes == null)
                {
                    return null;
                }

                List<string> codeList = Codes.Split(',').ToList();
                List<string> list = new List<string>();
                foreach (string code in codeList)
                {
                    list.Add(Service_sysRole.GetDetail(code)?.RoleName ?? "");
                }

                return list.Where(x => !string.IsNullOrEmpty(x)).ToList();
            }

            set
            {
                Codes = value != null ? String.Join(",", value.ToArray()) : null;                
            }
        }

        /// <summary>
        /// 取回民國年的建立日期
        /// </summary>
        public string CreatedAtStr
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(CreatedAt, "yyy/M/dd"); }
            set { CreatedAt = Utility_DateTime.ParseExact(value) ?? DateTime.Now; }
        }

        /// <summary>
        /// 取回民國年的更新日期
        /// </summary>
        public string UpdatedAtStr
        {
            get { return Utility_DateTime.ToFormat_inTaiwanYear(UpdatedAt, "yyy/M/dd"); }
            set { UpdatedAt = Utility_DateTime.ParseExact(value) ?? DateTime.Now; }
        }
    }
}
