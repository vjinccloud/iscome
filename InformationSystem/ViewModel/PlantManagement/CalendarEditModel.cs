using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Management
{
    public class CalendarEditModel
    {
        public CalendarEditModel()
        {
            ID = 0;
            Name = "";
            Type = "";
            Enable = true;
            Information = "";
            Explanation = "";
            ControlHistoryCrops = new List<controlHistoryCrop>();
        }
        /// <summary> 
        /// 流水號
        /// </summary> 
        public int ID { get; set; }

        /// <summary> 
        /// 防治名稱
        /// </summary> 
        [Required]
        public string Name { get; set; }

        /// <summary> 
        /// 類型
        /// </summary> 
        public string Type { get; set; }

        /// <summary> 
        /// 啟用狀態
        /// </summary> 
        public bool Enable { get; set; }

        /// <summary> 
        /// 基本資料 RichText
        /// </summary> 
        public string Information { get; set; }

        /// <summary> 
        /// 說明 RichText
        /// </summary> 
        public string Explanation { get; set; }
        /// <summary>
        /// 作物防治歷
        /// </summary>
        public List<controlHistoryCrop> ControlHistoryCrops { get; set; }

        /// <summary> 
        /// 說明 RichText
        /// </summary> 
        public string ActionName { get; set; }
        /// <summary> 
        /// 說明 RichText
        /// </summary> 
        public string ControlHistoryCropId { get; set; }
    }
}