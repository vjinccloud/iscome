using IscomG2C.Utility.Library;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.IO;
using System.Web.Configuration;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "FileManagement")]
    public class FileManagement
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public long ID { get; set; }

        /// <summary> 資料表名稱
        /// 
        /// </summary> 
        [Column]
        public string TableName { get; set; }

        /// <summary> 對應資料表ID
        /// 
        /// </summary> 
        [Column]
        public string TableID { get; set; }

        /// <summary> 檔案名稱
        /// 
        /// </summary> 
        [Column]
        public string FileName { get; set; }

        public string FileNameStr
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FilePath);
            }
        }

        /// <summary> 檔案大小
        /// 
        /// </summary> 
        [Column]
        public Int64 FileLength { get; set; }

        /// <summary> 檔案類型
        /// 
        /// </summary> 
        [Column]
        public string FileType { get; set; }

        /// <summary>
        /// 取得檔案副檔名
        /// </summary>
        /// <value>
        /// The file extestion.
        /// </value>
        public string FileExtension
        {
            get
            {
                if (FileType == null)
                {
                    return "";
                }

                string[] subs = FileType.Split('/');
                return subs[subs.Length - 1];
            }
        }

        /// <summary> 檔案路徑
        /// 
        /// </summary> 
        [Column]
        public string FilePath { get; set; }

        public string FileUrl
        {
            get
            {
                // 找到 UploadedFiles 字串的 Index
                int index = FilePath.IndexOf("\\UploadedFiles");
                string url = "";
                if (index > 0)
                {
                    url = FilePath.Substring(index);
                }
                else url = FilePath;
                return url;
            }
        }

        /// <summary> 檔案描述
        /// 
        /// </summary> 
        [Column]
        public string FileDescription { get; set; }

        /// <summary> MD5校驗碼
        /// 
        /// </summary> 
        [Column]
        public string MD5 { get; set; }

        /// <summary> 下載次數
        /// 
        /// </summary> 
        [Column]
        public int Times { get; set; }

        /// <summary> 上傳時間
        /// 
        /// </summary> 
        [Column(IsDbGenerated = true)]
        public DateTime UploadTime { get; set; }

        /// <summary> 是否刪除
        /// 
        /// </summary> 
        [Column]
        public bool? IsDelete { get; set; }

        /// <summary> 刪除時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? DeleteTime { get; set; }

        /// <summary>
        /// 取得檔案大小對應空間格式
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public string Size
        {
            get
            {
                return FileSizeFormatter.FormatSize(FileLength);
            }
        }
    }
}
