using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using ICCModule.EntityService.Service;
using ICCModule.Models;
using ICCModule.ViewModel;
using InformationSystem.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InformationSystem.ViewModel.PlantDoctor
{
    public class TransSchedukeViewModel
    {
        public TransSchedukeViewModel()
        {
            LoginID = SessionHelper.Get("LoginID");
            RoleCode = Service_sysUserInfo.GetDetail(LoginID)?.RoleID ?? "";
        }
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 案號
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        public string LoginID { get; set; }

        /// <summary>
        /// 權限
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 當前值醫紀錄或新的
        /// </summary>
        public List<TransSchedukeSelectModel> doctorSchedule { get; set; }
    }
    public class TransSchedukeSelectModel
    {

        /// <summary>
        /// 識別ID
        /// </summary> 
        public long ID { get; set; }

        /// <summary>
        /// 案號
        /// </summary>
        public string CaseNo { get; set; }

        /// <summary>
        /// 轉案時間
        /// </summary>
        public string TransDate { get; set; }

        /// <summary>
        /// 轉案行政區
        /// </summary>
        public string TransDistrict { get; set; }
        
        /// <summary>
        /// 轉案植物醫師序號
        /// </summary>
        public string TransDocId { get; set; }
        
        /// <summary>
        /// 轉案植物醫師
        /// </summary>
        public string TransDocName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 作物名稱
        /// </summary> 
        public string CropName { get; set; }
        
        /// <summary>
        /// 作物名稱
        /// </summary> 
        public string Origin { get; set; }
    }
}