using ICCModule.Entity.Tables;
using ICCModule.Entity.Tables.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InformationSystem.ViewModel.Event
{
    public class EventInfoViewModel
    {
        /// <summary>
        /// 類別查詢
        /// </summary>
        public List<defCode> Types { get; set; }

        /// <summary>
        /// 活動
        /// </summary>
        /// <value>
        /// The plan.
        /// </value>
        public activityPlan Plan { get; set; }

        public PlanField PlanField { get; set; }

        /// <summary>
        /// 活動場次
        /// </summary>
        /// <value>
        /// The open times.
        /// </value>
        public List<activityPlanOpenTime> OpenTimes { get; set; } = new List<activityPlanOpenTime>();

        /// <summary>
        /// 附加檔案
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public List<HttpPostedFileBase> Files { get; set; }
        /// <summary>
        /// 舊檔案
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public List<FileManagement> OldFiles { get; set; } = new List<FileManagement>();
        public List<long> EditOldFiles { get; set; } = new List<long>();
    }

    public class RegistSignModel
    {
        public string 序號 { get; set; }
        public string 姓名 { get; set; }
        public string 身分證字號 { get; set; }
        public string 認證時數 { get; set; }
    }
    public class SignInExportModel
    {
        public string 序號 { get; set; }
        public string 姓名 { get; set; }
        public string 身分證字號 { get; set; }
        public string 手機號碼 { get; set; }
    }
    public class AchievementModel
    {
        public activityPlanOpenTime Data { get; set; }
        public List<FileManagement> PicturesFiles { get; set; }
        public List<FileManagement> SignInResultFiles { get; set; }
    }

    public class OpenTimeQrcode
    {
        public string imgSrc { get; set; }
    }
}