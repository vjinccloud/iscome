using ICCModule.EntityService.Service;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq.Expressions;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "crop_Types")]
    public class cropType
    {
        public cropType()
        {
            Sort = 0;
            Enable = true;
        }
        /// <summary> 流水號
        /// 
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int ID { get; set; }

        /// <summary> 類別名稱
        /// 
        /// </summary> 
        [Column]
        public string Name { get; set; }

        /// <summary> 排序
        /// 
        /// </summary> 
        [Column]
        public int Sort { get; set; }

        /// <summary> 狀態
        /// 
        /// </summary> 
        [Column]
        public bool Enable { get; set; }

        /// <summary> 創建時間
        /// 
        /// </summary> 
        [Column]
        public DateTime CreatedAt { get; set; }

        /// <summary> 更新時間
        /// 
        /// </summary> 
        [Column]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 取回所有相關的作物
        /// </summary>
        public List<crops> Crops
        {
            get
            {
                Expression<Func<crops, bool>> filters = x => x.Enable;
                filters = filters.And(x => x.TypeID == ID);
                return Service_crops.GetList(filters);
            }
        }

    }
}
