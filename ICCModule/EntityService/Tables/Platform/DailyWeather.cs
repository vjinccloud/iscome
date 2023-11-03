using ICCModule.EntityService.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Web.Mvc;

namespace ICCModule.Entity.Tables.Platform
{
    [Serializable]
    [Table(Name = "DailyWeather")]
    public class DailyWeather
    {
        /// <summary> 
        /// 流水號
        /// </summary> 
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary> 
        /// 資料日期
        /// </summary> 
        [Column]
        public DateTime DataDate { get; set; }

        /// <summary>
        /// 縣市名稱
        /// </summary>
        [Column]
        public string CityName { get; set; }
        /// <summary>
        /// 縣市編號
        /// </summary>
        [Column]
        public string CitySN { get; set; }
        /// <summary>
        /// 區域名稱
        /// </summary>
        [Column]
        public string DistrictName { get; set; }
        /// <summary>
        /// 區域編號
        /// </summary>
        [Column]
        public string DistrictSN { get; set; }
        /// <summary>
        /// 氣溫
        /// </summary>
        [Column]
        public decimal Temp { get; set; }
        /// <summary>
        /// 降雨量
        /// </summary>
        [Column]
        public decimal Rainfall { get; set; }
    }
}
