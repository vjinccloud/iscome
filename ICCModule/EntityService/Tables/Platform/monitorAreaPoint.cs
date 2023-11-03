using System;
using System.Data.Linq.Mapping;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "monitor_Area_Points")]
    public class monitorAreaPoint
    {
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 對應專案區域ID
        /// 
        /// </summary> 
         [Column(UpdateCheck=UpdateCheck.Never)]
        public int ProjectAreaID { get; set; }

        /// <summary> 里別
        /// 
        /// </summary> 
         [Column(UpdateCheck=UpdateCheck.Never)]
        public string Village { get; set; }

        /// <summary> 編號
        /// 
        /// </summary> 
         [Column(UpdateCheck=UpdateCheck.Never)]
        public string SerialNum { get; set; }

        /// <summary> GPS-北緯
        /// 
        /// </summary> 
         [Column(UpdateCheck=UpdateCheck.Never)]
        public decimal NorthLatitude { get; set; }

        /// <summary> GPS-東經
        /// 
        /// </summary> 
         [Column(UpdateCheck=UpdateCheck.Never)]
        public decimal EastLongitude { get; set; }

        /// <summary> 
        /// 作物類別
        /// </summary> 
         [Column(UpdateCheck=UpdateCheck.Never)]
        public int CropType { get; set; }
        

        /// <summary> 作物(多筆)
        /// 
        /// </summary> 
         [Column(UpdateCheck=UpdateCheck.Never)]
        public int Crops { get; set; }

        

        /// <summary> 備註
        /// 
        /// </summary> 
         [Column(UpdateCheck=UpdateCheck.Never)]
        public string Comment { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
         [Column(UpdateCheck=UpdateCheck.Never)]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column(UpdateCheck=UpdateCheck.Never)]
        public DateTime? UpdatedAt { get; set; }
    }
}
